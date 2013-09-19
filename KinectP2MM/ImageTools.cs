using System;
using System.Windows;

namespace KinectP2MM
{
    class ImageTools
    {

        //to know whether o1 is inside o2
        public static bool isOn(GUIObject o1, GUIObject o2)
        {            
            return (o1.x + o1.ActualWidth / 2 >= o2.x && (o1.x < o2.x + o2.ActualWidth) && o1.y + o1.ActualHeight / 2 >= o2.y && (o1.y < o2.y + o2.ActualHeight));        
        }

        //distance calculation
        public static double getDistance(GUIObject o1, GUIObject o2)
        {
            return (o2.x - o1.x) * (o2.x - o1.x) + (o2.y - o1.y) * (o2.y - o1.y);
        }

        //rotation calculation in degrees
        public static double getRotation(GUIObject o1, GUIObject o2)
        {
            Point A = new Point(o1.x, o1.y);
            Point B = new Point(o2.x, o2.y);

            Point C = new Point(B.X, A.Y);

            var AC = C.X - A.X;
            var AB = Math.Sqrt((B.X - A.X) * (B.X - A.X) + (B.Y - A.Y) * (B.Y - A.Y));
            var BC = C.Y - B.Y;

            var cos = Math.Acos(AC / AB);
            var sin = Math.Asin(BC / AB);

            double angle = Math.Acos(AC / AB) * 180 / Math.PI;
            if (sin < 0)
                angle = 360 - angle;

            return angle;
        }
    }
}
