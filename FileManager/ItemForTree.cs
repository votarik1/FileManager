using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Runtime.Serialization;

namespace FileManager
{

    public class ItemForTree : TreeViewItem // Взял готовый класс и добавил в него несколько своих свойств и конструктор
    {

        public string Path { get; set; }
        public string DirectoryPath { get; set; }
        public bool IsItFile { get; set; }
        public ItemForTree(string path, bool isItFile )
        {
            Path = path;
            IsItFile = isItFile;
        }

    }
}
