using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using MahApps.Metro.Controls.Dialogs;
using System.Text.RegularExpressions;
using System.Threading;

namespace TheGUIAndSQLApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        DataColumn[] temp;
        bool dataBaseSelected = false;
        bool tableSelected = false;
        bool querySelected = false;

        private string currentTable = "";

        private string userID;
        private string password;
        private string host = "localhost";
        private string currentDataBase;

        private string connectStrWithDb;
        private string connectStr;

        public MainWindow()
        {
            InitializeComponent();
            table.Items.Clear();
            dataBases.Items.Clear();
            openedTableGrid.Items.Clear();
            describeTable.Items.Clear();
            sqlSelectGrid.Items.Clear();
        }


        private void CloseCustomDialog(object sender, RoutedEventArgs e)
        {
            CloseCustomDialogRealize(1);
        }

        private void CloseCustomDialog1(object sender, RoutedEventArgs e)
        {
            CloseCustomDialogRealize(2);
        }

        private void CloseCustomDialog2(object sender, RoutedEventArgs e)
        {
            CloseCustomDialogRealize(3);
        }

        private async void CloseCustomDialogRealize(int id)
        {
            string resource = "";
            switch (id)
            {
                case 1: resource = "CustomCloseDialogTest"; break;
                case 2: resource = "CustomCloseTDialogTest"; break;
                case 3: resource = "CustomSqlDialog"; break;
                default: MessageBox.Show("Ошибка"); break;
            }
            var dialog = (BaseMetroDialog)this.Resources[resource];
            await this.HideMetroDialogAsync(dialog);
        }


        private void UsingSql(object sender, RoutedEventArgs e)
        {
            if (SQLQuery.Text.StartsWith("SELECT"))
            {
                MySqlLib.MySqlData.MySqlExecuteData.MyResultData result = new MySqlLib.MySqlData.MySqlExecuteData.MyResultData();
                result = MySqlLib.MySqlData.MySqlExecuteData.SqlReturnDataset(SQLQuery.Text, connectStrWithDb);
                querySelected = true;
                if (result.HasError == false)
                {
                    sqlSelectGrid.ItemsSource = result.ResultData.DefaultView;
                    tabControl.SelectedIndex = 5;
                }
                else
                {
                    MessageBox.Show(result.ErrorText);
                }
            }
            else
            {
                MySqlLib.MySqlDataC.MySqlExecute.MyResult result = new MySqlLib.MySqlDataC.MySqlExecute.MyResult();
                result = MySqlLib.MySqlDataC.MySqlExecute.SqlNoneQuery(SQLQuery.Text, connectStrWithDb);
                if (result.HasError == false)
                {
                    MessageBox.Show(result.ResultText);
                }
                else
                {
                    MessageBox.Show(result.ErrorText);
                }
            }
        }

        private void Auth_Click(object sender, RoutedEventArgs e)
        {
            userID = loginBox.Text;
            password = passwordBox.Password;
            AuthGrid.Visibility = Visibility.Collapsed;
            loginBox.Text = null;
            passwordBox.Password = null;
            MainGrid.Visibility = Visibility.Visible;
            connectStr = @"Data Source = " + host + "; User Id = " + userID + "; Password =" + password;
            SelectDataBases();
        }

        public void SelectDataBases()
        {
            MySqlLib.MySqlData.MySqlExecuteData.MyResultData result = new MySqlLib.MySqlData.MySqlExecuteData.MyResultData();
            result = MySqlLib.MySqlData.MySqlExecuteData.SqlReturnDataset("SHOW DATABASES", connectStr);
            if (result.HasError == false)
            {
                dataBases.ItemsSource = result.ResultData.DefaultView;
                dataBases.UpdateLayout();
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
                currentDataBase = ((DataRowView)dataBases.SelectedItems[0]).Row["DATABASE"].ToString();
                connectStrWithDb = @"Database = " + currentDataBase + "; " + connectStr;
                MySqlLib.MySqlData.MySqlExecuteData.MyResultData result = new MySqlLib.MySqlData.MySqlExecuteData.MyResultData();
                result = MySqlLib.MySqlData.MySqlExecuteData.SqlReturnDataset("SHOW TABLES", connectStrWithDb);
                if (result.HasError == false)
                {
                    dataBaseSelected = true;
                    tabControl.SelectedIndex = 1;
                    tabControl.SelectedItem = tablesTab;
                    tablesTab.IsSelected = true;
                    RemovePunctiatonFromColumns(result, table);
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
            if (querySelected == false && tabControl.SelectedIndex == 5)
            {
                showChooseDialog(3);
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
                case 5:
                    statusBar.Visibility = Visibility.Visible;
                    statusBarType.Content = "Результат запроса";
                    statusBarSelected.Content = currentTable;
                    break;
                default: statusBar.Visibility = Visibility.Collapsed; break;
            }
        }
        private async void showChooseDialog(int type)
        {
            EventHandler<DialogStateChangedEventArgs> dialogManagerOnDialogOpened = null;
            dialogManagerOnDialogOpened = (o, args) =>
            {
                DialogManager.DialogOpened -= dialogManagerOnDialogOpened;
            };
            DialogManager.DialogOpened += dialogManagerOnDialogOpened;

            EventHandler<DialogStateChangedEventArgs> dialogManagerOnDialogClosed = null;
            dialogManagerOnDialogClosed = (o, args) =>
            {
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
                case 3:
                    dialog = (BaseMetroDialog)this.Resources["CustomSqlDialog"];
                    await this.ShowMetroDialogAsync(dialog);
                    await dialog.WaitUntilUnloadedAsync();
                    tabControl.SelectedIndex = 4;
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

            if (result == MessageDialogResult.Affirmative)
            {
                if (queryStr.Contains("DROP TABLE `"))
                {
                    MySqlLib.MySqlData.MySqlExecute.SqlNoneQuery(queryStr, connectStrWithDb);
                    if (tabControl.SelectedIndex == 2 || queryStr.Contains(currentTable))
                    {
                        tabControl.SelectedIndex = 1;
                        tableSelected = false;
                    }
                    updateCurrentTable();
                }
                else if (queryStr.Contains("TRUNCATE `"))
                {
                    MySqlLib.MySqlData.MySqlExecute.SqlNoneQuery(queryStr, connectStrWithDb);
                    updateTable(null, null);
                }
                else
                {
                    MySqlLib.MySqlData.MySqlExecute.SqlNoneQuery(queryStr, connectStr);
                    if (queryStr.Contains(currentDataBase))
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
            AddDataBase addDataBase = new AddDataBase(connectStr);
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
                currentTable = ((DataRowView)table.SelectedItems[0]).Row[0].ToString();
                MySqlLib.MySqlData.MySqlExecuteData.MyResultData result = new MySqlLib.MySqlData.MySqlExecuteData.MyResultData();
                result = MySqlLib.MySqlData.MySqlExecuteData.SqlReturnDataset("SELECT * FROM " + currentTable, connectStrWithDb);
                if (result.HasError == false)
                {
                    tableSelected = true;
                    tabControl.SelectedIndex = 2;
                    tabControl.SelectedItem = openedTable;
                    openedTable.IsSelected = true;
                    RemovePunctiatonFromColumns(result, openedTableGrid);
                    DescribeTableFoo();
                }
                else
                {
                    MessageBox.Show(result.ErrorText);
                }
            }
        }

        private void RemovePunctiatonFromColumns(MySqlLib.MySqlData.MySqlExecuteData.MyResultData result, DataGrid grid)
        {
            temp = new DataColumn[result.ResultData.Columns.Count];
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = new DataColumn();
                temp[i].ColumnName = result.ResultData.Columns[i].ToString();
            }
            string pattern = @"[\p{P}]";
            for (int i = 0; i < result.ResultData.Columns.Count; i++)
            {
                result.ResultData.Columns[i].ColumnName =
                     Regex.Replace(result.ResultData.Columns[i].ColumnName, pattern, "");
            }
            grid.ItemsSource = result.ResultData.DefaultView;
            grid.UpdateLayout();
            for (int i = 0; i < temp.Length - 1; i++)
                grid.Columns[i].Header = temp[i].ColumnName;
        }

        private void Table_Create(object sender, RoutedEventArgs e)
        {
            AddTable addTable = new AddTable(currentDataBase, connectStrWithDb);
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
            MySqlLib.MySqlData.MySqlExecuteData.MyResultData result = new MySqlLib.MySqlData.MySqlExecuteData.MyResultData();
            result = MySqlLib.MySqlData.MySqlExecuteData.SqlReturnDataset("SHOW TABLES", connectStrWithDb);
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
            DescribeTableFoo();
            tabControl.SelectedIndex = 3;
        }

        private void DescribeTableFoo()
        {
            if (tableSelected == false)
            {
                showChooseDialog(2);
            }
            else
            {
                MySqlLib.MySqlData.MySqlExecuteData.MyResultData result = new MySqlLib.MySqlData.MySqlExecuteData.MyResultData();
                result = MySqlLib.MySqlData.MySqlExecuteData.SqlReturnDataset("DESCRIBE `" + currentTable + "`", connectStrWithDb);
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

        public void updateTable(object sender, RoutedEventArgs e)
        {
            MySqlLib.MySqlData.MySqlExecuteData.MyResultData result = new MySqlLib.MySqlData.MySqlExecuteData.MyResultData();
            result = MySqlLib.MySqlData.MySqlExecuteData.SqlReturnDataset("SELECT * FROM " + currentTable, connectStrWithDb);
            if (result.HasError == false)
            {
                RemovePunctiatonFromColumns(result, openedTableGrid);
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
            if (openedTableGrid.SelectedItem == null)
            {
                somethingAintSelected("Строка не выбрана", "Сначала выберите строку, которую хотите изменить");
            }
            else
            {
                int count = openedTableGrid.Columns.Count;
                string[] rows = new string[count];
                string[] columns = new string[count];
                string[] types = new string[count];
                for (int i = 0; i < count; i++)
                {
                    rows[i] = ((DataRowView)openedTableGrid.SelectedItem).Row[i].ToString();
                    columns[i] = openedTableGrid.Columns[i].Header.ToString();
                    types[i] = ((DataRowView)describeTable.Items[i]).Row[1].ToString();
                }
                ChangeColumn change = new ChangeColumn(currentDataBase, connectStr, currentTable, columns, types, rows);
                change.Owner = this;
                this.IsEnabled = false;
                change.Show();
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

        private void FillTable(object sender, RoutedEventArgs e)
        {
            FillOrSearch(false);
        }

        private void SearchInTable(object sender, RoutedEventArgs e)
        {
            FillOrSearch(true);
        }

        private void FillOrSearch(bool FoS)
        {
            int count = openedTableGrid.Columns.Count;
            string[] columns = new string[count];
            string[] types = new string[count];
            for (int i = 0; i < count; i++)
            {
                columns[i] = openedTableGrid.Columns[i].Header.ToString();
                types[i] = ((DataRowView)describeTable.Items[i]).Row[1].ToString();
            }
            FillOrSearch fill = new FillOrSearch(currentDataBase, connectStr, connectStrWithDb, currentTable, columns, types, openedTableGrid, FoS);
            fill.Owner = this;
            this.IsEnabled = false;
            fill.Show();
        }

        private async void ShowConnectDialog(object sender, RoutedEventArgs e)
        {
            var result = await this.ShowInputAsync("Хост (Data Source)", "Введите хост, к которому нужно подключиться");

            if (result == null)
                return;

            host = result;

            connectStr = @"Data Source = " + host + "; User Id = " + userID + "; Password =" + password;
            connectStrWithDb = @"Database = " + currentDataBase + "; " + connectStr;
        }

        private void ChangeUser(object sender, RoutedEventArgs e)
        {
            table.ItemsSource = null;
            dataBases.ItemsSource = null;
            openedTableGrid.ItemsSource = null;
            describeTable.ItemsSource = null;
            sqlSelectGrid.ItemsSource = null;
            dataBaseSelected = false;
            tableSelected = false;
            querySelected = false;

            AuthGrid.Visibility = Visibility.Visible;
            MainGrid.Visibility = Visibility.Collapsed;
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void About(object sender, RoutedEventArgs e)
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Закрыть",
                MaximumBodyHeight = 50,
                ColorScheme = MetroDialogOptions.ColorScheme

            };

            MessageDialogResult result = await this.ShowMessageAsync("Дипломная программа", "" + string.Join(Environment.NewLine,
                "Автор: Смочилин Михаил, МП-41"),
                MessageDialogStyle.Affirmative, mySettings);
        }
    }
}
