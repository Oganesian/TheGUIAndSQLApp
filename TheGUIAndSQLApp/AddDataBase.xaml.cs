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
    /// Логика взаимодействия для AddDataBase.xaml
    /// </summary>
    public partial class AddDataBase
    {

        public AddDataBase()
        {
            InitializeComponent();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(App.Current.MainWindow != null)
            {
                App.Current.MainWindow.IsEnabled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string comboSelectedValue = charSet.SelectedItem.ToString().Replace("System.Windows.Controls.ComboBoxItem: ", "");
            string charSetS = getCharSet(comboSelectedValue);
            string createStr = "CREATE DATABASE `";
            createStr += this.DBName.Text + "` ";
            if(comboSelectedValue != "binary")
            {
                createStr += "DEFAULT CHARACTER SET " + charSetS + " COLLATE " + comboSelectedValue;
            }
            else
            {
                createStr += "DEFAULT CHARACTER SET binary";
            }

            MySqlLib.MySqlData.MySqlExecute.MyResult result =
                MySqlLib.MySqlData.MySqlExecute.SqlNoneQuery(createStr, "Data Source=localhost;User Id=root;Password=");
            if (result.HasError == false)
            {
                MainWindow wnd = (MainWindow)App.Current.MainWindow;
                wnd.SelectDataBases();
                this.Close();
            }
            else
            {
                MessageBox.Show(result.ErrorText);
            }
        }
        private string getCharSet(string cSV)
        {
            if(cSV.IndexOf('_') != -1)
            {
                return cSV.Substring(0, cSV.IndexOf('_'));
            }
            else
            {
                return "binary";
            }
        }
    }
}
