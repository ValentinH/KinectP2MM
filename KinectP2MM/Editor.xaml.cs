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
        private bool unsaveChanges;

        public Editor(MainWindow parentWindow)
        {
            InitializeComponent();

            listSequences = new List<Sequence>();
            jsonManager = new JsonManager();

            sequenceCount = 1;

            this.parentWindow = parentWindow;

            PoliceTextBlock.Text = "Police :\n  1 : " + Properties.Settings.Default.font1_full + "\n  2 : " + Properties.Settings.Default.font2_full;

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
            VerificatinSauvegarde();   
            openFile();
        }
        
        private void OpenCanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }   

        private void NewCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            VerificatinSauvegarde();  
            newFile();
        }        

        private void NewCanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void VerificatinSauvegarde()
        {
            if (unsaveChanges)
            {
                MessageBoxResult result = MessageBox.Show("Voulez vous enregistrer le fichier actuel ?", "Attention", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (filename != String.Empty)
                    {
                        saveSequence();
                        jsonManager.save(listSequences, filename);
                    }
                    else
                        saveFileChooser();
                }
            }            
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
                unsaveChanges = false;
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
            unsaveChanges = false;
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
            unsaveChanges = true;
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
            listTypeTextBox.Clear();
            listPoliceTextBox.Clear();
            listTailleTextBox.Clear();
            listLeftBottomCornerTextBox.Clear();
            listLeftTopCornerTextBox.Clear();
            listRightBottomCornerTextBox.Clear();
            listRightTopCornerTextBox.Clear();
            canZoomBox.IsChecked = false;
            canRotateBox.IsChecked = false;
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
                listTypeTextBox.Text += (int)word.typeWord + "\r\n";
                if(word.fontFamily == Properties.Settings.Default.font1_full)
                    listPoliceTextBox.Text += 1 + "\r\n";
                else
                    listPoliceTextBox.Text += 2 + "\r\n";
                listTailleTextBox.Text += word.fontSize + "\r\n";
            }

            if(listSequences[sequenceNumber].leftBottomCornerWords != null)
                foreach (String mot in listSequences[sequenceNumber].leftBottomCornerWords)
                {
                    listLeftBottomCornerTextBox.Text += mot + "\r\n";
                }

            if (listSequences[sequenceNumber].rightBottomCornerWords != null)
                foreach (String mot in listSequences[sequenceNumber].rightBottomCornerWords)
                {
                    listRightBottomCornerTextBox.Text += mot + "\r\n";
                }

            if (listSequences[sequenceNumber].leftTopCornerWords != null)
                foreach (String mot in listSequences[sequenceNumber].leftTopCornerWords)
                {
                    listLeftTopCornerTextBox.Text += mot + "\r\n";
                }

            if (listSequences[sequenceNumber].rightTopCornerWords != null)
                foreach (String mot in listSequences[sequenceNumber].rightTopCornerWords)
                {
                    listRightTopCornerTextBox.Text += mot + "\r\n";
                }

            unsaveChanges = false;

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
            int x = 0, y = 0;
            WordType type = WordType.FULL;
            String fontFamily = Properties.Settings.Default.font1_full;
            int fontSize = Word.FONTSIZE;

            String[] splitString = { "\r\n" };
            String wordsUntreated = listWordsTextBox.Text;
            String[] wordsTreated = wordsUntreated.Split(splitString, StringSplitOptions.RemoveEmptyEntries);

            String xUntreated = listXTextBox.Text;
            String[] xTreated = xUntreated.Split(splitString, StringSplitOptions.RemoveEmptyEntries);

            String yUntreated = listYTextBox.Text;
            String[] yTreated = yUntreated.Split(splitString, StringSplitOptions.RemoveEmptyEntries);

            String TypeUntreated = listTypeTextBox.Text;
            String[] TypeTreated = TypeUntreated.Split(splitString, StringSplitOptions.RemoveEmptyEntries);

            String PoliceUntreated = listPoliceTextBox.Text;
            String[] PoliceTreated = PoliceUntreated.Split(splitString, StringSplitOptions.RemoveEmptyEntries);

            String TailleUntreated = listTailleTextBox.Text;
            String[] TailleTreated = TailleUntreated.Split(splitString, StringSplitOptions.RemoveEmptyEntries);

            String LeftBottomCornerUntreated = listLeftBottomCornerTextBox.Text;
            String[] LeftBottomCornerTreated = LeftBottomCornerUntreated.Split(splitString, StringSplitOptions.RemoveEmptyEntries);

            String RightBottomCornerUntreated = listRightBottomCornerTextBox.Text;
            String[] RightBottomCornerTreated = RightBottomCornerUntreated.Split(splitString, StringSplitOptions.RemoveEmptyEntries);

            String LeftTopCornerUntreated = listLeftTopCornerTextBox.Text;
            String[] LeftTopCornerTreated = LeftTopCornerUntreated.Split(splitString, StringSplitOptions.RemoveEmptyEntries);

            String RightTopCornerUntreated = listRightTopCornerTextBox.Text;
            String[] RightTopCornerTreated = RightTopCornerUntreated.Split(splitString, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < wordsTreated.Count(); i++)
            {                
                // Gestion de x
                if (i < xTreated.Count())
                    x = Convert.ToInt32(xTreated[i]);
                
                //Gestion de y
                if(i < yTreated.Count())
                    y = Convert.ToInt32(yTreated[i]);

                // Gestion du type
                if(i < TypeTreated.Count())
                {
                    if(Convert.ToInt32(TypeTreated[i]) == 0)
                        type = WordType.FULL;
                    else if (Convert.ToInt32(TypeTreated[i]) == 1)
                        type = WordType.BOTTOM;
                    else if (Convert.ToInt32(TypeTreated[i]) == 2)
                        type = WordType.TOP;
                }

                //Gestion de la police
                if (i < PoliceTreated.Count())
                {
                    if (Convert.ToInt32(PoliceTreated[i]) == 1)
                        fontFamily = Properties.Settings.Default.font1_full;
                    else if (Convert.ToInt32(PoliceTreated[i]) == 2)
                        fontFamily = Properties.Settings.Default.font2_full;
                }

                //Gestion de la taille
                if (i < TailleTreated.Count() && Convert.ToInt32(TailleTreated[i]) != 0)
                    fontSize = Convert.ToInt32(TailleTreated[i]);


                //Création du mot avec les données vérifiées
                words.Add(new Word(wordsTreated[i], fontFamily, fontSize, x, y, type));
                
            }

            int sequenceNumber = sequenceComboBox.SelectedIndex;

            listSequences[sequenceNumber].canRotate = canRotate;
            listSequences[sequenceNumber].canZoom = canZoom;
            listSequences[sequenceNumber].words = words;
            listSequences[sequenceNumber].leftBottomCornerWords = new List<String>(LeftBottomCornerTreated);
            listSequences[sequenceNumber].rightBottomCornerWords = new List<String>(RightBottomCornerTreated);
            listSequences[sequenceNumber].leftTopCornerWords = new List<String>(LeftTopCornerTreated);
            listSequences[sequenceNumber].rightTopCornerWords = new List<String>(RightTopCornerTreated);

            unsaveChanges = false;
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
                unsaveChanges = true;
            }
            else
            {
                newFile();
            }

        }
        
        private void Editor_Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            VerificatinSauvegarde();
            if(filename != String.Empty)
                parentWindow.loadJson(filename);
            unsaveChanges = false;
        }

        private void canRotate_Checked(object sender, RoutedEventArgs e)
        {
            canRotate = true;
            unsaveChanges = true;
        }

        private void canRotate_Unchecked(object sender, RoutedEventArgs e)
        {
            canRotate = false;
            unsaveChanges = true;
        }

        private void canZoom_Checked(object sender, RoutedEventArgs e)
        {
            canZoom = true;
            unsaveChanges = true;
        }

        private void canZoom_Unchecked(object sender, RoutedEventArgs e)
        {
            canZoom = false;
            unsaveChanges = true;
        }

        private void listWordsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            unsaveChanges = true;
        }

        private void listXTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            unsaveChanges = true;
        }

        private void listYTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            unsaveChanges = true;
        }

        private void listTailleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            unsaveChanges = true;
        }

        private void listPoliceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            unsaveChanges = true;
        }

        private void listLeftBottomCornerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            unsaveChanges = true;
        }

        private void listRightBottomCornerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            unsaveChanges = true;
        }

        private void listLeftTopCornerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            unsaveChanges = true;
        }

        private void listRightTopCornerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            unsaveChanges = true;
        }
    }

}
