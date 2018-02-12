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
        public string currentDataBase = "";
        string currentTable = "";

        public MainWindow()
        {
            InitializeComponent();
            table.Items.Clear();
            dataBases.Items.Clear();
            openedTableGrid.Items.Clear();
            describeTable.Items.Clear();
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

        }

        private void Auth_Click(object sender, RoutedEventArgs e)
        {
            AuthGrid.Visibility = Visibility.Collapsed;
            MainGrid.Visibility = Visibility.Visible;
        }

        private void selectTable()
        {
            string sConn = @"Database = EmployeesBase; Data Source = localhost; User Id = root; Password =";
            MySqlLib.MySqlData.MySqlExecuteData.MyResultData result = new MySqlLib.MySqlData.MySqlExecuteData.MyResultData();
            result = MySqlLib.MySqlData.MySqlExecuteData.SqlReturnDataset("SELECT * FROM Employees", sConn);
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
            if (dataBases.SelectedItem == null)
            {
                somethingAintSelected("База данных не выбрана", "Выберите базу данных, которую хотите открыть");
            }
            else
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
        }

        private void DataBase_Delete(object sender, RoutedEventArgs e)
        {
            if (dataBases.SelectedItem == null)
            {
                somethingAintSelected("База данных не выбрана", "Выберите базу данных, которую хотите удалить");
            }
            else
            {
                string str = ((DataRowView)dataBases.SelectedItems[0]).Row["DATABASE"].ToString();
                showAcceptDropDBDialog("DROP DATABASE `" + str + "`");
            }
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
            if (tableSelected == false && (tabControl.SelectedIndex == 2 ||
                tabControl.SelectedIndex == 3))
            {
                showChooseDialog(2);
            }
            switch (tabControl.SelectedIndex)
            {
                case 2:
                    statusBar.Visibility = Visibility.Visible;
                    statusBarType.Content = "Содержание таблицы";
                    statusBarSelected.Content = currentTable;
                    break;
                case 3:
                    statusBar.Visibility = Visibility.Visible;
                    statusBarType.Content = "Структура таблицы";
                    statusBarSelected.Content = currentTable;
                    break;
                default: statusBar.Visibility = Visibility.Collapsed; break;
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

            private async void showAcceptDropDBDialog(string queryStr)
            {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Да",
                NegativeButtonText = "Нет",
                ColorScheme = MetroDialogOptions.ColorScheme
            };

            MessageDialogResult result = await this.ShowMessageAsync("Вы действительно хотите выполнить запрос?", queryStr,
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if(result == MessageDialogResult.Affirmative)
            {
                if(queryStr.Contains("DROP TABLE `") || queryStr.Contains("TRUNCATE `"))
                {
                    MySqlLib.MySqlData.MySqlExecute.SqlNoneQuery(queryStr, "Database = " + currentDataBase + "; Data Source=localhost;User Id=root;Password=");
                    if(tabControl.SelectedIndex == 2 || queryStr.Contains(currentTable))
                    {
                        tabControl.SelectedIndex = 1;
                        tableSelected = false;
                    }
                    updateCurrentTable();
                }
                else
                {
                    MySqlLib.MySqlData.MySqlExecute.SqlNoneQuery(queryStr, "Data Source=localhost;User Id=root;Password=");
                    if(queryStr.Contains(currentDataBase))
                    {
                        tabControl.SelectedIndex = 0;
                        tableSelected = false;
                        dataBaseSelected = false;
                    }
                    SelectDataBases();
                }
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
            if (table.SelectedItem == null)
            {
                somethingAintSelected("Таблица не выбрана", "Выберите таблицу, которую хотите открыть");
            }
            else
            {
                string str = ((DataRowView)table.SelectedItems[0]).Row[0].ToString();
                string sConn = @"Database = " + currentDataBase + "; Data Source = localhost; User Id = root; Password =";
                MySqlLib.MySqlData.MySqlExecuteData.MyResultData result = new MySqlLib.MySqlData.MySqlExecuteData.MyResultData();
                result = MySqlLib.MySqlData.MySqlExecuteData.SqlReturnDataset("SELECT * FROM " + str, sConn);
                if (result.HasError == false)
                {
                    tableSelected = true;
                    currentTable = str;
                    tabControl.SelectedIndex = 2;
                    tabControl.SelectedItem = openedTable;
                    openedTable.IsSelected = true;
                    openedTableGrid.ItemsSource = result.ResultData.DefaultView;
                    describeTableFoo();
                }
                else
                {
                    MessageBox.Show(result.ErrorText);
                }
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
            updateCurrentTable();
        }
        public void updateCurrentTable()
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
            if (table.SelectedItem == null)
            {
                somethingAintSelected("Таблица не выбрана", "Выберите таблицу, которую хотите удалить");
            }
            else
            {
                string str = ((DataRowView)table.SelectedItems[0]).Row[0].ToString();
                showAcceptDropDBDialog("DROP TABLE `" + str + "`");
            }
        }

        private void showStructure(object sender, RoutedEventArgs e)
        {
            describeTableFoo();
            tabControl.SelectedIndex = 3;
        }
        private void describeTableFoo()
        {
            if (tableSelected == false)
            {
                showChooseDialog(2);
            }
            else
            {
                string sConn = @"Database = " + currentDataBase + "; Data Source = localhost; User Id = root; Password =";
                MySqlLib.MySqlData.MySqlExecuteData.MyResultData result = new MySqlLib.MySqlData.MySqlExecuteData.MyResultData();
                result = MySqlLib.MySqlData.MySqlExecuteData.SqlReturnDataset("DESCRIBE `" + currentTable + "`", sConn);
                if (result.HasError == false)
                {
                    describeTable.ItemsSource = result.ResultData.DefaultView;
                }
                else
                {
                    MessageBox.Show(result.ErrorText);
                }
            }
        }

        private void truncateTable(object sender, RoutedEventArgs e)
        {
            showAcceptDropDBDialog("TRUNCATE `" + currentTable + "`");
        }

        private void updateTable(object sender, RoutedEventArgs e)
        {
            string sConn = @"Database = " + currentDataBase + "; Data Source = localhost; User Id = root; Password =";
            MySqlLib.MySqlData.MySqlExecuteData.MyResultData result = new MySqlLib.MySqlData.MySqlExecuteData.MyResultData();
            result = MySqlLib.MySqlData.MySqlExecuteData.SqlReturnDataset("SELECT * FROM " + currentTable, sConn);
            if (result.HasError == false)
            {
                openedTableGrid.ItemsSource = result.ResultData.DefaultView;
            }
            else
            {
                MessageBox.Show(result.ErrorText);
            }
        }

        private void deleteTable(object sender, RoutedEventArgs e)
        {
            showAcceptDropDBDialog("DROP TABLE `" + currentTable + "`");
        }


        private void changeTable(object sender, RoutedEventArgs e)
        {
            if(openedTableGrid.SelectedItem == null)
            {
                somethingAintSelected("Строка не выбрана", "Сначала выберите строку, которую хотите изменить");
            }
            else
            {
                string[] rows = new string[openedTableGrid.Columns.Count];
                for (int i = 0; i < openedTableGrid.Columns.Count; i++)
                {
                    rows[i] = ((DataRowView)openedTableGrid.SelectedItem).Row[i].ToString();
                }
            }
        }

        private void somethingAintSelected(string caption, string text)
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "OK",
                ColorScheme = MetroDialogOptions.ColorScheme
            };

            this.ShowMessageAsync(caption, text, MessageDialogStyle.Affirmative, mySettings);
        }
    }
}
