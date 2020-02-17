using PublicLibrary.lip;
using PublicLibrary.Pages;
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
using PublicLibrary.NCALayer;

namespace PublicLibrary
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string path = @"C:\Temp\MyData.db";
        public static User user = null;
        public static Frame _MainFrame = null;
        public static Menu _MainMenu;

        private DbContext db = new DbContext(path);

        public MainWindow()
        {
            InitializeComponent();
            _MainFrame = MainFrame;
            _MainMenu = MainMenu;

            MainFrame.Navigate(new PageAuth());

            PublicLibrary.NCALayer.NCALayer layer = 
                new PublicLibrary.NCALayer.NCALayer();

            layer.getActiveTokens();
        }

    }
}
