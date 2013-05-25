using System;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;

namespace TestKinect
{
    class Hand : GUIObject
    {
        public Hand()
        {
            this._x = 0;
            this._y = 0;
            this._grip = false;
            this.pressed = false;
            this.justGrip = false;
            this.justReleased = false;
            this.attachedObjectName = "";
            this.path = "";
            this.lastEvent = "";
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
                    this.Source = new BitmapImage(new Uri(path+"_grip.png", UriKind.Relative));
                else
                    this.Source = new BitmapImage(new Uri(path+".png", UriKind.Relative));

            }
        }
        public bool pressed { get; set; }
        public bool justGrip { get; set; }
        public bool justReleased { get; set; }
        public String attachedObjectName { get; set; }
        public String path { get; set; }
        public String lastEvent { get; set; }
        public static double distance = 0;

        public override string ToString()
        {
            String s = _grip ? "Grip" : "Open"; 
            return "X: "+x+", Y: "+y+" , "+s;
        }
    }
}
