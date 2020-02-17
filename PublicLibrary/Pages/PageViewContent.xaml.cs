using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace PublicLibrary.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageViewContent.xaml
    /// </summary>
    public partial class PageViewContent : Page
    {
        public PageViewContent()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opentFile = new OpenFileDialog();
            opentFile.Filter =
                "RichText Files (*.rtf)|*.rtf|All Files (*.*)|*.*";

            if (opentFile.ShowDialog() == true)
            {
                TextRange docTextRange = new TextRange(
                    myRichTextBox.Document.ContentStart,
                    myRichTextBox.Document.ContentEnd);

                using (FileStream fs = File.Open(opentFile.FileName, FileMode.Open))
                {
                    docTextRange.Load(fs, DataFormats.Rtf);
                }

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SaveFileDialog opentFile = new SaveFileDialog();
            opentFile.Filter =
                "RichText Files (*.rtf)|*.rtf|All Files (*.*)|*.*";

            if (opentFile.ShowDialog() == true)
            {
                TextRange docTextRange = new TextRange(myRichTextBox.Document.ContentStart,
                    myRichTextBox.Document.ContentEnd);

                using (FileStream fs = File.Create(opentFile.FileName))
                {
                    docTextRange.Save(fs, DataFormats.Rtf);
                }

            }
        }
    }
}
