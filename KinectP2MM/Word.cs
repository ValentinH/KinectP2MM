using System.Windows.Controls;
using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
namespace KinectP2MM
{
    class Word : GUIObject
    {
        public Word()
        {
            this.beginRotation = 0;
            this._hover = false;
            this.typeWord = "complete";
            Loaded +=WordLoaded;

        }

        public Word(Word source): base(source)

        {
            this.beginRotation = source.beginRotation;
            this._hover = source._hover;

            this.wordBottom = new Label();
            this.wordBottom.Margin = source.wordBottom.Margin;
            this.wordBottom.Height = source.wordBottom.Height;
            this.wordBottom.Width = source.wordBottom.Width;
            this.wordBottom.FontFamily = source.wordBottom.FontFamily;
            this.wordBottom.FontSize = source.wordBottom.FontSize;

            this.wordTop = new Label();
            this.wordTop.Margin = source.wordTop.Margin;
            this.wordTop.Height = source.wordTop.Height;
            this.wordTop.Width = source.wordTop.Width;
            this.wordTop.FontFamily = source.wordTop.FontFamily;
            this.wordTop.FontSize = source.wordTop.FontSize;

            this.sourceTop = source.sourceTop;
            this.sourceBottom = source.sourceBottom;
            this.wordTop.Content = this.sourceTop;
            this.wordBottom.Content = this.sourceBottom;
            this.Children.Add(this.wordTop);
            this.Children.Add(this.wordBottom);

            long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            this.Name = source.Name + "_dup_" + milliseconds;

        }
        public static int MIN_WIDTH = 100, MAX_WIDTH = 1000;

        public static double ZOOM_FACTOR = 1.05, UNZOOM_FACTOR = 0.95;

        public double beginRotation { get; set; }
        public Label wordTop { get; set; }
        public Label wordBottom { get; set; }
        
        private void WordLoaded(object sender, RoutedEventArgs e)
        {
            this.wordTop = (Label)this.Children[0];
            this.wordBottom = (Label)this.Children[1];
        } 

        public object sourceTop
        {
            get { return wordTop.Content; }
            set
            {
                wordTop.Content = value;                
            }
        }
        public object sourceBottom
        {
            get { return wordBottom.Content; }
            set
            {
                wordBottom.Content = value;
                
            }
        }
       
        
        public String typeWord { get; set; }
        public Guid id { get; set; }
        public Guid oldIdPartner { get; set; }
            

        
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
            this.wordTop.Opacity = 0;
            this.typeWord = "bottom";
            
            top.wordBottom.Opacity = 0;
            top.typeWord = "top";

            top.id = Guid.NewGuid();
            this.id = Guid.NewGuid();
            top.oldIdPartner = this.id;
            this.oldIdPartner = top.id;

            return top;
        }

        public void Fusion(Word secondWord)
        {
            if(this.typeWord == "top")
            {                
                this.wordBottom.Opacity = 1;
                this.sourceBottom = secondWord.sourceBottom;
                this.typeWord = "complete";
            }
            else
                if(this.typeWord == "bottom")
                {                
                    this.wordTop.Opacity = 1;
                    this.sourceTop = secondWord.sourceTop;
                    this.typeWord = "complete";
                }

           
        }
    }
}
