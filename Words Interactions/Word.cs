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

        public Word(Word source): base(source)

        {
            this.beginRotation = source.beginRotation;
            this._hover = source._hover;

            this.wordBottom = new Image();
            this.wordBottom.Source = source.wordBottom.Source;
            this.wordBottom.Margin = source.wordBottom.Margin;
            this.wordBottom.Height = source.wordBottom.Height;
            this.wordBottom.Width = source.wordBottom.Width;

            this.wordTop = new Image();
            this.wordTop.Source = source.wordTop.Source;
            this.wordTop.Margin = source.wordTop.Margin;
            this.wordTop.Height = source.wordTop.Height;
            this.wordTop.Width = source.wordTop.Width;

            

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

        public Word Duplicate() //Return a copy of the word with blank bottom image. Delete top bottom image from current word.
        {
            Word top = new Word(this);
            this.wordTop = null;
            top.wordBottom = null;

            return top;
        }
    }
}
