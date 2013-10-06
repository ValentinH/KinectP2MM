using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect.Toolkit.Interaction;
using System.Windows.Input;


namespace KinectP2MM
{
    // Logique d'interaction pour MainWindow.xaml
    public partial class MainWindow : Window
    {
        private MainManager mainManager;
        private bool fullScreen;
        private bool inputOpen;

        public MainWindow()
        {
            InitializeComponent();
            //references the OnLoaded function on the OnLoaded event
            Loaded += WindowLoaded;
            KeyUp += KeyManager;
        }

        // Execute startup tasks
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            fullScreen = false;
            inputOpen = false;
            this.mainManager = new MainManager(this);
            this.Activate();
        }

        // Manage Key Events
        private void KeyManager(object sender, KeyEventArgs e)
        {
            if (!inputOpen)
            {
                if ((e.Key >= Key.D1 && e.Key <= Key.D9) || (e.Key >= Key.NumPad1 && e.Key <= Key.NumPad9))
                {
                    JsonLoader jsonLoader = new JsonLoader();
                    //create list from JSON
                    List<Sequence> sequences = jsonLoader.load();

                    if ((e.Key == Key.D1 || e.Key == Key.NumPad1) && sequences.Count >= 1)
                        this.mainManager.loadSequence(sequences[0]);
                    else if ((e.Key == Key.D2 || e.Key == Key.NumPad2) && sequences.Count >= 2)
                        this.mainManager.loadSequence(sequences[1]);
                    else if ((e.Key == Key.D3 || e.Key == Key.NumPad3) && sequences.Count >= 3)
                        this.mainManager.loadSequence(sequences[2]);
                    else if ((e.Key == Key.D4 || e.Key == Key.NumPad4) && sequences.Count >= 4)
                        this.mainManager.loadSequence(sequences[3]);
                    else if ((e.Key == Key.D5 || e.Key == Key.NumPad5) && sequences.Count >= 5)
                        this.mainManager.loadSequence(sequences[4]);
                    else if ((e.Key == Key.D6 || e.Key == Key.NumPad6) && sequences.Count >= 6)
                        this.mainManager.loadSequence(sequences[5]);
                    else if ((e.Key == Key.D7 || e.Key == Key.NumPad7) && sequences.Count >= 7)
                        this.mainManager.loadSequence(sequences[6]);
                    else if ((e.Key == Key.D8 || e.Key == Key.NumPad8) && sequences.Count >= 8)
                        this.mainManager.loadSequence(sequences[7]);
                    else if ((e.Key == Key.D9 || e.Key == Key.NumPad9) && sequences.Count >= 9)
                        this.mainManager.loadSequence(sequences[8]);
                }

                if (e.Key == Key.Back)
                    this.mainManager.loadSequence(new Sequence());

                if (e.Key == Key.Escape)
                    toggleFullscreen(false);
                if (e.Key == Key.F11 || e.Key == Key.F)
                    toggleFullscreen(!fullScreen);

                if (e.Key == Key.N)
                    showAddWord();
            }
            else
            {
                if (e.Key == Key.Enter)
                {
                    validateInput();
                }
                if (e.Key == Key.Escape)
                    cancelInput();
            }
        }


        private void showAddWord()
        {
            inputOpen = true;
            InputBox.Visibility = System.Windows.Visibility.Visible;
            InputTextBox.Focusable = true;
            Keyboard.Focus(InputTextBox);
        }


        private void validateInput()
        {
            inputOpen = false;
            this.mainManager.addWord(InputTextBox.Text);
            InputBox.Visibility = System.Windows.Visibility.Collapsed;
            InputTextBox.Text = String.Empty;
        }

        private void cancelInput()
        {
            inputOpen = false;
            InputBox.Visibility = System.Windows.Visibility.Collapsed;
            InputTextBox.Text = String.Empty;
        }

        private void toggleFullscreen(bool fs)
        {
            if (fs)
            {
                this.ResizeMode = ResizeMode.NoResize;
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
                this.Topmost = true;
                fullScreen = true;
            }
            else
            {
                this.ResizeMode = ResizeMode.CanResize;
                this.WindowState = System.Windows.WindowState.Normal;
                this.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
                this.Topmost = false;
                fullScreen = false;
            }
        }

        
    }

}


