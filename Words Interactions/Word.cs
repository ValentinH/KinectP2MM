﻿using System.Windows.Controls;
using System;
using System.Windows.Media;
using System.Windows;
namespace TestKinect
{
    class Word : GUIObject
    {
        public Word()
        {
            this.beginRotation = 0;
            this._hover = false;
            Loaded +=WordLoaded;
        }
        public static int MIN_WIDTH = 100, MAX_WIDTH = 1000;

        public static double ZOOM_FACTOR = 1.05, UNZOOM_FACTOR = 0.95;

        public double beginRotation { get; set; }
        public Image wordTop { get; set; }
        public Image wordBottom { get; set; }
        
        private void WordLoaded(object sender, RoutedEventArgs e)
        {
            this.wordTop = (Image)this.Children[0];
            this.wordBottom = (Image)this.Children[1];
        } 

        public ImageSource sourceTop
        {
            get { return wordTop.Source; }
            set
            {
                wordTop.Source = value;
                
            }
        }
        public ImageSource sourceBottom
        {
            get { return wordBottom.Source; }
            set
            {
                wordBottom.Source = value;
                
            }
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