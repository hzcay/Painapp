using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Painapp
{
    public abstract class clsDrawObject
    {
        public Point p1;
        public Point p2;
        public Pen myPen = new Pen(Color.Red, 2);
        public Brush myBrush = new SolidBrush(Color.Red);
        public bool isFill = false;

        public abstract void Draw(Graphics myGp);

        public virtual Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    Math.Min(p1.X, p2.X),
                    Math.Min(p1.Y, p2.Y),
                    Math.Abs(p2.X - p1.X),
                    Math.Abs(p2.Y - p1.Y));
            }
        }
    }

    public class clsComplexobject : clsDrawObject
    {

        protected List<clsDrawObject> lstObject = new List<clsDrawObject>();
        public void AddObject(clsDrawObject obj)
        {
            lstObject.Add(obj);
        }

        public List<clsDrawObject> GetObjects()
        {
            return lstObject;
        }
        public int Count()
        {
            return lstObject.Count();
        }

        public void RemoveAll()
        {
            int count = lstObject.Count();
            lstObject.Clear();
        }
        public bool Contains(Point p)
        {
            return this.Bounds.Contains(p);
        }
        public override void Draw(Graphics myGp)
        {
            foreach (var obj in lstObject)
            {
                obj.Draw(myGp);
            }
        }

        public override Rectangle Bounds
        {
            get
            {
                if (lstObject.Count == 0)
                    return new Rectangle();

                int minX = lstObject.Min(obj => obj.Bounds.Left);
                int minY = lstObject.Min(obj => obj.Bounds.Top);
                int maxX = lstObject.Max(obj => obj.Bounds.Right);
                int maxY = lstObject.Max(obj => obj.Bounds.Bottom);

                return new Rectangle(minX, minY, maxX - minX, maxY - minY);
            }
        }
    }
    public class clsLine : clsDrawObject
    {
        public override void Draw(Graphics myGp)
        {
        myGp.DrawLine(myPen, p1, p2);
        }
    }

    public class clsEllipse : clsDrawObject
    {
        public override void Draw(Graphics myGp)
        {
            if (isFill)
            {
                myGp.FillEllipse(myBrush, p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y);
            }
            myGp.DrawEllipse(myPen, p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y);
        }
    }

    public class clsRectangle : clsDrawObject
    {
        public override void Draw(Graphics myGp)
        {
            int width = Math.Abs(p2.X - p1.X);
            int height = Math.Abs(p2.Y - p1.Y);

            int x = Math.Min(p1.X, p2.X);
            int y = Math.Min(p1.Y, p2.Y);
            if (isFill)
            {
                myGp.FillRectangle(myBrush, x, y, width, height);
            }
            myGp.DrawRectangle(myPen, x, y, width, height);

        }
    }

    public class clsSquare : clsDrawObject
    {
        public override void Draw(Graphics myGp)
        {
            int width = Math.Abs(p2.X - p1.X);
            int height = Math.Abs(p2.Y - p1.Y);
            int size = Math.Min(width, height);
            int x = p1.X < p2.X ? p1.X : p1.X - size;
            int y = p1.Y < p2.Y ? p1.Y : p1.Y - size;
            if (isFill)
            {
                myGp.FillRectangle(myBrush, x, y, size, size);
            }
            myGp.DrawRectangle(myPen, x, y, size, size);
        }
    }
    

    public class clsCircle : clsDrawObject
    {
        public override void Draw(Graphics myGp)
        {
            int width = Math.Abs(p2.X - p1.X);
            int height = Math.Abs(p2.Y - p1.Y);
            int size = Math.Min(width, height);
            int x = p1.X < p2.X ? p1.X : p1.X - size;
            int y = p1.Y < p2.Y ? p1.Y : p1.Y - size;
            if (isFill)
            {
                myGp.FillEllipse(myBrush, x, y, size, size);
            }
            myGp.DrawEllipse(myPen, x, y, size, size);
        }
    }

    public class clsCurve : clsDrawObject
    {
        public override void Draw(Graphics myGp)
        { 
        Point p3 = new Point((p1.X + p2.X) / 2,p1.Y);
        myGp.DrawCurve(myPen, new Point[] { p1, p3, p2 });
        }
    }

    public class clsPolygon : clsDrawObject
    {
        public override void Draw(Graphics myGp)
        {

        int width = p2.X - p1.X;  
        int height = p2.Y - p1.Y; 
        Point[] hexagonPoints = {
            new Point(p1.X + width / 4, p1.Y),       
            new Point(p1.X + 3 * width / 4, p1.Y),   
            new Point(p2.X, p1.Y + height / 2),     
            new Point(p1.X + 3 * width / 4, p2.Y),   
            new Point(p1.X + width / 4, p2.Y),      
            new Point(p1.X, p1.Y + height / 2)       
        };
            if (isFill)
            {
                myGp.FillPolygon(myBrush, hexagonPoints);
            }
                myGp.DrawPolygon(myPen, hexagonPoints);
        }
    }

    public class clsTriangle : clsDrawObject
    {
        public override void Draw(Graphics myGp)
        {
            Point p3 = new Point(p1.X, p2.Y);
            Point p5 = new Point(p2.X, p2.Y);
            Point[] trianglePoints = { p1, p3, p5 };
            if (isFill)
            {
                myGp.FillPolygon(myBrush, trianglePoints);
            }
            myGp.DrawPolygon(myPen, trianglePoints);
        }
    }
}
