using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace FileManager
{
    public partial class MainWindow : Window
    {
        public static DriveInfo[] drive = DriveInfo.GetDrives(); // Массив корневых Дисков ПК
        public static Button[] buttons = new Button[drive.Length]; // Массив кнопок для корневых Дисков ПК
        static ItemForTree mainTree = new ItemForTree("", false); // Корень файлового дерева
        //Статичные переменные. Потом этим переменным будут заданы ссылки на текстовые объекты окна
        static TextBlock info1;
        static TextBlock info2;
        static TextBox TextAdress;
        static CopyOrDelete buffer = new CopyOrDelete(); // буфер для сохранения пути и флага копировать/переместить 
        public MainWindow()
        {
            InitializeComponent();

            DirectoryTree.Items.Add(mainTree);
            TextAdress = Adress;
            info1 = texBox;
            info2 = Information;
            for (int i = 0; i < drive.Length; i++)
            {
                buttons[i] = new Button();
                buttons[i].Content = drive[i].Name;
                buttons[i].Click += ClickDrive;
                stackPanelButtons.Children.Add(buttons[i]);
            }


        }

        static void GetAndIncertTwoSteps(ItemForTree dir)
        {
            try
            {
                GetAndIncert(dir);
            }
            catch (System.Exception ex)
            {
                ToWriteMistake(ex);
            }
            foreach (ItemForTree item in dir.Items)
            {
                try
                {
                    GetAndIncert(item);
                }
                catch (System.Exception)
                {
                    continue;
                }

            }
        }    // Считывает файловое дерево на 2 шага и достраивает дерево элементов ItemForTree
        static void GetAndIncert(ItemForTree dir)
        {
            IncertItem(dir, GetThisDirectory(dir));
        } // Считывает файловое дерево на 1 шаг и достраивает дерево элементов ItemForTree
        static void IncertItem(ItemForTree parent, ItemForTree[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                parent.Items.Add(arr[i]);
            }
        }    // Выстраивает массив "детей" под родительским ItemForTree
        static ItemForTree[] GetThisDirectory(ItemForTree thisDirectory)
        {

            string[] Dir = Directory.GetDirectories(thisDirectory.Path);
            string[] Files = Directory.GetFiles(thisDirectory.Path);
            ItemForTree[] output = new ItemForTree[(Dir.Length + Files.Length)];
            for (int i = 0; i < Dir.Length; i++)
            {
                output[i] = new ItemForTree(Dir[i], false);
                output[i].Header = TakeNameFromPath(Dir[i]);
                output[i].Expanded += Expand;
                output[i].Selected += Select;
                output[i].DirectoryPath = output[i].Path;
                AddContextMenu(output[i]);
            }
            for (int i = Dir.Length; i < Dir.Length + Files.Length; i++)
            {
                output[i] = new ItemForTree(Files[i - Dir.Length], true);
                output[i].Header = TakeNameFromPath(Files[i - Dir.Length]);
                output[i].Selected += Select;
                output[i].DirectoryPath = Files[i - Dir.Length].Substring(0, (Files[i - Dir.Length].Length) - (output[i].Header.ToString().Length));
                AddContextMenu(output[i]);
            }
            return output;
        } //Получает все элементы в указанной директории, заполняет ими массив ItemForTree и возвращает его
        static string TakeNameFromPath(string Path)
        {
            string[] output = Path.Split('\\', System.StringSplitOptions.None);
            return output[output.Length - 1];
        }

        static string MakeShort(string str, int howMuch)
        {
            if (str.Length > howMuch)
            {
                return (str.Substring(0, howMuch - 3) + "...");
            }
            else
            {
                return str;
            }
        } //Укорачивает строку, добовляет в конце ...

        static string GetBytes(long bytes)
        {
            double size = bytes;
            int order = 0;
            while (size >= 1024 && order < 4)
            {
                size /= 1024;
                order++;
            }
            switch (order)
            {
                case 0:
                    return $"{size.ToString("#.##")} байт";
                case 1:
                    return $"{size.ToString("#.##")} КБ";
                case 2:
                    return $"{size.ToString("#.##")} МБ";
                case 3:
                    return $"{size.ToString("#.##")} ГБ";
                default:
                    return $"{size.ToString("#.##")} ТБ";
            }


        }   // Перевод байтов в более адекватную единицу измерения

        static string GetInfo1(ItemForTree selectedItem)
        {
            StringBuilder stringBuilder = new StringBuilder();
            DirectoryInfo directory = new DirectoryInfo(selectedItem.Path);
            if (directory.Exists)
            {
                stringBuilder.Append($"Тип:\n\n");
                stringBuilder.Append($"Имя:\n\n");
                stringBuilder.Append($"Расположение:\n\n");
                stringBuilder.Append($"Cодержит:  \n\n\n");
                stringBuilder.Append($"Cоздан:  \n\n");
                stringBuilder.Append($"Изменён: \n\n");
            }
            else
            {
                stringBuilder.Append($"Тип:\n\n");
                stringBuilder.Append($"Имя:\n\n");
                stringBuilder.Append($"Расположение:\n\n");
                stringBuilder.Append($"Размер:  \n\n");
                stringBuilder.Append($"Cоздан:  \n\n");
                stringBuilder.Append($"Изменён:  \n\n");
            }

            return stringBuilder.ToString();
        }   // Заполнение левой части информационного поля
        static string GetInfo2(ItemForTree selectedItem)
        {
            StringBuilder stringBuilder = new StringBuilder();
            DirectoryInfo directory = new DirectoryInfo(selectedItem.Path);
            FileInfo file = new FileInfo(selectedItem.Path);
            if (directory.Exists)
            {
                if (selectedItem == mainTree)
                {
                    stringBuilder.Append($"Диск \n\n");
                }
                else
                {
                    stringBuilder.Append($"Папка с файлами\n\n");
                }
                stringBuilder.Append($"{MakeShort(directory.Name, 20)} \n\n");
                stringBuilder.Append($"{MakeShort(directory.FullName, 20)} \n\n");
                stringBuilder.Append($"{directory.GetFiles().Length} файлов\n");
                stringBuilder.Append($"{directory.GetDirectories().Length} папок\n\n");
                stringBuilder.Append($"{directory.CreationTime}\n\n");
                stringBuilder.Append($"{directory.LastWriteTime}\n");
            }
            else
            {
                stringBuilder.Append($"Файл\n\n");
                stringBuilder.Append($"{MakeShort(file.Name, 20)} \n\n");
                stringBuilder.Append($"{MakeShort(file.FullName, 20)} \n\n");
                stringBuilder.Append($"{GetBytes(file.Length)} \n\n");
                stringBuilder.Append($"{file.CreationTime}\n\n");
                stringBuilder.Append($"{file.LastWriteTime}\n");
            }

            return stringBuilder.ToString();
        }  // Заполнение правой части информационного поля

        static void AddContextMenu(ItemForTree item)
        {
            MenuItem[] menuItems = new MenuItem[5];
            for (int j = 0; j < menuItems.Length; j++)
            {
                menuItems[j] = new MenuItem();
            }
            menuItems[0].Header = "Копировать";
            menuItems[0].Click += Copy;
            menuItems[1].Header = "Вставить";
            menuItems[1].Click += Incert;
            menuItems[2].Header = "Переместить";
            menuItems[2].Click += MoveTo;
            menuItems[3].Header = "Создать";
            MenuItem createDirectory = new MenuItem();
            createDirectory.Header = "Папку";
            createDirectory.Click += Create;
            MenuItem createFile = new MenuItem();
            createFile.Header = "Файл";
            createFile.Click += Create;
            menuItems[3].Items.Add(createDirectory);
            menuItems[3].Items.Add(createFile);
            menuItems[4].Header = "Удалить";
            menuItems[4].Click += Delete;
            item.ContextMenu = new ContextMenu();
            item.ContextMenu.PlacementTarget = item;
            for (int j = 0; j < menuItems.Length; j++)
            {
                item.ContextMenu.Items.Add(menuItems[j]);
            }
        }     // Добавление контекстного меню к объекту ItemForTree

        static void Update(ItemForTree item)
        {
            if (item.IsItFile)
            {
                ItemForTree parent = (ItemForTree)(item.Parent);
                GetAndIncertTwoSteps(parent);
                parent.IsExpanded = false;
                parent.IsExpanded = true;
            }
            else
            {
                GetAndIncertTwoSteps(item);
                item.IsExpanded = false;
                item.IsExpanded = true;
            }



        }        //Обновление дерева

        //Методы для событий
        static void Select(object o, RoutedEventArgs a)
        {




            ItemForTree select = (ItemForTree)o;
            TextAdress.Text = select.Path;
            try
            {
                info1.Text = GetInfo1(select);
                info2.Text = GetInfo2(select);
            }
            catch (System.Exception ex)
            {
                ToWriteMistake(ex);
            }

            a.Handled = true;
        }  //Выделение объекта ItemForTree
        public static void Expand(object o, RoutedEventArgs a)
        {

            ItemForTree cat = (ItemForTree)o;
            cat.Items.Clear();
            GetAndIncertTwoSteps(cat);
            a.Handled = true;
        } //Раскрытие объекта ItemForTree
        public static void ClickDrive(object o, RoutedEventArgs a)
        {

            foreach (var item in buttons)
            {
                item.IsEnabled = true;
            }
            Button button = (Button)o;
            button.IsEnabled = false;
            mainTree.Items.Clear();
            mainTree.Path = button.Content.ToString();
            mainTree.IsExpanded = true;
            mainTree.Expanded += Expand;
            mainTree.Header = mainTree.Path;
            mainTree.DirectoryPath = mainTree.Path;
            mainTree.Selected += Select;
            AddContextMenu(mainTree);
            GetAndIncertTwoSteps(mainTree);
        }    //Нажатие кнопки корневого диска

        static void Copy(object o, RoutedEventArgs e)
        {

            ContextMenu z = (ContextMenu)(((MenuItem)o).Parent);
            ItemForTree selected = (ItemForTree)(z.PlacementTarget);
            buffer.bufferPath = selected.Path;
            buffer.copy = true;

        }       //Нажатие копировать в контекстном меню

        static void Incert(object o, RoutedEventArgs e)
        {
            if (buffer.bufferPath == null)
            {
                MessageBox.Show("Буфер обмена пуст");
            }
            else
            {
                ContextMenu z = (ContextMenu)(((MenuItem)o).Parent);
                ItemForTree selected = (ItemForTree)(z.PlacementTarget);

                try
                {
                    CopyOrDelete.CopyAllDirectoryOrFile(buffer.bufferPath, selected.DirectoryPath);

                    if (buffer.copy == false && selected != mainTree)
                    {
                        CopyOrDelete.DeleteAllDirectoryOrFile(buffer.bufferPath);
                        buffer.bufferPath = null;
                    }
                }
                catch (System.Exception ex)
                {
                    ToWriteMistake(ex);
                }
                Update(selected);

            }
        }   //Нажатие вставить в контекстном меню

        static void MoveTo(object o, RoutedEventArgs e)
        {
            Copy(o, e);
            buffer.copy = false;
        }  //Нажатие переместить в контекстном меню

        static void Create(object o, RoutedEventArgs e)
        {

            MenuItem x = (MenuItem)o;
            MenuItem y = (MenuItem)(x.Parent);
            ContextMenu z = (ContextMenu)(y.Parent);
            ItemForTree selected = (ItemForTree)(z.PlacementTarget);

            if (x.Header.ToString() == "Файл")
            {
                WindowEnterName windowEnterName = new WindowEnterName();
                windowEnterName.Owner = GetWindow(mainTree);
                windowEnterName.inscription.Text = "Введите имя нового файла";
                windowEnterName.path = selected.DirectoryPath;
                windowEnterName.boolFile = true;
                windowEnterName.ShowDialog();

            }
            else if (x.Header.ToString() == "Папку")
            {
                WindowEnterName windowEnterName = new WindowEnterName();
                windowEnterName.Owner = GetWindow(mainTree);
                windowEnterName.inscription.Text = "Введите имя новой папки";
                windowEnterName.path = selected.DirectoryPath;
                windowEnterName.boolFile = false;
                windowEnterName.ShowDialog();
            }
            Update(selected);
        }  //Нажатие создать в контекстном меню

        static void Delete(object o, RoutedEventArgs e)
        {
            ContextMenu z = (ContextMenu)(((MenuItem)o).Parent);
            ItemForTree selected = (ItemForTree)(z.PlacementTarget);
            if (selected != mainTree)
            {
                ItemForTree parent = (ItemForTree)(selected.Parent);
                try
                {
                    CopyOrDelete.DeleteAllDirectoryOrFile(selected.Path);
                }
                catch (System.Exception ex)
                {
                    ToWriteMistake(ex);
                }
                GetAndIncertTwoSteps(parent);
                parent.IsExpanded = false;
                parent.IsExpanded = true;
            }
            else
            {
                MessageBox.Show($"Невозможно удалить корневой каталог {selected.Path}");
            }

        }  //Нажатие удалить в контекстном меню

        void Comand(object o, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string comand = (cmd.Text.Split(" ")[0]).ToLower();
                string parametr = cmd.Text.Substring(comand.Length);
                if (parametr == "")
                {
                    MessageBox.Show("Параметр не задан");
                    return;
                }
                while (parametr[0] == ' ' && parametr != " ")
                {
                    parametr = parametr.Substring(1);
                }
                string[] parametrs = parametr.Split('"', System.StringSplitOptions.RemoveEmptyEntries);
                switch (comand)
                {
                    case "copy":
                        if (parametrs.Length == 3)
                        {
                            try
                            {
                                CopyOrDelete.CopyAllDirectoryOrFile(parametrs[0], parametrs[2]);
                                MessageBox.Show($"Копирование завершено");
                                Update(mainTree);
                                cmd.Text = "";
                            }
                            catch (System.Exception ex)
                            {
                                ToWriteMistake(ex);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Неверно заданы пареметры");
                        }
                        break;
                    case "delete":
                        if (parametrs.Length > 0)
                        {
                            try
                            {
                                CopyOrDelete.DeleteAllDirectoryOrFile(parametrs[0]);
                                MessageBox.Show($"Удаление завершено");
                                Update(mainTree);
                                cmd.Text = "";
                            }
                            catch (System.Exception ex)
                            {
                                ToWriteMistake(ex);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Неверно заданы пареметры");
                        }
                        break;
                    case "move":
                        if (parametrs.Length == 3)
                        {
                            try
                            {
                                CopyOrDelete.CopyAllDirectoryOrFile(parametrs[0], parametrs[2]);
                                CopyOrDelete.DeleteAllDirectoryOrFile(parametrs[0]);
                                MessageBox.Show($"Перемещение завершено");
                                Update(mainTree);
                                cmd.Text = "";
                            }
                            catch (System.Exception ex)
                            {
                                ToWriteMistake(ex);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Неверно заданы пареметры");
                        }
                        break;
                    case "create":
                        if (parametrs.Length > 0)
                        {

                            if (parametrs.Length == 3 && parametrs[2] == "f")
                            {
                                try
                                {
                                    File.Create(parametrs[0]);
                                    MessageBox.Show("Файл создан");
                                    Update(mainTree);
                                    cmd.Text = "";
                                }
                                catch (System.Exception ex)
                                {
                                    ToWriteMistake(ex);
                                }
                            }

                            else
                            {
                                try
                                {
                                    Directory.CreateDirectory(parametrs[0]);
                                    MessageBox.Show("Каталог создан");
                                    Update(mainTree);
                                    cmd.Text = "";
                                }
                                catch (System.Exception ex)
                                {
                                    ToWriteMistake(ex);
                                }
                            }


                        }
                        else
                        {
                            MessageBox.Show("Неверно заданы пареметры");
                        }
                        break;
                    default:
                        MessageBox.Show($"Команда {comand} не предусмотрена разработчиком");
                        break;
                }



            }
        }    //Нажатие кнопки Enter после ввода команды

        private void OpenHelp(object o, RoutedEventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("Команды для командной строки:\n\n");
            stringBuilder.Append("Копировать: copy \"путь(что?)\" \"путь(куда)\" \n");
            stringBuilder.Append("Переместить: move \"путь(что?)\" \"путь(куда)\" \n");
            stringBuilder.Append("Удалить: delete \"путь(что?)\")\n");
            stringBuilder.Append("Создать файл: create \"путь(что?)\" \"f\"\n");
            stringBuilder.Append("Создать папку: create \"путь(что?)\"\n\n");
            stringBuilder.Append("Или вызвать контекстное меню, нажав ПКМ на нужном элементе");
            MessageBox.Show(stringBuilder.ToString());


        } //Нажатие кнопки Help

        static void ToWriteMistake(System.Exception ex)
        {
            MessageBox.Show(ex.Message);
            try
            {
                File.AppendAllText("Errors.log", $"{DateTime.Now} {ex.Message}\n");
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось записать ошибку в Errors.log");
            }
            
        } //Лог ошибок
    }
}
