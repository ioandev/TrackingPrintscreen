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
using System.Windows.Shapes;

namespace TrackPrintScreen
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        Options options;
        public OptionsWindow(Options options)
        {
            this.options = options;
            InitializeComponent();
        }

        public Options getOptions()
        {
            return options;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SavePath();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SavePathTextBox.Text = options.FolderPath;
        }

        private void SavePath()
        {
            if(!System.IO.Directory.Exists(SavePathTextBox.Text))
            {
                MessageBox.Show("Directory does not exist!");
            }
            else
            {
                options.FolderPath = SavePathTextBox.Text;
                MessageBox.Show("Saved successfully. Close the window for the changes to occur.");
            }
        }

        private void BrowseSavePathButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Feature not working for now.");
        }
    }
}
