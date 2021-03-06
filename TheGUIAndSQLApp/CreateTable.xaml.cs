﻿using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace TheGUIAndSQLApp
{
    /// <summary>
    /// Логика взаимодействия для CreateTable.xaml
    /// </summary>
    public partial class CreateTable
    {
        private string SQLQuery = "";
        private string currentDB;
        string tableName;
        int columnsNumber;
        int currentColumn = 0;
        List<string> columns = new List<string>();
        private string connectStr;

        public CreateTable(string tN, string connStr, string cDB, int cN)
        {
            InitializeComponent();
            tableName = tN;
            columnsNumber = cN;
            currentDB = cDB;
            connectStr = connStr;
            currentColumnLabel.Content = "Формирование столбца " + ++currentColumn + " из " + columnsNumber;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
                if (tableNameTB.Text == null || tableNameTB.Text == "" ||
                    typeCB.SelectedItem == null)
                {
                    MessageBox.Show("Заполните обязательные поля", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    string name = tableNameTB.Text;
                    string type = typeCB.SelectedItem.ToString().Replace("System.Windows.Controls.ComboBoxItem: ", "");
                    int length;
                    int.TryParse(lengthTB.Text, out length);
                    string defaultValue = "NO";
                    if (defaultValueCB.SelectedIndex == 1) defaultValue = null;
                    bool isNull = isNullCB.SelectedIndex == 0 ? false : true;
                    bool isAutoIncrementEnabled = isAutoIncrementEnabledCB.SelectedIndex == 0 ? true : false;
                    string index = null;
                    if (indexCB.SelectedItem != null)
                    {
                        index = indexCB.SelectedItem.ToString().Replace("System.Windows.Controls.ComboBoxItem: ", "");
                    }
                    ClearBoxes();
                    currentColumn++;
                
                    Column temp = new Column(name, type, length, defaultValue, isNull, isAutoIncrementEnabled, index);

                    columns.Add(temp.ToString());

                    if(currentColumn > columnsNumber)
                    {
                        columnsFormatted();
                    }
                    else
                    {
                        currentColumnLabel.Content = "Формирование столбца " + currentColumn + " из " + columnsNumber;
                    }
            }
        }
        private void columnsFormatted()
        {
            SQLQuery = "CREATE TABLE IF NOT EXISTS " + tableName + "(";
            for(int i = 0; i < columns.Count; i++)
            {
                SQLQuery += columns[i];
                if (i+1 != columns.Count) SQLQuery += ", ";
            }
            SQLQuery += ")";
            showAcceptCreateTableDialog();
        }
        private async void showAcceptCreateTableDialog()
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Да",
                NegativeButtonText = "Нет",
                MaximumBodyHeight = 100,
                ColorScheme = MetroDialogOptions.ColorScheme
                
        };

            MessageDialogResult result = await this.ShowMessageAsync("SQL-запрос сформирован. Хотите выполнить его?", "" + string.Join(Environment.NewLine, SQLQuery),
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if (result == MessageDialogResult.Affirmative)
            {
                MainWindow wnd = (MainWindow)App.Current.MainWindow;
                MySqlLib.MySqlData.MySqlExecute.MyResult queryResult =
                MySqlLib.MySqlData.MySqlExecute.SqlNoneQuery(SQLQuery, connectStr);
                if (queryResult.HasError == false)
                {
                    MessageBox.Show("Запрос выполнился без ошибок");
                    wnd.updateCurrentTable();
                    wnd.Activate();
                    this.Close();
                }
                else
                {
                    MessageBox.Show(queryResult.ErrorText, "Возникла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    wnd.Activate();
                    this.Close();
                }
            }
        }
        private void ClearBoxes()
        {
            tableNameTB.Text = null;
            typeCB.SelectedItem = null;
            lengthTB.Text = null;
            defaultValueCB.SelectedItem = null;
            isNullCB.SelectedIndex = 0;
            isAutoIncrementEnabledCB.SelectedItem = null;
            indexCB.SelectedItem = null;
        }
        private class Column
        {
            string name;
            string type;
            int length;
            string defaultValue;
            bool isNull;
            bool isAutoIncrementEnabled;
            string index;

            public Column(string n, string t, int l, string dV, bool iN, bool iAIE, string i)
            {
                name = n;
                type = t;
                length = l;
                defaultValue = dV;
                isNull = iN;
                isAutoIncrementEnabled = iAIE;
                index = i;
            }

            public override string ToString()
            {
                bool doNeedLength = true;
                string[] dontNeedLength = { "DATE", "YEAR", "TINYBLOB", "MEDIUMBLOB", "LONGBLOB",
                                            "TINYTEXT", "MEDIUMTEXT", "LONGTEXT"};
                string returnStr = name + " ";
                foreach(string s in dontNeedLength)
                {
                    if(s == type)
                    {
                        doNeedLength = false;
                        continue;
                    }
                }
                if(doNeedLength == true && length != 0 && !(type == "VARCHAR" || type == "VARBINARY"))
                {
                    returnStr += type + "(" + length.ToString() + ") ";
                }
                else if(doNeedLength == false)
                {
                    returnStr += type + " ";
                }
                else
                {
                    returnStr += type + "(64) ";
                }
                if (defaultValue == null) returnStr += "DEFAULT NULL ";
                if (isNull == false) returnStr += "NOT NULL ";
                if (index != null) returnStr += index + " ";
                if (isAutoIncrementEnabled == true) returnStr += "AUTO_INCREMENT";

                return returnStr;
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }
    }
}
