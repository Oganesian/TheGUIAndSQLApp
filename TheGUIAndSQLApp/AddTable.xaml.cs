using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TheGUIAndSQLApp
{
    /// <summary>
    /// Логика взаимодействия для AddTable.xaml
    /// </summary>
    public partial class AddTable
    {
        public AddTable()
        {
            InitializeComponent();
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
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int columnsNumberI;
            int.TryParse(columnsNumber.Text, out columnsNumberI);
            CreateTable create = new CreateTable(tableName.Text, columnsNumberI);
            create.Owner = this.Owner;
            create.Show();
            this.Close();
        }
    }
}
