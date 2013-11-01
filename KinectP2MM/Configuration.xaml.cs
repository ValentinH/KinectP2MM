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

namespace KinectP2MM
{
    /// Logique d'interaction pour Configuration.xaml
    public partial class Configuration : Window
    {
        private MainWindow parentWindow;

        public Configuration(MainWindow parentWindow)
        {
            InitializeComponent();

            this.parentWindow = parentWindow;
        }

        private void openFileChooser()
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.Filter = "Images (*.png;*.jpg)|*.png;*.jpg";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                String filename = dlg.FileName;
            }
        }

        private void RightHandButton_Click(object sender, RoutedEventArgs e)
        {
            openFileChooser();
        }

        private void LeftHandButton_Click(object sender, RoutedEventArgs e)
        {
            openFileChooser();
        }

        private void RightHandGripButton_Click(object sender, RoutedEventArgs e)
        {
            openFileChooser();
        }

        private void LeftHandGripButton_Click(object sender, RoutedEventArgs e)
        {
            openFileChooser();
        }
    }
}
