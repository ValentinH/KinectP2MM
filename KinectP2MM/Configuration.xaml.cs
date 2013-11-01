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

        private String openFileChooser()
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.InitialDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            dlg.Filter = "Images (*.png;*.jpg)|*.png;*.jpg";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                String filename = dlg.FileName;
                return filename;
            }
            else
                return null;
        }

        private void RightHandButton_Click(object sender, RoutedEventArgs e)
        {
            String filePath = openFileChooser();
            if (filePath != null)
            {
                RightHandTextBox.Text = filePath;
            }
        }

        private void LeftHandButton_Click(object sender, RoutedEventArgs e)
        {
            String filePath = openFileChooser();
            if (filePath != null)
            {
                LeftHandTextBox.Text = filePath;
            }
        }

        private void RightHandGripButton_Click(object sender, RoutedEventArgs e)
        {
            String filePath = openFileChooser();
            if (filePath != null)
            {
                RightHandGripTextBox.Text = filePath;
            }
        }

        private void LeftHandGripButton_Click(object sender, RoutedEventArgs e)
        {
            String filePath = openFileChooser();
            if (filePath != null)
            {
                LeftHandGripTextBox.Text = filePath;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (LeftHandTextBox.Text != String.Empty)
                Properties.Settings.Default.left_hand = LeftHandTextBox.Text;

            if (RightHandTextBox.Text != String.Empty)
                Properties.Settings.Default.right_hand = RightHandTextBox.Text;

            if (LeftHandGripTextBox.Text != String.Empty)
                Properties.Settings.Default.left_hand_grip = LeftHandGripTextBox.Text;

            if (RightHandGripTextBox.Text != String.Empty)
                Properties.Settings.Default.right_hand_grip = RightHandGripTextBox.Text;

            Properties.Settings.Default.Save();
        }
    }
}
