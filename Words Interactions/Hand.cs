using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TestKinect
{
    class Hand
    {
       
        public double x {get; set;}
        public double y { get; set; }
        public bool grip { get; set; }
        public bool pressed { get; set; }
        public bool justGrip { get; set; }
        public String attachedObjectName { get; set; }
        

       public Hand()
        {
            this.x = 0;
            this.y = 0;
            this.grip = false;
            this.pressed = false;
            this.justGrip = false;
            this.justReleased = false;
            this.attachedObjectName = "";
        }

        public override string ToString()
        {
            String s = grip ? "Grip" : "Open"; 
            return "X: "+x+", Y: "+y+" , "+s;
        }
    }
}
