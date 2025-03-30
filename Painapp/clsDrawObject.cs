using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        public bool isSelected = false;

        public abstract void Draw(Graphics myGp);

        public abstract bool Contains(Point point);

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

        public void Move(int deltaX, int deltaY)
        {
            p1.X += deltaX;
            p1.Y += deltaY;
            p2.X += deltaX;
            p2.Y += deltaY;
        }


        protected Point[] GetHandlePoints(Rectangle bounds)
        {
            return new Point[]
            {
                new Point(bounds.Left, bounds.Top),
                new Point(bounds.Right, bounds.Top),
                new Point(bounds.Left, bounds.Bottom),
                new Point(bounds.Right, bounds.Bottom),
                new Point(bounds.Left, bounds.Top + bounds.Height / 2),
                new Point(bounds.Left + bounds.Width / 2, bounds.Top),
                new Point(bounds.Right, bounds.Top + bounds.Height / 2),
                new Point(bounds.Left + bounds.Width / 2, bounds.Bottom),
                new Point(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height)
            };
        }
        
        protected void DrawSelectionHandles(Graphics g, Rectangle bounds)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;
            
            Point[] handlePoints = GetHandlePoints(bounds);
            int handleSize = 7;
            
            for (int i = 0; i < handlePoints.Length - 1; i++)
            {
                var point = handlePoints[i];
                Rectangle handleRect = new Rectangle(
                    point.X - handleSize / 2, 
                    point.Y - handleSize / 2, 
                    handleSize, 
                    handleSize);
                    
                g.FillEllipse(Brushes.White, handleRect);
                g.DrawEllipse(new Pen(Color.DodgerBlue, 1.5f), handleRect);
            }
            
        }

        public virtual void DrawSelection(Graphics g)
        {
            if (isSelected)
            {
                Rectangle bounds = Bounds;
                
                using (Pen selectionPen = new Pen(Color.DodgerBlue, 1.5f))
                {
                    selectionPen.DashStyle = DashStyle.Dash;
                    selectionPen.DashPattern = new float[] { 3, 3 };
                    g.DrawRectangle(selectionPen, bounds);
                }
                
                DrawSelectionHandles(g, bounds);
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

        public void RemoveObject(clsDrawObject obj)
    {
        if (obj != null && lstObject.Contains(obj))
        {
            lstObject.Remove(obj);
        }
    }

    public void RemoveObjects(IEnumerable<clsDrawObject> objects)
    {
        if (objects == null) return;
        
        foreach (var obj in objects.ToList())
        {
            if (lstObject.Contains(obj))
            {
                lstObject.Remove(obj);
            }
        }
    }
        
        public override bool Contains(Point p)
        {
            foreach (var obj in lstObject)
            {
                if (obj.Contains(p))
                    return true;
            }
            return false;
        }
        
        public clsDrawObject FindObjectAt(Point p)
        {
            for (int i = lstObject.Count - 1; i >= 0; i--)
            {
                if (lstObject[i].Contains(p))
                    return lstObject[i];
            }
            return null;
        }
        
        public void BringToFront(clsDrawObject obj)
        {
            if (obj == null || !lstObject.Contains(obj))
                return;
            
            lstObject.Remove(obj);
            lstObject.Add(obj);
        }
        
        public override void Draw(Graphics myGp)
        {
            foreach (var obj in lstObject)
            {
                obj.Draw(myGp);
                obj.DrawSelection(myGp);
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
        
        public override bool Contains(Point point)
        {
            const int tolerance = 5;
            
            double lineLength = Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
            if (lineLength == 0) return false;
            
            double distance = Math.Abs((p2.X - p1.X) * (p1.Y - point.Y) - (p1.X - point.X) * (p2.Y - p1.Y)) / lineLength;
            
            if (distance <= tolerance)
            {
                double dotProduct = ((point.X - p1.X) * (p2.X - p1.X) + (point.Y - p1.Y) * (p2.Y - p1.Y)) / (lineLength * lineLength);
                return dotProduct >= 0 && dotProduct <= 1;
            }
            
            return false;
        }
    }

    public class clsEllipse : clsDrawObject
    {
        public override void Draw(Graphics myGp)
        {
            Rectangle bounds = Bounds;
            if (bounds.Width <= 0 || bounds.Height <= 0) return;
            
            if (isFill)
            {
                myGp.FillEllipse(myBrush, bounds);
            }
            myGp.DrawEllipse(myPen, bounds);
        }
        
        public override bool Contains(Point point)
        {
            Rectangle bounds = Bounds;
            if (bounds.Width <= 0 || bounds.Height <= 0) return false;
            
            double a = bounds.Width / 2.0;
            double b = bounds.Height / 2.0;
            double centerX = bounds.X + a;
            double centerY = bounds.Y + b;
            
            double normalizedX = (point.X - centerX) / a;
            double normalizedY = (point.Y - centerY) / b;
            
            return (normalizedX * normalizedX + normalizedY * normalizedY) <= 1.0;
        }
    }

    public class clsRectangle : clsDrawObject
    {
        public override void Draw(Graphics myGp)
        {
            Rectangle bounds = Bounds;
            if (bounds.Width <= 0 || bounds.Height <= 0) return;
            
            if (isFill)
            {
                myGp.FillRectangle(myBrush, bounds);
            }
            myGp.DrawRectangle(myPen, bounds);
        }
        
        public override bool Contains(Point point)
        {
            return Bounds.Contains(point);
        }
    }

    public class clsSquare : clsDrawObject
    {
        public override void Draw(Graphics myGp)
        {
            Rectangle square = GetSquareRect();
            
            if (square.Width < 1 || square.Height < 1) return;
            
            if (isFill)
            {
                myGp.FillRectangle(myBrush, square);
            }
            myGp.DrawRectangle(myPen, square);
        }
        
        private Rectangle GetSquareRect()
        {
            int width = Math.Abs(p2.X - p1.X);
            int height = Math.Abs(p2.Y - p1.Y);
            int size = Math.Min(width, height);
            
            int x, y;
            
            if (p1.X <= p2.X)
            {
                x = p1.X;
            }
            else
            {
                x = p2.X + (width - size);
            }
            
            if (p1.Y <= p2.Y)
            {
                y = p1.Y;
            }
            else
            {
                y = p2.Y + (height - size);
            }
            
            return new Rectangle(x, y, size, size);
        }
        
        public override bool Contains(Point point)
        {
            return GetSquareRect().Contains(point);
        }
        
        public override Rectangle Bounds
        {
            get
            {
                return GetSquareRect();
            }
        }
    }
    

    public class clsCircle : clsDrawObject
    {
        public override void Draw(Graphics myGp)
        {
            Rectangle circle = GetCircleRect();
            
            if (circle.Width < 1 || circle.Height < 1) return;
            
            if (isFill)
            {
                myGp.FillEllipse(myBrush, circle);
            }
            myGp.DrawEllipse(myPen, circle);
        }
        
        private Rectangle GetCircleRect()
        {
            int width = Math.Abs(p2.X - p1.X);
            int height = Math.Abs(p2.Y - p1.Y);
            int size = Math.Min(width, height);
            
            int x, y;
            
            if (p1.X <= p2.X)
            {
                x = p1.X;
            }
            else
            {
                x = p2.X + (width - size);
            }
            
            if (p1.Y <= p2.Y)
            {
                y = p1.Y;
            }
            else
            {
                y = p2.Y + (height - size);
            }
            
            return new Rectangle(x, y, size, size);
        }
        
        public override bool Contains(Point point)
        {
            Rectangle circle = GetCircleRect();
            
            int centerX = circle.X + circle.Width / 2;
            int centerY = circle.Y + circle.Height / 2;
            int radius = circle.Width / 2;
            
            double distance = Math.Sqrt(Math.Pow(point.X - centerX, 2) + Math.Pow(point.Y - centerY, 2));
            
            return distance <= radius;
        }
        
        public override Rectangle Bounds
        {
            get
            {
                return GetCircleRect();
            }
        }
    }

    public class clsCurve : clsDrawObject
    {
        public override void Draw(Graphics myGp)
        { 
            if (p1.X == p2.X && p1.Y == p2.Y) return;
            
            Point p3 = new Point((p1.X + p2.X) / 2, p1.Y);
            myGp.DrawCurve(myPen, new Point[] { p1, p3, p2 });
        }
        
        public override bool Contains(Point point)
        {
            if (p1.X == p2.X && p1.Y == p2.Y) return false;
            
            Point p3 = new Point((p1.X + p2.X) / 2, p1.Y);
            Point[] curvePoints = { p1, p3, p2 };
            
            const int tolerance = 5;
            
            for (int i = 0; i < curvePoints.Length - 1; i++)
            {
                Point start = curvePoints[i];
                Point end = curvePoints[i + 1];
                
                double lineLength = Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2));
                if (lineLength == 0) continue;
                
                double distance = Math.Abs((end.X - start.X) * (start.Y - point.Y) - (start.X - point.X) * (end.Y - start.Y)) / lineLength;
                
                if (distance <= tolerance)
                {
                    double dotProduct = ((point.X - start.X) * (end.X - start.X) + (point.Y - start.Y) * (end.Y - start.Y)) / (lineLength * lineLength);
                    if (dotProduct >= 0 && dotProduct <= 1)
                        return true;
                }
            }
            
            
            return false;
        }
        
        public override Rectangle Bounds
        {
            get
            {
                int minX = Math.Min(Math.Min(p1.X, p2.X), (p1.X + p2.X) / 2);
                int minY = Math.Min(p1.Y, p2.Y);
                int maxX = Math.Max(Math.Max(p1.X, p2.X), (p1.X + p2.X) / 2);
                int maxY = Math.Max(p1.Y, p2.Y);
                
                return new Rectangle(minX, minY, maxX - minX, maxY - minY);
            }
        }
    }

    public class clsPolygon : clsDrawObject
    {
        public override void Draw(Graphics myGp)
        {
            if (p1.X == p2.X || p1.Y == p2.Y) return;
            
            Point[] hexagonPoints = GetPolygonPoints();
            
            if (isFill)
            {
                myGp.FillPolygon(myBrush, hexagonPoints);
            }
            myGp.DrawPolygon(myPen, hexagonPoints);
        }
        
        private Point[] GetPolygonPoints()
        {
            int minX = Math.Min(p1.X, p2.X);
            int minY = Math.Min(p1.Y, p2.Y);
            int width = Math.Abs(p2.X - p1.X);
            int height = Math.Abs(p2.Y - p1.Y);
            
            return new Point[] {
                new Point(minX + width / 4, minY),
                new Point(minX + 3 * width / 4, minY),
                new Point(minX + width, minY + height / 2),
                new Point(minX + 3 * width / 4, minY + height),
                new Point(minX + width / 4, minY + height),
                new Point(minX, minY + height / 2)
            };
        }
        
        public override bool Contains(Point point)
        {
            if (p1.X == p2.X || p1.Y == p2.Y) return false;
            
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddPolygon(GetPolygonPoints());
                return path.IsVisible(point);
            }
        }
        
        public override Rectangle Bounds
        {
            get
            {
                if (p1.X == p2.X || p1.Y == p2.Y)
                    return new Rectangle(p1.X, p1.Y, 1, 1);
                
                int minX = Math.Min(p1.X, p2.X);
                int minY = Math.Min(p1.Y, p2.Y);
                int maxX = Math.Max(p1.X, p2.X);
                int maxY = Math.Max(p1.Y, p2.Y);
                
                return new Rectangle(minX, minY, maxX - minX, maxY - minY);
            }
        }
    }

    public class clsTriangle : clsDrawObject
    {
        public override void Draw(Graphics myGp)
        {
            if (p1.X == p2.X || p1.Y == p2.Y) return;
            
            Point[] trianglePoints = GetTrianglePoints();
            
            if (isFill)
            {
                myGp.FillPolygon(myBrush, trianglePoints);
            }
            myGp.DrawPolygon(myPen, trianglePoints);
        }
        
        private Point[] GetTrianglePoints()
        {
            int minX = Math.Min(p1.X, p2.X);
            int minY = Math.Min(p1.Y, p2.Y);
            int maxX = Math.Max(p1.X, p2.X);
            int maxY = Math.Max(p1.Y, p2.Y);
            
            return new Point[] {
                new Point(minX, minY),
                new Point(maxX, maxY),
                new Point(minX, maxY)
            };
        }
        
        public override bool Contains(Point point)
        {
            if (p1.X == p2.X || p1.Y == p2.Y) return false;
            
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddPolygon(GetTrianglePoints());
                return path.IsVisible(point);
            }
        }
        
        public override Rectangle Bounds
        {
            get
            {
                if (p1.X == p2.X || p1.Y == p2.Y)
                    return new Rectangle(p1.X, p1.Y, 1, 1);
                
                int minX = Math.Min(p1.X, p2.X);
                int minY = Math.Min(p1.Y, p2.Y);
                int maxX = Math.Max(p1.X, p2.X);
                int maxY = Math.Max(p1.Y, p2.Y);
                
                return new Rectangle(minX, minY, maxX - minX, maxY - minY);
            }
        }
    }
}
