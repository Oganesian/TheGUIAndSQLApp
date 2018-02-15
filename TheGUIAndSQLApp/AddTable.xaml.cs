using System;
using System.Windows;
using System.Windows.Input;

namespace TheGUIAndSQLApp
{
    /// <summary>
    /// Логика взаимодействия для AddTable.xaml
    /// </summary>
    public partial class AddTable
    {
        private string currentDB;
        private string connectString;

        public AddTable(string cDB, string connStr)
        {
            InitializeComponent();
            currentDB = cDB;
            connectString = connStr;
        }

        private void columnsNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (App.Current.MainWindow != null)
            {
                App.Current.MainWindow.IsEnabled = true;
                App.Current.MainWindow.Activate();
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int columnsNumberI;
            int.TryParse(columnsNumber.Text, out columnsNumberI);
            CreateTable create = new CreateTable(tableName.Text, connectString, currentDB, columnsNumberI);
            create.Owner = this.Owner;
            create.Show();
            this.Close();
        }
    }
}
