using System.Windows.Controls;

namespace TestKinect
{
    class GUIObject : Grid
    {
        public GUIObject()
        {
            this._x = 0;
            this._y = 0;
        }

        protected double _x;
        protected double _y;
        public virtual double x { get; set; }
        public virtual double y { get; set; }
    }
}
