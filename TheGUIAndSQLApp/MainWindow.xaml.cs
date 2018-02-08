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
        bool tableSelected = false;
        string currentDataBase = "";
        string currentTable = "";
        public MainWindow()
        {
            InitializeComponent();
            table.Items.Clear();
            dataBases.Items.Clear();
            openedTableGrid.Items.Clear();
        }

        private void CloseCustomDialog(object sender, RoutedEventArgs e)
        {
            CloseCustomDialogRealize(1);
        }
        private void CloseCustomDialog1(object sender, RoutedEventArgs e)
        {
            CloseCustomDialogRealize(2);
        }
        private async void CloseCustomDialogRealize(int id)
        {
            string resource = "";
            switch (id)
            {
                case 1: resource = "CustomCloseDialogTest";  break;
                case 2: resource = "CustomCloseTDialogTest"; break;
                default: MessageBox.Show("Ошибка"); break;
            }
            var dialog = (BaseMetroDialog)this.Resources[resource];
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
        public void SelectDataBases()
        {
            string sConn = @"Database = EmployeesBase; Data Source = localhost; User Id = root; Password =";
            MySqlLib.MySqlData.MySqlExecuteData.MyResultData result = new MySqlLib.MySqlData.MySqlExecuteData.MyResultData();
            result = MySqlLib.MySqlData.MySqlExecuteData.SqlReturnDataset("SHOW DATABASES", sConn);

            //MySqlLib.MySqlData.MySqlExecute.SqlNoneQuery(this.SQLQuery.Text, "Data Source=localhost;User Id=root;Password=q13466431");

            if (result.HasError == false)
            {
                dataBases.ItemsSource = result.ResultData.DefaultView;
                dataBases.Columns[0].Width = dataBases.Width - 1;
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
                currentDataBase = str;
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
            SelectDataBases();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataBaseSelected == false && tabControl.SelectedIndex == 1)
            {
                showChooseDialog(1);
            }
            if (tableSelected == false && tabControl.SelectedIndex == 2)
            {
                showChooseDialog(2);
            }
        }
        private async void showChooseDialog(int type)
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

            BaseMetroDialog dialog;

            switch (type)
            {
                case 1:
                    dialog = (BaseMetroDialog)this.Resources["CustomCloseDialogTest"];
                    await this.ShowMetroDialogAsync(dialog);
                    await dialog.WaitUntilUnloadedAsync();
                    tabControl.SelectedIndex = 0;
                    tabControl.SelectedItem = dataBasesTab;
                    dataBasesTab.IsSelected = true;
                    break;
                case 2:
                    dialog = (BaseMetroDialog)this.Resources["CustomCloseTDialogTest"];
                    await this.ShowMetroDialogAsync(dialog);
                    await dialog.WaitUntilUnloadedAsync();
                    tabControl.SelectedIndex = 1;
                    tabControl.SelectedItem = tablesTab;
                    tablesTab.IsSelected = true;
                    break;
                default: break;
            }
        }

            private async void showAcceptDropDBDialog(string dropStr)
            {
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
                SelectDataBases();
            }
        }

        private void DataBase_Create(object sender, RoutedEventArgs e)
        {
            AddDataBase addDataBase = new AddDataBase();
            addDataBase.Owner = this;
            addDataBase.Show();
            addDataBase.Activate();
            this.IsEnabled = false;
        }

        private void DataBase_Update(object sender, RoutedEventArgs e)
        {
            SelectDataBases();
        }

        private void Table_Open(object sender, RoutedEventArgs e)
        {
            string str = ((DataRowView)table.SelectedItems[0]).Row[0].ToString();
            string sConn = @"Database = " + currentDataBase + "; Data Source = localhost; User Id = root; Password =";
            MySqlLib.MySqlData.MySqlExecuteData.MyResultData result = new MySqlLib.MySqlData.MySqlExecuteData.MyResultData();
            result = MySqlLib.MySqlData.MySqlExecuteData.SqlReturnDataset("SELECT * FROM "+str, sConn);
            if (result.HasError == false)
            {
                tableSelected = true;
                currentTable = str;
                tabControl.SelectedIndex = 2;
                tabControl.SelectedItem = openedTable;
                openedTable.IsSelected = true;
                openedTableGrid.ItemsSource = result.ResultData.DefaultView;
            }
            else
            {
                MessageBox.Show(result.ErrorText);
            }
        }

        private void Table_Create(object sender, RoutedEventArgs e)
        {
            AddTable addTable = new AddTable();
            addTable.Owner = this;
            addTable.Show();
            addTable.Activate();
            this.IsEnabled = false;
        }

        private void Table_Update(object sender, RoutedEventArgs e)
        {
            string sConn = @"Database = " + currentDataBase + "; Data Source = localhost; User Id = root; Password =";
            MySqlLib.MySqlData.MySqlExecuteData.MyResultData result = new MySqlLib.MySqlData.MySqlExecuteData.MyResultData();
            result = MySqlLib.MySqlData.MySqlExecuteData.SqlReturnDataset("SHOW TABLES", sConn);
            if (result.HasError == false)
            {
                table.ItemsSource = result.ResultData.DefaultView;
            }
            else
            {
                MessageBox.Show(result.ErrorText);
            }
        }

        private void Table_Delete(object sender, RoutedEventArgs e)
        {

        }
    }
}
