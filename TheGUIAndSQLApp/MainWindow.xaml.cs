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
using MahApps.Metro.Controls.Dialogs;

namespace TheGUIAndSQLApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        bool dataBaseSelected = false;
        public MainWindow()
        {
            InitializeComponent();
            table.Items.Clear();
            dataBases.Items.Clear();
        }

        private async void CloseCustomDialog(object sender, RoutedEventArgs e)
        {
            var dialog = (BaseMetroDialog)this.Resources["CustomCloseDialogTest"];
            await this.HideMetroDialogAsync(dialog);
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

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AuthGrid.Visibility = Visibility.Collapsed;
            MainGrid.Visibility = Visibility.Visible;
        }

        private void selectTable()
        {
            string sConn = @"Database = EmployeesBase; Data Source = localhost; User Id = root; Password =";
            MySqlLib.MySqlData.MySqlExecuteData.MyResultData result = new MySqlLib.MySqlData.MySqlExecuteData.MyResultData();
            result = MySqlLib.MySqlData.MySqlExecuteData.SqlReturnDataset("SELECT * FROM Employees", sConn);

            //MySqlLib.MySqlData.MySqlExecute.SqlNoneQuery(this.SQLQuery.Text, "Data Source=localhost;User Id=root;Password=q13466431");

            if (result.HasError == false)
            {
                table.ItemsSource = result.ResultData.DefaultView;
            }
            else
            {
                MessageBox.Show(result.ErrorText);
            }
        }
        private void selectDataBases()
        {
            string sConn = @"Database = EmployeesBase; Data Source = localhost; User Id = root; Password =";
            MySqlLib.MySqlData.MySqlExecuteData.MyResultData result = new MySqlLib.MySqlData.MySqlExecuteData.MyResultData();
            result = MySqlLib.MySqlData.MySqlExecuteData.SqlReturnDataset("SHOW DATABASES", sConn);

            //MySqlLib.MySqlData.MySqlExecute.SqlNoneQuery(this.SQLQuery.Text, "Data Source=localhost;User Id=root;Password=q13466431");

            if (result.HasError == false)
            {
                dataBases.ItemsSource = result.ResultData.DefaultView;
                dataBases.Columns[0].Width = dataBases.Width - 1;
                dataBases.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show(result.ErrorText);
            }
        }

        private void DataBase_Open(object sender, RoutedEventArgs e)
        {
            string str = ((DataRowView)dataBases.SelectedItems[0]).Row["DATABASE"].ToString();
            string sConn = @"Database = " + str + "; Data Source = localhost; User Id = root; Password =";
            MySqlLib.MySqlData.MySqlExecuteData.MyResultData result = new MySqlLib.MySqlData.MySqlExecuteData.MyResultData();
            result = MySqlLib.MySqlData.MySqlExecuteData.SqlReturnDataset("SHOW TABLES", sConn);
            if (result.HasError == false)
            {
                dataBaseSelected = true;
                tabControl.SelectedIndex = 1;
                tabControl.SelectedItem = tablesTab;
                tablesTab.IsSelected = true;
                table.ItemsSource = result.ResultData.DefaultView;
            }
            else
            {
                MessageBox.Show(result.ErrorText);
            }
        }

        private void DataBase_Delete(object sender, RoutedEventArgs e)
        {
            string str = ((DataRowView)dataBases.SelectedItems[0]).Row["DATABASE"].ToString();
            showAcceptDropDBDialog("DROP DATABASE `" + str + "`");
        }

        private void TabItem_Loaded(object sender, RoutedEventArgs e)
        {
            selectDataBases();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataBaseSelected == false && tabControl.SelectedIndex == 1)
            {
                showChooseDBDialog();
                tabControl.SelectedIndex = 0;
                tabControl.SelectedItem = dataBasesTab;
                dataBasesTab.IsSelected = true;
                tablesTab.IsSelected = false;
                SQLTab.IsSelected = false;
            }
        }
        private async void showChooseDBDialog()
        {
            EventHandler<DialogStateChangedEventArgs> dialogManagerOnDialogOpened = null;
            dialogManagerOnDialogOpened = (o, args) => {
                DialogManager.DialogOpened -= dialogManagerOnDialogOpened;
            };
            DialogManager.DialogOpened += dialogManagerOnDialogOpened;

            EventHandler<DialogStateChangedEventArgs> dialogManagerOnDialogClosed = null;
            dialogManagerOnDialogClosed = (o, args) => {
                DialogManager.DialogClosed -= dialogManagerOnDialogClosed;
            };
            DialogManager.DialogClosed += dialogManagerOnDialogClosed;

            var dialog = (BaseMetroDialog)this.Resources["CustomCloseDialogTest"];

            await this.ShowMetroDialogAsync(dialog);
            await dialog.WaitUntilUnloadedAsync();
        }

 

            private async void showAcceptDropDBDialog(string dropStr)
            {
            // This demo runs on .Net 4.0, but we're using the Microsoft.Bcl.Async package so we have async/await support
            // The package is only used by the demo and not a dependency of the library!
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Да",
                NegativeButtonText = "Нет",
                ColorScheme = MetroDialogOptions.ColorScheme
            };

            MessageDialogResult result = await this.ShowMessageAsync("Вы действительно хотите выполнить запрос?", dropStr,
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if(result == MessageDialogResult.Affirmative)
            {
                MySqlLib.MySqlData.MySqlExecute.SqlNoneQuery(dropStr, "Data Source=localhost;User Id=root;Password=");
                selectDataBases();
            }
        }

        private void DataBase_Create(object sender, RoutedEventArgs e)
        {

        }
    }
}
