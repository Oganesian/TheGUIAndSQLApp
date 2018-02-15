using System.Windows;

namespace TheGUIAndSQLApp
{
    /// <summary>
    /// Логика взаимодействия для AddDataBase.xaml
    /// </summary>
    public partial class AddDataBase
    {
        private string connectString;
        public AddDataBase(string cS)
        {
            InitializeComponent();
            connectString = cS;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(App.Current.MainWindow != null)
            {
                App.Current.MainWindow.IsEnabled = true;
                App.Current.MainWindow.Activate();
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
                MySqlLib.MySqlData.MySqlExecute.SqlNoneQuery(createStr, connectString);
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
