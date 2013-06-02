using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;



namespace TestKinect
{
    class Bin : GUIObject
    {

        public Bin()
        {
            Loaded += BinLoaded;
        }
        public Image binImage { get; set; }

        private void BinLoaded(object sender, RoutedEventArgs e)
        {
            this.binImage = (Image)this.Children[0];

        }

        private bool _hover { get; set; }
        public bool hover
        {
            get { return _hover; }
            set
            {
                _hover = value;
                if (_hover)
                    this.Opacity = 0.6;
                else
                    this.Opacity = 1;
            }
        }

        public override double x
        {
            get
            {
                if (_x == 0)
                    _x = Canvas.GetLeft(this);
                return _x;
            }
            set
            {
                _x = value;
                Canvas.SetLeft(this, value);
            }
        }

        public override double y
        {
            get
            {
                if (_y == 0)
                    _y = Canvas.GetTop(this);
                return _y;
            }
            set
            {
                _y = value;
                Canvas.SetTop(this, value);
            }
        }
    }
}
