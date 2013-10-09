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
        private String filename;

        public Editor(MainWindow parentWindow)
        {
            InitializeComponent();

            listSequences = new List<Sequence>();
            jsonManager = new JsonManager();

            sequenceCount = 1;

            this.parentWindow = parentWindow;

            newFile();

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
            KeyGesture saveAsCmdKeyGesture = new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift);            

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
            if (filename != String.Empty)
            {
                saveSequence();
                jsonManager.save(listSequences, filename);
            }
            else
                saveFileChooser();
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
                filename = dlg.FileName;
                jsonManager = new JsonManager(filename);
                listSequences.Clear();
                listSequences = this.jsonManager.load();

                sequenceComboBox.Items.Clear();

                this.sequenceCount = 0;
                foreach (Sequence sequence in listSequences)
                {
                    this.sequenceComboBox.Items.Add("Séquence " + (sequenceCount+1));
                    sequenceCount++;
                }
                
                this.sequenceComboBox.SelectedItem = this.sequenceComboBox.Items[0];

            }
        }

        private void newFile()
        {
            filename = String.Empty;
            sequenceComboBox.Items.Clear();
            listSequences.Clear();
            clearSequence();
            sequenceCount = 0;
            addSequence();
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

                if (listWordsTextBox.Text != String.Empty)
                    saveSequence();

                filename = dlg.FileName;
                jsonManager.save(listSequences, filename);
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            saveSequence();
            sequenceComboBox.SelectedItem = null;
            addSequence();
        }

        private void addSequence()
        {
            listSequences.Add(new Sequence());

            clearSequence();
            this.sequenceComboBox.Items.Add("Séquence " + (sequenceCount+1));
            sequenceComboBox.SelectedIndex = sequenceCount++;
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
            
            int sequenceNumber = sequenceComboBox.SelectedIndex;

            if (listSequences[sequenceNumber].canRotate)
                canRotateBox.IsChecked = true;
            if (listSequences[sequenceNumber].canZoom)
                canZoomBox.IsChecked = true;

            foreach(Word word in listSequences[sequenceNumber].words)
            {
                listWordsTextBox.Text += word.wordTop.Content + "\r\n";
                listXTextBox.Text += word.x + "\r\n";
                listYTextBox.Text += word.y + "\r\n";
            }

        }

        private void sequenceComboBox_MouseEnter(object sender, MouseEventArgs e)
        {
            saveSequence();
        }

        private void saveSequence()
        {
            if (sequenceComboBox.SelectedItem == null)
                return;

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

            int sequenceNumber = sequenceComboBox.SelectedIndex;

            listSequences[sequenceNumber].canRotate = canRotate;
            listSequences[sequenceNumber].canZoom = canZoom;
            listSequences[sequenceNumber].words = words;
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sequenceCount > 1)
            {
                int sequenceNumber = sequenceComboBox.SelectedIndex;
                if (sequenceNumber == -1) return;
                sequenceCount--;

                listSequences.RemoveAt(sequenceNumber);
                int newIndex = sequenceNumber - 1;

                this.sequenceComboBox.Items.RemoveAt(sequenceNumber);

                for (int i = sequenceNumber; i < this.sequenceComboBox.Items.Count; i++)
                {
                    this.sequenceComboBox.Items[i] = "Séquence "+ (i+1);
                }

                if (newIndex < 0)
                    newIndex = 0;
                this.sequenceComboBox.SelectedIndex = newIndex;
            }
            else
            {
                newFile();
            }
        }
    }
}
