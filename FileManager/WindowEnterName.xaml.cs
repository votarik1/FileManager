using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace FileManager
{

    public partial class WindowEnterName : Window
    {

        public string path { get; set; }
        public bool boolFile { get; set; }
        public WindowEnterName()
        {
            InitializeComponent();
            name.Focus();
        }

        private void ClickOk(Object o, RoutedEventArgs e)
        {
            string totalPath = System.IO.Path.Combine(path, name.Text);
            if (boolFile)
            {
                try
                {
                    File.Create(totalPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(totalPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            Close();
        }

        private void ClickCancel(Object o, RoutedEventArgs e)
        {
            Close();
        }

        private void ToEnter(Object o, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ClickOk(new object(), new RoutedEventArgs());
            }
        }
    }
}
