using System;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace KinectP2MM
{
    class Hand : GUIObject
    {
        public bool pressed { get; set; }
        public bool justGrip { get; set; }
        public bool justReleased { get; set; }
        public Guid attachedObjectId { get; set; }
        public String path { get; set; }
        public String path_grip { get; set; }
        public String lastEvent { get; set; }
        public static double distance = 0;

        public Image handCursor { get; set; }
        public ImageSource source
        {
            get { return handCursor.Source; }
            set
            {
                handCursor.Source = value;

            }
        }
        public override double x
        {
            get { return _x; }
            set
            {
                _x = value;
                Canvas.SetLeft(this, value);
            }
        }

        public override double y
        {
            get { return _y; }
            set
            {
                _y = value;
                Canvas.SetTop(this, value);
            }
        }
        private bool _grip;
        public bool grip
        {
            get { return _grip; }
            set
            {
                _grip = value;
                if (_grip)
                    this.handCursor.Source = new BitmapImage(new Uri(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\KinectP2MM\\images\\" + path_grip, UriKind.Absolute));
                else
                    this.handCursor.Source = new BitmapImage(new Uri(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\KinectP2MM\\images\\" + path, UriKind.Absolute));

            }
        }
        
        public Hand()
        {
            this._x = 0;
            this._y = 0;
            this._grip = false;
            this.pressed = false;
            this.justGrip = false;
            this.justReleased = false;
            this.attachedObjectId = Guid.Empty;
            this.path = "";
            this.path_grip = "";
            this.lastEvent = "";

            Loaded += HandLoaded;
        }

        // Execute startup tasks
        private void HandLoaded(object sender, RoutedEventArgs e)
        {
            this.handCursor = (Image)this.Children[0];
            this.reload();
        }

        public void reload()
        {
            Uri uri = new Uri(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\KinectP2MM\\images\\" + path, UriKind.Absolute);
            if(uri != null && uri.IsFile)
                this.handCursor.Source = new BitmapImage(uri);
        } 

        
        public override string ToString()
        {
            String s = _grip ? "Grip" : "Open";
            return "X: " + x + ", Y: " + y + " , " + s;
        }
    }
}
