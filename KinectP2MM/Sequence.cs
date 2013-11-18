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
        public String fontType { get; set; }

        public Sequence()
        {
            this.words = new List<Word>();
            this.canZoom = true;
            this.canRotate = true;
            this.fontType = App.FONT_FAMILY;
        }

        public Sequence(List<Word> words, bool canZoom, bool canRotate, String fontType)
        {
            this.words = words;
            this.canZoom = canZoom;
            this.canRotate = canRotate;
            this.fontType = fontType;
        }
    }
}
