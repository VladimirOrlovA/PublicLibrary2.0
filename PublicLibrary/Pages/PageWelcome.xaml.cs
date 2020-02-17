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
    /// Логика взаимодействия для PageWelcome.xaml
    /// </summary>
    public partial class PageWelcome : Page
    {
        DbContext dbContext = new DbContext(MainWindow.path);
        Book _book = null;
        public PageWelcome(Book book)
        {
            InitializeComponent();
            
            if (book != null)
            {
                _book = book;
                AddBtn.Content = "Редактировать";
                AddBtn.Click += EditBook_Click;
                AddBtn.Click -= AddBtn_Click;

                TBXname.Text = book.Name;
                TBXedition.Text = book.Edition;
                DpDate.SelectedDate = book.IssueDate;
                // book.Author = ((ComboBoxItem)CBXauthor.SelectedItem).Content.ToString();
                // book.Genre = ((ComboBoxItem)CBXgenre.SelectedItem).Content.ToString();

                rbAvaliable.IsChecked = book.IsAvailible;
                chAfter18.IsChecked = book.IsEighteenPlus;
                chOld.IsChecked = book.IsRaritet;
                chLasBook.IsChecked = book.IsTheLastestPublisher;
            }
            else
            {
                AddBtn.Content = "Добавить книгу";
                AddBtn.Click -= EditBook_Click;
                AddBtn.Click += AddBtn_Click;
            }
        }

        private void EditBook_Click(object sender, RoutedEventArgs e)
        {
            _book.Name = TBXname.Text;
            _book.Edition = TBXedition.Text;
            _book.IssueDate = DpDate.SelectedDate == null ? DateTime.Now : (DateTime)DpDate.SelectedDate;
            _book.Author = ((ComboBoxItem)CBXauthor.SelectedItem).Content.ToString();
            _book.Genre = ((ComboBoxItem)CBXgenre.SelectedItem).Content.ToString();
            _book.IsAvailible = (bool)rbAvaliable.IsChecked;
            _book.IsEighteenPlus = (bool)chAfter18.IsChecked;
            _book.IsRaritet = (bool)chOld.IsChecked;
            _book.IsTheLastestPublisher = (bool)chLasBook.IsChecked;

            if (dbContext.EditBook(_book))
            {
                MessageBox.Show("Книга изменена успешно!");
                MainWindow._MainFrame.Navigate(new PageBooks());
            }
            else
            {
                MessageBox.Show("Возникли ошибки при изменении книги!");
            }

        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Book book = new Book();
            book.Name = TBXname.Text;
            book.Edition = TBXedition.Text;
            book.IssueDate = DpDate.SelectedDate == null ? DateTime.Now : (DateTime)DpDate.SelectedDate;
            book.Author = ((ComboBoxItem)CBXauthor.SelectedItem).Content.ToString();
            book.Genre = ((ComboBoxItem)CBXgenre.SelectedItem).Content.ToString();
            book.IsAvailible = (bool)rbAvaliable.IsChecked;
            book.IsEighteenPlus = (bool)chAfter18.IsChecked;
            book.IsRaritet = (bool)chOld.IsChecked;
            book.IsTheLastestPublisher = (bool)chLasBook.IsChecked;
            book.AddedBy = MainWindow.user.Id;
            book.AddedTime = DateTime.Now;

           
            if (dbContext.AddBook(book))
            {
                MessageBox.Show("Книга добавлена успешно!");
                MainWindow._MainFrame.Navigate(new PageBooks());

            }
            else
            {
                MessageBox.Show("Возникли ошибки при добавлении книги!");
            }



        }
    }
}
