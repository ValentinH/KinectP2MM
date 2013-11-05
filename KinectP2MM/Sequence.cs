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

        public Sequence()
        {
            this.words = new List<Word>();
            this.canZoom = true;
            this.canRotate = true;
        }

        public Sequence(List<Word> words, bool canZoom, bool canRotate)
        {
            this.words = words;
            this.canZoom = canZoom;
            this.canRotate = canRotate;
        }
    }
}
