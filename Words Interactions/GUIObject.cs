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
        public GUIObject(GUIObject source)
        {
            this._x = source._x;
            this._y = source._y;
            
            //Copy grid info
            this.Height = source.Height;
            this.Width = source.Width;
            this.SetCanvas(source);
            


        }

        public void SetCanvas(GUIObject source){
            //get canvas position from GuiObject source
            Canvas.SetLeft(this, Canvas.GetLeft(source));
            Canvas.SetTop(this, Canvas.GetTop(source)); 
        }

        protected double _x;
        protected double _y;
        public virtual double x { get; set; }
        public virtual double y { get; set; }
    }
}
