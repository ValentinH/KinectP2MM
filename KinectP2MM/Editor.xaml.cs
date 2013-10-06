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
    // Logique d'interaction pour Editor.xaml
    public partial class Editor : Window
    {
        private JsonLoader jsonLoader;
        private List<Sequence> listSequences;
        private bool canRotate;
        private bool canZoom;
        private int sequenceCount;

        public Editor()
        {
            InitializeComponent();

            Loaded += WindowLoaded;
            KeyUp += KeyManager;

            listSequences = new List<Sequence>();
            jsonLoader = new JsonLoader();

            sequenceCount = 1;
        }

        private void KeyManager(object sender, KeyEventArgs e)
        {
            ;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            ;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            jsonLoader.save(listSequences);
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            List<Word> words = new List<Word>();
            String[] splitString = {"\r\n"};
            String wordsUntreated = listWordsTextBox.Text;
            String[] wordsTreated = wordsUntreated.Split(splitString, StringSplitOptions.RemoveEmptyEntries);

            foreach (String word in wordsTreated)
            {
                words.Add(new Word(word, 0, 0));
            }

            listSequences.Add(new Sequence(words, canZoom, canRotate));

            listWordsTextBox.Clear();
            canZoomBox.IsChecked = false;
            canRotateBox.IsChecked = false;
            sequenceCount++;
            sequenceNumber.Content = "Sequence " + sequenceCount;

        }

        private void canRotate_Checked(object sender, RoutedEventArgs e)
        {
            canRotate = true;
        }

        private void canRotate_Unchecked(object sender, RoutedEventArgs e)
        {
            canRotate = false;
        }        

        private void canZoom_Checked(object sender, RoutedEventArgs e)
        {
            canZoom = true;
        }

        private void canZoom_Unchecked(object sender, RoutedEventArgs e)
        {
            canZoom = false;
        }
    }
}
