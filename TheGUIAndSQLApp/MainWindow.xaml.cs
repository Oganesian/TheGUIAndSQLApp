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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MySqlLib;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace TheGUIAndSQLApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            table.Items.Clear();
        }
        private void UsingSql(object sender, RoutedEventArgs e)
        {
            string sConn = @"Database = EmployeesBase; Data Source = localhost; User Id = root; Password =";
            MySqlLib.MySqlData.MySqlExecuteData.MyResultData result = new MySqlLib.MySqlData.MySqlExecuteData.MyResultData();
            result = MySqlLib.MySqlData.MySqlExecuteData.SqlReturnDataset(this.SQLQuery.Text, sConn);

            //MySqlLib.MySqlData.MySqlExecute.SqlNoneQuery(this.SQLQuery.Text, "Data Source=localhost;User Id=root;Password=q13466431");
              
             if (result.HasError == false)
             {
                table.ItemsSource = result.ResultData.DefaultView;
                for(int i = 0; i < table.Columns.Count; i++)
                {
                    if (table.Columns[i].Header.ToString() == "photopath")
                    {
                        table.Columns[i].Visibility = Visibility.Hidden;
                    }
                }
            }
             else
             {
                 MessageBox.Show(result.ErrorText);
             }
        }

        private void table_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string sConn = @"Database = EmployeesBase; Data Source = localhost; User Id = root; Password =";
            MySqlLib.MySqlData.MySqlExecuteData.MyResultData result = new MySqlLib.MySqlData.MySqlExecuteData.MyResultData();
            result = MySqlLib.MySqlData.MySqlExecuteData.SqlReturnDataset("SELECT photopath FROM Employees WHERE ID = " + (table.SelectedIndex+1).ToString() + ";", sConn);


            if (result.HasError == false)
            {
                if (result.ResultData.Rows.Count != 0)
                {
                    if (File.Exists(result.ResultData.Rows[0][0].ToString())){
                        employeersPhoto.Source = new BitmapImage(new Uri(result.ResultData.Rows[0][0].ToString(), UriKind.Absolute));
                    }
                    else
                    {
                        employeersPhoto.Source = new BitmapImage(new Uri(@"C:\Users\bezim\source\repos\TheGUIAndSQLApp\TheGUIAndSQLApp\bin\Debug\employersphoto\nophoto.jpg", UriKind.Absolute));
                    }
                }
            }
            else
            {
                MessageBox.Show(result.ErrorText);
            }
        }
    }
}
