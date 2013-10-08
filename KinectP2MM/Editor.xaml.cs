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
        private JsonManager jsonManager;
        private List<Sequence> listSequences;
        private bool canRotate;
        private bool canZoom;
        private int sequenceCount;
        private MainWindow parentWindow;

        public Editor(MainWindow parentWindow)
        {
            InitializeComponent();

            Loaded += WindowLoaded;
            KeyUp += KeyManager;

            listSequences = new List<Sequence>();
            jsonManager = new JsonManager();

            sequenceCount = 1;

            sequenceComboBox.Visibility = Visibility.Hidden;

            this.parentWindow = parentWindow;

            // Create the CommandBinding.
            CommandBinding NewCommandBinding = new CommandBinding(
                ApplicationCommands.New, NewCommandHandler, NewCanExecuteHandler);
            CommandBinding OpenCommandBinding = new CommandBinding(
                ApplicationCommands.Open, OpenCommandHandler, OpenCanExecuteHandler);
            CommandBinding SaveCommandBinding = new CommandBinding(
                ApplicationCommands.Save, SaveCommandHandler, SaveCanExecuteHandler);
            CommandBinding SaveAsCommandBinding = new CommandBinding(
                ApplicationCommands.SaveAs, SaveAsCommandHandler, SaveAsCanExecuteHandler);

            // Add the CommandBinding to the root Window.
            this.CommandBindings.Add(NewCommandBinding);
            this.CommandBindings.Add(OpenCommandBinding);
            this.CommandBindings.Add(SaveCommandBinding);
            this.CommandBindings.Add(SaveAsCommandBinding);

            // KeyGesture for SaveAs
            KeyGesture saveAsCmdKeyGesture = new KeyGesture(Key.S,
            ModifierKeys.Control | ModifierKeys.Shift);            

            ApplicationCommands.SaveAs.InputGestures.Add(saveAsCmdKeyGesture);

        }

        private void SaveAsCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            saveFileChooser();
        }

        private void SaveAsCanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Console.WriteLine("Un fichier est enregistré");
        }

        private void SaveCanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            openFile();
        }

        private void OpenCanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }   

        private void NewCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            newFile();
        }        

        private void NewCanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
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
            saveFileChooser();            
        }

        private void openFile()
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".p2mm";
            dlg.Filter = "Fichiers P2MM (*.p2mm)|*.p2mm";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                jsonManager = new JsonManager(filename);
                listSequences.Clear();
                listSequences = this.jsonManager.load();

                sequenceComboBox.Items.Clear();
                sequenceNumber.Visibility = Visibility.Hidden;
                sequenceComboBox.Visibility = Visibility.Visible;

                this.sequenceCount = 1;
                foreach (Sequence sequence in listSequences)
                {
                    this.sequenceComboBox.Items.Add("Sequence " + sequenceCount);
                    sequenceCount++;
                }

                this.sequenceComboBox.SelectedItem = "Sequence 1";

            }
        }

        private void newFile()
        {
            listSequences.Clear();
            clearSequence();
            sequenceCount = 1;
            sequenceNumber.Content = "Sequence " + sequenceCount;
            sequenceNumber.Visibility = Visibility.Visible;
            sequenceComboBox.Visibility = Visibility.Hidden;
        }

        private void saveFileChooser()
        {
            // Create OpenFileDialog 
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".p2mm";
            dlg.Filter = "Fichiers P2MM (*.p2mm)|*.p2mm";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {

                if (!listWordsTextBox.Text.Equals(String.Empty))
                    addSequence();

                string filename = dlg.FileName;
                jsonManager.save(listSequences, filename);
                this.parentWindow.loadJson(filename);
                this.Close();
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            addSequence();
        }

        private void addSequence()
        {
            List<Word> words = new List<Word>();
            String[] splitString = { "\r\n" };
            String wordsUntreated = listWordsTextBox.Text;
            String[] wordsTreated = wordsUntreated.Split(splitString, StringSplitOptions.RemoveEmptyEntries);

            String xUntreated = listXTextBox.Text;
            String[] xTreated = xUntreated.Split(splitString, StringSplitOptions.RemoveEmptyEntries);

            String yUntreated = listYTextBox.Text;
            String[] yTreated = yUntreated.Split(splitString, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < wordsTreated.Count(); i++)
            {
                if (i < xTreated.Count() && i < yTreated.Count())
                    words.Add(new Word(wordsTreated[i], Convert.ToInt32(xTreated[i]), Convert.ToInt32(yTreated[i])));
                else
                    words.Add(new Word(wordsTreated[i], 0, 0));

            }

            listSequences.Add(new Sequence(words, canZoom, canRotate));

            clearSequence();
            sequenceCount++;
            sequenceNumber.Content = "Sequence " + sequenceCount;
        }

        private void clearSequence()
        {
            listWordsTextBox.Clear();
            listXTextBox.Clear();
            listYTextBox.Clear();
            canZoomBox.IsChecked = false;
            canRotateBox.IsChecked = false;
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

        private void sequenceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clearSequence();

            // permet d'éviter de faire l'action quand on réinitialise la ComboBox
            if (sequenceComboBox.SelectedItem == null)
                return;

            String sequence = (String)sequenceComboBox.SelectedItem;
            Console.WriteLine(sequence);
            sequence = sequence.Replace("Sequence ", String.Empty);
            Console.WriteLine(sequence);

            int sequenceNumber = Convert.ToInt32(sequence);

            if (listSequences[sequenceNumber - 1].canRotate)
                canRotateBox.IsChecked = true;
            if (listSequences[sequenceNumber - 1].canZoom)
                canZoomBox.IsChecked = true;

            foreach(Word word in listSequences[sequenceNumber - 1].words)
            {
                listWordsTextBox.Text += word.wordTop.Content + "\r\n";
                listXTextBox.Text += word.x + "\r\n";
                listYTextBox.Text += word.y + "\r\n";
            }

        }
    }
}
