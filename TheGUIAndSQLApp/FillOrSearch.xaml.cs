using System;
using System.Windows;
using System.Windows.Controls;

namespace TheGUIAndSQLApp
{
    /// <summary>
    /// Логика взаимодействия для FillOrTable.xaml
    /// </summary>
    public partial class FillOrSearch
    {
        bool isWindowForSearch;
        string[] columns;
        string[] types;
        string table;
        string dataBase;
        string connectString;
        string connectStringWithDb;
        DataGrid openedGrid;
        public FillOrSearch(string db, string cS, string cSWD, string t, string[] c, string[] ts, DataGrid oG, bool iWFS)
        {
            InitializeComponent();
            dataBase = db;
            table = t;
            columns = c;
            types = ts;
            isWindowForSearch = iWFS;
            openedGrid = oG;
            connectString = cS;
            connectStringWithDb = cSWD;

            double top = columnsName.Margin.Top;
            double left1 = columnsName.Margin.Left;
            double left2 = typeOfData.Margin.Left;
            double left3 = columnsValue.Margin.Left;
            Button accept = new Button();

            accept.Content = iWFS == true ? "Искать" : "Добавить";
            accept.HorizontalAlignment = HorizontalAlignment.Center;
            accept.VerticalAlignment = VerticalAlignment.Top;
            accept.Margin = new Thickness(0, 150 + ((columns.Length - 1) * 50), 0, 50);
            accept.Click += new RoutedEventHandler(AcceptChanges);

            for (int i = 0; i < c.Length; i++)
            {
                top += 50.0;

                Label column = new Label();
                Label type = new Label();
                TextBox Value = new TextBox();

                column.FontSize = type.FontSize = 16;
                column.Content = c[i];
                type.Content = ts[i];
                column.Margin = new Thickness(left1, top, 0, 0);
                type.Margin = new Thickness(left2, top, 0, 0);

                Value.Height = 31;
                Value.Width = 143;
                Value.TextWrapping = TextWrapping.Wrap;
                Value.VerticalAlignment = VerticalAlignment.Top;
                Value.HorizontalAlignment = HorizontalAlignment.Left;
                Value.Margin = new Thickness(left3, top, 0, 0);

                MainGrid.Height += 50;
                MainGrid.Children.Add(column);
                MainGrid.Children.Add(type);
                MainGrid.Children.Add(Value);
            }
            MainGrid.Children.Add(accept);
            MainGrid.Height += 50;
        }

        private void AcceptChanges(object sender, EventArgs e)
        {
            string SQLQuery;
                
            if (isWindowForSearch == false)
            {
                SQLQuery = "INSERT INTO `" + dataBase + "`.`" +
                    table + "`" + " (";
                for (int i = 0; i < columns.Length; i++)
                {
                    SQLQuery += "`" + columns[i] + "`, ";
                }
                SQLQuery += "replaceme";
                SQLQuery = SQLQuery.Replace(", replaceme", ") ");
                SQLQuery += "VALUES(";
                int index = 5;
                for (int i = 0; i < columns.Length; i++)
                {
                    TextBox tb = (TextBox)MainGrid.Children[index];
                    SQLQuery += "'" + tb.Text + "', ";
                    index += 3;
                }
                SQLQuery += "replaceme";
                SQLQuery = SQLQuery.Replace(", replaceme", ")");
                MySqlLib.MySqlData.MySqlExecute.MyResult result =
                    MySqlLib.MySqlData.MySqlExecute.SqlNoneQuery(SQLQuery, connectString);
                if (result.HasError == false)
                {
                    this.Close();
                }
                else
                {
                    MessageBox.Show(result.ErrorText);
                }
            }
            else
            {
                SQLQuery = "SELECT * FROM `" + table + "`" + " WHERE ";
                int index = 5;
                for (int i = 0; i < columns.Length; i++)
                {
                    TextBox tb = (TextBox)MainGrid.Children[index];
                    if(tb.Text != null && tb.Text != "")
                    {
                        SQLQuery += "`" + columns[i] + "` = " + "'" + tb.Text + "' AND ";
                        index += 3;
                    }
                }
                SQLQuery += "replaceme";
                SQLQuery = SQLQuery.Replace("AND replaceme", " LIMIT 0, 30");

                MySqlLib.MySqlData.MySqlExecuteData.MyResultData result = new MySqlLib.MySqlData.MySqlExecuteData.MyResultData();
                result = MySqlLib.MySqlData.MySqlExecuteData.SqlReturnDataset(SQLQuery, connectStringWithDb);
                if (result.HasError == false)
                {
                    openedGrid.ItemsSource = result.ResultData.DefaultView;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(result.ErrorText);
                }
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.Activate();
            this.Owner.IsEnabled = true;
            if (isWindowForSearch == false)
            {
                MainWindow wnd = (MainWindow)App.Current.MainWindow;
                wnd.updateTable(null, null);
            }
        }
    }
}
