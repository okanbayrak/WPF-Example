using IsarEntities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IsarAerospace
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string DEFAULT_EXT = ".csv";
        private const string EXTENSION_FILTER = "CSV files(*.csv)|*.csv";
        private const char SEPERATOR = ';';
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = DEFAULT_EXT,
                Filter = EXTENSION_FILTER
            };

            bool? selectedFile = dialog.ShowDialog();
            if (selectedFile == true)
            {
                FillGrid(dialog.FileName);
            }
        }

        private void FillGrid(string fileName)
        {
            Books.Visibility = Visibility.Visible;
            BookBinding.ItemsSource = Enum.GetValues(typeof(Binding));
            List<Book> books = new List<Book>();
            foreach (var line in File.ReadLines(fileName).Skip(1))
            {
                string[] data = line.Split(SEPERATOR);
                books.Add(new Book(data[0], data[1], data[2], data[3], data[4], data[5], data[6]));
            }
            PickPriceTextColor(books);
            Books.ItemsSource = books;
        }

        private void PickPriceTextColor(List<Book> books)
        {
            decimal highestPrice = books.Max(b => b.Price);
            decimal lowestPrice = books.Min(b => b.Price);
            decimal priceRange = highestPrice - lowestPrice;
            foreach (var book in books)
            {
                int price = Convert.ToInt32((book.Price - lowestPrice) * 255 / priceRange);
                byte r = Convert.ToByte(price);
                byte g = Convert.ToByte(255 - price);
                book.PriceColor = new SolidColorBrush(Color.FromRgb(r, g, 0));
            }
        }

        private void Show_Description_Click(object sender, RoutedEventArgs e)
        {
            var book = ((Button)sender).DataContext as Book;
            MessageBox.Show(book.Description);
        }

        private void Show_InStock_Click(object sender, RoutedEventArgs e)
        {
            List<Book> books = (List<Book>)Books.ItemsSource;
            books = books.Where(b => b.InStock).ToList();
            Books.ItemsSource = books;
        }
    }
}
