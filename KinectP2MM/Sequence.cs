using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectP2MM
{
    public class Sequence
    {
        public List<Word> words { get; set; }
        public bool canZoom { get; set; }
        public bool canRotate { get; set; }
        public List<String> leftBottomCornerWords { get; set; }
        public List<String> rightBottomCornerWords { get; set; }
        public List<String> leftTopCornerWords { get; set; }
        public List<String> rightTopCornerWords { get; set; }
        public String foregroundColor { get; set; }
        public String backgroundColor { get; set; }

        public Sequence()
        {
            this.words = new List<Word>();
            this.canZoom = true;
            this.canRotate = true;
            this.leftBottomCornerWords = new List<String>();
            this.rightBottomCornerWords = new List<String>();
            this.leftTopCornerWords = new List<String>();
            this.rightTopCornerWords = new List<String>();
        }

        public Sequence(List<Word> words, bool canZoom, bool canRotate, List<String> leftBottomCornerWords,
            List<String> rightBottomCornerWords, List<String> leftTopCornerWords, List<String> rightTopCornerWords, String foreground, String background)
        {
            this.words = words;
            this.canZoom = canZoom;
            this.canRotate = canRotate;
            this.leftBottomCornerWords = leftBottomCornerWords;
            this.rightBottomCornerWords = rightBottomCornerWords;
            this.leftTopCornerWords = leftTopCornerWords;
            this.rightTopCornerWords = rightTopCornerWords;
            this.foregroundColor = foreground;
            this.backgroundColor = background;
        }
    }
}
