using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TestKinect
{
    class Word:Image
    {
        //private Image word { get; set; }

        //limit of size for image
        private const int MIN_WIDTH = 100, MAX_WIDTH = 1000;

        //zoom factors
        private const double ZOOM_FACTOR = 1.05, UNZOOM_FACTOR = 0.95;

        //to store the rotation of the hand at the beginning of the movement
        private double word_begin_rotation;

        //to know if the current word is hover
        private bool hoverWord; 
        
        public Word()
        {  
            this.word_begin_rotation = 0;
            this.hoverWord = false;
        }
    }
}
