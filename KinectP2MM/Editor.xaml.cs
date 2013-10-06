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
    /// <summary>
    /// Logique d'interaction pour Editor.xaml
    /// </summary>
    public partial class Editor : Window
    {
        public Editor()
        {
            InitializeComponent();

            Loaded += WindowLoaded;
            KeyUp += KeyManager;
        }

        private void KeyManager(object sender, KeyEventArgs e)
        {
            ;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            ;
        }
    }
}
