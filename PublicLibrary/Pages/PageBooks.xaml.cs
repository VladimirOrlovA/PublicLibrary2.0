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
using PublicLibrary.lip;

namespace PublicLibrary.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageBooks.xaml
    /// </summary>
    public partial class PageBooks : Page
    {
        private DbContext dbContext = null;
        public PageBooks()
        {
            InitializeComponent();

            dbContext = new DbContext(MainWindow.path);

            lwBooks.ItemsSource = dbContext.GetBooks();
        }

        private void LbBooks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            StackPanel sp = ((ListBox)sender).SelectedItem as StackPanel;
        
            int id = Convert.ToInt32(((Label)sp.Children[0]).Content);

            Book book = dbContext.GetBookbyId(id);
            if(book!=null)
            {
                MainWindow._MainFrame.Navigate(new PageWelcome(book));
            }
            else
            {
                MessageBox.Show("OOps");
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainWindow._MainFrame.Navigate(new PageWelcome((Book)lwBooks.SelectedItem));
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow._MainFrame.Navigate(new PageViewContent());
        }
    }
}
