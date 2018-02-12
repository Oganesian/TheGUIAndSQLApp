
using System.Windows.Controls;

namespace TheGUIAndSQLApp
{
    /// <summary>
    /// Логика взаимодействия для changeColumn.xaml
    /// </summary>
    public partial class changeColumn
    {
        public changeColumn()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            double left1 = columnsName.Margin.Left;
            double left2 = typeOfData.Margin.Left;
            double left3 = columnsValue.Margin.Left;
            double left4 = columnsNewValue.Margin.Left;
            double top = columnsName.Margin.Top;
            top += 50.0;

            Label label1 = new Label();
            Label label2 = new Label();
            TextBox textBox3 = new TextBox();
            TextBox textBox4 = new TextBox();

            label1.FontSize = label2.FontSize = 16;
            label1.Content = "ASDDD";
            label2.Content = "DDDSS";
            label1.Margin = new System.Windows.Thickness(left1, top, 0, 0);
            label2.Margin = new System.Windows.Thickness(left2, top, 0, 0);

            textBox3.Height = textBox4.Height = 31;
            textBox3.Width = textBox4.Width = 143;
            textBox3.TextWrapping = textBox4.TextWrapping = System.Windows.TextWrapping.Wrap;
            textBox3.VerticalAlignment = textBox4.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            textBox3.HorizontalAlignment = textBox4.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

            textBox3.Margin = new System.Windows.Thickness(left3, top, 0, 0);
            textBox4.Margin = new System.Windows.Thickness(left4, top, 0, 0);

            MainGrid.Height += 50;
            MainGrid.Children.Add(label1);
            MainGrid.Children.Add(label2);
            MainGrid.Children.Add(textBox3);
            MainGrid.Children.Add(textBox4);
            //<TextBox x:Name="columnsValue" Height="31" Margin="370,63,0,0" TextWrapping="Wrap" Text="TextBox" VerticalAlignment="Top" HorizontalAlignment="Left" Width="143"/>
        }
    }
}
