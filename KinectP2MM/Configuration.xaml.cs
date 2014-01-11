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
        bool unsaveChanges;
        bool firstOpening;

        public Configuration(MainWindow parentWindow)
        {
            InitializeComponent();

            this.parentWindow = parentWindow;

            LeftHandTextBox.Text = Properties.Settings.Default.left_hand;
            RightHandTextBox.Text = Properties.Settings.Default.right_hand;
            LeftHandGripTextBox.Text = Properties.Settings.Default.left_hand_grip;
            RightHandGripTextBox.Text = Properties.Settings.Default.right_hand_grip;

            ICollection<FontFamily> ListFonts = Fonts.SystemFontFamilies;
            IEnumerable<FontFamily> ListFonts2 = ListFonts.OrderBy(f => f.Source);

            foreach (FontFamily fontFamily in ListFonts2)
            {
                ComboBoxItem font1 = new ComboBoxItem();
                ComboBoxItem font2 = new ComboBoxItem();
                ComboBoxItem font3 = new ComboBoxItem();
                ComboBoxItem font4 = new ComboBoxItem();
                ComboBoxItem font5 = new ComboBoxItem();
                ComboBoxItem font6 = new ComboBoxItem();

                font1.Content = fontFamily.Source;
                font2.Content = fontFamily.Source;
                font3.Content = fontFamily.Source;
                font4.Content = fontFamily.Source;
                font5.Content = fontFamily.Source;
                font6.Content = fontFamily.Source;

                this.ComboBoxEntiere1.Items.Add(font1);
                this.ComboBoxEntiere2.Items.Add(font2);
                this.ComboBoxBas1.Items.Add(font3);
                this.ComboBoxBas2.Items.Add(font4);
                this.ComboBoxHaut1.Items.Add(font5);
                this.ComboBoxHaut2.Items.Add(font6);
            }

            this.ComboBoxEntiere1.Text = Properties.Settings.Default.font1_full;
            this.ComboBoxEntiere2.Text = Properties.Settings.Default.font2_full;
            this.ComboBoxBas1.Text = Properties.Settings.Default.font1_bottom;
            this.ComboBoxBas2.Text = Properties.Settings.Default.font2_bottom;
            this.ComboBoxHaut1.Text = Properties.Settings.Default.font1_top;
            this.ComboBoxHaut2.Text = Properties.Settings.Default.font2_top;

            //autres
            this.decalagePolices.Value = Properties.Settings.Default.decalage_polices;
            this.distanceAttraction.Value = Properties.Settings.Default.distance_attraction;

            this.ClrPcker_Foreground.SelectedColor = (System.Windows.Media.Color)ColorConverter.ConvertFromString(Properties.Settings.Default.foreground_color);
            this.ClrPcker_Background.SelectedColor = (System.Windows.Media.Color)ColorConverter.ConvertFromString(Properties.Settings.Default.background_color);

            unsaveChanges = false;
            firstOpening = true;
        }

        private String openFileChooser()
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            if (firstOpening)
                dlg.InitialDirectory = App.IMAGE_PATH;
            dlg.Filter = "Images (*.png;*.jpg)|*.png;*.jpg";
            dlg.RestoreDirectory = true;

            firstOpening = false;


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                String filename = dlg.FileName;

                if (filename.StartsWith(dlg.InitialDirectory))
                { }
                else
                {
                    String file = dlg.SafeFileName;
                    System.IO.File.Copy(filename, dlg.InitialDirectory + "\\" + file, true);
                    filename = dlg.InitialDirectory + "\\" + file;
                }

                filename = filename.Substring(filename.IndexOf("images") + 7);
                return filename;
            }
            else
            {
                return null;
            }
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
            saveAll();
            this.Close();
        }

        private void saveAll()
        {
            saveCursors();
            saveFonts();
            saveColors();
            if (saveOthers())
            {
                unsaveChanges = false;
                MessageBoxResult result = MessageBox.Show("L'application doit être redémarrée pour prendre en compte toutes les modifications.\nVoulez-vous la redémarrer maintenant?", "Attention", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    parentWindow.reload();
                }
            }
        }


        private void saveCursors()
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

        private bool saveOthers()
        {
            try
            {
                Properties.Settings.Default.decalage_polices = (int)decalagePolices.Value;
            }
            catch (Exception)
            {
                MessageBox.Show("Valeur de décalage incorrecte.", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            try
            {
                Properties.Settings.Default.distance_attraction = (int )distanceAttraction.Value;
            }
            catch (Exception)
            {
                MessageBox.Show("Valeur de distance incorrecte.", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            Properties.Settings.Default.Save();
            return true;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reset();
            LeftHandTextBox.Text = Properties.Settings.Default.left_hand;
            RightHandTextBox.Text = Properties.Settings.Default.right_hand;
            LeftHandGripTextBox.Text = Properties.Settings.Default.left_hand_grip;
            RightHandGripTextBox.Text = Properties.Settings.Default.right_hand_grip;
            saveFonts(); // // pour ne pas réinitialiser les polices
            saveColors();
            saveOthers();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (unsaveChanges)
            {
                MessageBoxResult result = MessageBox.Show("Voulez vous sauvegarder les modifications ?", "Attention", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    saveAll();
                }
            }
        }

        private void ResetFont_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reset();
            this.ComboBoxEntiere1.Text = Properties.Settings.Default.font1_full;
            this.ComboBoxEntiere2.Text = Properties.Settings.Default.font2_full;
            this.ComboBoxBas1.Text = Properties.Settings.Default.font1_bottom;
            this.ComboBoxBas2.Text = Properties.Settings.Default.font2_bottom;
            this.ComboBoxHaut1.Text = Properties.Settings.Default.font1_top;
            this.ComboBoxHaut2.Text = Properties.Settings.Default.font2_top;
            saveCursors(); // pour ne pas réinitialiser les mains
            saveColors();
            saveOthers();
        }

        private void ResetColor_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reset();
            this.ClrPcker_Foreground.SelectedColor = (System.Windows.Media.Color)ColorConverter.ConvertFromString(Properties.Settings.Default.foreground_color);
            this.ClrPcker_Background.SelectedColor = (System.Windows.Media.Color)ColorConverter.ConvertFromString(Properties.Settings.Default.background_color);
            saveCursors(); // pour ne pas réinitialiser le reste
            saveFonts();
            saveOthers();
        }

        private void saveFonts()
        {
            Properties.Settings.Default.font1_full = this.ComboBoxEntiere1.Text;
            Properties.Settings.Default.font2_full = this.ComboBoxEntiere2.Text;
            Properties.Settings.Default.font1_bottom = this.ComboBoxBas1.Text;
            Properties.Settings.Default.font2_bottom = this.ComboBoxBas2.Text;
            Properties.Settings.Default.font1_top = this.ComboBoxHaut1.Text;
            Properties.Settings.Default.font2_top = this.ComboBoxHaut2.Text;

            Properties.Settings.Default.Save();
        }


        private void saveColors()
        {
            Properties.Settings.Default.background_color = ClrPcker_Background.SelectedColor.ToString();
            Properties.Settings.Default.foreground_color = ClrPcker_Foreground.SelectedColor.ToString();
            Properties.Settings.Default.Save();
        }

        private void LeftHandTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            unsaveChanges = true;
        }

        private void RightHandTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            unsaveChanges = true;
        }

        private void LeftHandGripTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            unsaveChanges = true;
        }

        private void RightHandGripTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            unsaveChanges = true;
        }

        private void ComboBoxEntiere1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            unsaveChanges = true;
        }

        private void ComboBoxBas1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            unsaveChanges = true;
        }

        private void ComboBoxHaut1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            unsaveChanges = true;
        }

        private void ComboBoxEntiere2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            unsaveChanges = true;
        }

        private void ComboBoxBas2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            unsaveChanges = true;
        }

        private void ComboBoxHaut2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            unsaveChanges = true;
        }

        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            unsaveChanges = true;
        }
        private void ClrPcker_Foreground_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            unsaveChanges = true;
        }
    }
}
