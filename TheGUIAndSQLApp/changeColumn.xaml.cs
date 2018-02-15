
using System;
using System.Windows;
using System.Windows.Controls;

namespace TheGUIAndSQLApp
{
    /// <summary>
    /// Логика взаимодействия для changeColumn.xaml
    /// </summary>
    public partial class ChangeColumn
    {
        string[] columns;
        string[] types;
        string[] rows;
        string table;
        string dataBase;
        string connectString;
        public ChangeColumn(string db, string connStr, string t, string[] c, string[] ts, string[] r)
        {
            InitializeComponent();
            dataBase = db;
            table = t;
            columns = c;
            types = ts;
            rows = r;
            connectString = connStr;

            double top = columnsName.Margin.Top;
            double left1 = columnsName.Margin.Left;
            double left2 = typeOfData.Margin.Left;
            double left3 = columnsValue.Margin.Left;
            double left4 = columnsNewValue.Margin.Left;
            Button accept = new Button();
            accept.Content = "изменить";
            accept.HorizontalAlignment = HorizontalAlignment.Center;
            accept.VerticalAlignment = VerticalAlignment.Top;
            accept.Margin = new Thickness(0, 150 + ((columns.Length - 1) * 50), 0, 50);
            accept.Click += new RoutedEventHandler(acceptChanges);

            for (int i = 0; i < c.Length; i++)
            {
                top += 50.0;

                Label column = new Label();
                Label type = new Label();
                TextBox oldValue = new TextBox();
                TextBox newValue = new TextBox();

                column.FontSize = type.FontSize = 16;
                column.Content = c[i];
                type.Content = ts[i];
                column.Margin = new Thickness(left1, top, 0, 0);
                type.Margin = new Thickness(left2, top, 0, 0);

                oldValue.Height = newValue.Height = 31;
                oldValue.Width = newValue.Width = 143;
                oldValue.TextWrapping = newValue.TextWrapping = TextWrapping.Wrap;
                oldValue.VerticalAlignment = newValue.VerticalAlignment = VerticalAlignment.Top;
                oldValue.HorizontalAlignment = newValue.HorizontalAlignment = HorizontalAlignment.Left;

                oldValue.Margin = new Thickness(left3, top, 0, 0);
                newValue.Margin = new Thickness(left4, top, 0, 0);

                oldValue.IsReadOnly = true;
                oldValue.Text = r[i];

                MainGrid.Height += 50;
                MainGrid.Children.Add(column);
                MainGrid.Children.Add(type);
                MainGrid.Children.Add(oldValue);
                MainGrid.Children.Add(newValue);
            }
            MainGrid.Children.Add(accept);
            MainGrid.Height += 50;
        }

        private void acceptChanges(object sender, EventArgs e)
        {
            string SQLQuery = "UPDATE `" + dataBase + "`.`" + 
                table + "`" + " SET ";
            int index = 7;
            for(int i = 0; i < columns.Length; i++)
            {
                TextBox tb = (TextBox)MainGrid.Children[index];
                if (tb.Text != null && tb.Text != "")
                {
                    SQLQuery += "`" + table + "`" + "." + "`" + columns[i] + "` = '"
                        + tb.Text + "', ";
                }
                index += 4;
            }
            SQLQuery += "replaceme";
            if (!SQLQuery.Contains("'"))
            {
                this.Close();
            }
            else
            {
                SQLQuery = SQLQuery.Replace(", replaceme", " WHERE ");
                SQLQuery += "`" + table + "`.`" + columns[0] + "` = '" +
                        rows[0] + "' ";
                for (int i = 1; i < columns.Length; i++)
                {
                    if (types[i] == "date" || types[i] == "datetime") continue;
                    SQLQuery += "AND `" + table + "`.`" + columns[i] + "` = '" +
                        rows[i] + "' ";
                }
                SQLQuery += "LIMIT 1";
            }
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

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.IsEnabled = true;
            MainWindow wnd = (MainWindow)App.Current.MainWindow;
            wnd.updateTable(null, null);
            this.Owner.Activate();
        }
    }
}
