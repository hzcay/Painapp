using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bunifu.UI.WinForms;
using Bunifu.UI.WinForms.BunifuButton;

namespace Painapp
{
    public partial class Form1 : Form
    {

        Graphics gp;

        Color myPenColor = Color.Red;
        Color myColorBrush = Color.Red;
        int myPenWidth = 1;
        Pen CurPen = new Pen(Color.Red, 2);
        Brush CurBrush = new SolidBrush(Color.Red);
        Color color2 = Color.Blue;


        bool isFill = false;

        bool bLine = false;
        bool bEllipse = false;
        bool bRectangle = false;
        bool bSquare = false;
        bool bCircle = false;
        bool bCurve = false;
        bool bPolygon = false;
        bool bTriangle = false;

        clsComplexobject lstObject = new clsComplexobject();
        private clsDrawObject currentObject = null;

        private clsDrawObject selectedObject = null;
        private Point offset;
        private bool isMove = false;

        bool isPress = false;

        public Form1()
        {
            InitializeComponent();
            gp = this.canvas.CreateGraphics();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            curcolorPen.Image = null;
            curcolorBrush.Image = null;
        }

        private void btnexit_Click(object sender, EventArgs e)
        {
            Form1.ActiveForm.Close();
        }

        
        private void btnthang_Click(object sender, EventArgs e)
        {
            setup();
            this.bLine = true;
        }
        private void btnellipse_Click(object sender, EventArgs e)
        {
            setup();
            this.bEllipse = true;
        }
        private void btnhcn_Click(object sender, EventArgs e)
        {
            setup();
            this.bRectangle = true;
        }
        private void btnhv_Click(object sender, EventArgs e)
        {
            setup();
            this.bSquare = true;
        }
        private void btntron_Click(object sender, EventArgs e)
        {
            setup();
            this.bCircle = true;
        }
        private void btncong_Click(object sender, EventArgs e)
        {
            setup();
            this.bCurve = true;
        }
        private void btndagiac_Click(object sender, EventArgs e)
        {
            setup();
            this.bPolygon = true;
        }
        private void btnPen_Click(object sender, EventArgs e)
        {
            this.isFill = false;
            this.Cursor = new Cursor("D:\\giuaki-winform\\Painapp\\Painapp\\Resources\\pencil.ico");
        }
        private void btnfill_Click(object sender, EventArgs e)
        {
            this.isFill = true;
            this.Cursor = new Cursor("D:\\giuaki-winform\\Painapp\\Painapp\\Resources\\color.ico");
        }
        private void btnTriangle_Click(object sender, EventArgs e)
        {
            setup();
            this.bTriangle = true;
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            lstObject.Draw(g);
        }
            

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            this.isPress = true;

            if (this.bLine) 
            {
                clsLine myLine = new clsLine();
                setobject(myLine, e);
            }
            if (this.bEllipse)
            {
                clsEllipse myEllipse = new clsEllipse();
                setobject(myEllipse, e);
            }
            if (this.bRectangle)
            {
                clsRectangle myRectangle = new clsRectangle();
                setobject(myRectangle, e);
            }
            if (this.bSquare)
            {
                clsSquare mySquare = new clsSquare();
                setobject(mySquare, e);
            }
            if (this.bCircle)
            {
                clsCircle myCircle = new clsCircle();
                setobject(myCircle, e);
            }
            if (this.bCurve)
            {
                clsCurve myCurve = new clsCurve();
                setobject(myCurve, e);
            }
            if (this.bPolygon)
            {
                clsPolygon myPolygon = new clsPolygon();
                setobject(myPolygon, e);
            }
            if (this.bTriangle)
            {
                clsTriangle myTriangle = new clsTriangle();
                setobject(myTriangle, e);
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.isPress && this.currentObject != null)
            {
                if (this.currentObject is clsLine line)
                {
                    line.p2 = e.Location;
                    this.canvas.Invalidate();
                }
                if (this.currentObject is clsEllipse ellipse)
                {
                    ellipse.p2 = e.Location;
                    this.canvas.Invalidate();
                }
                if (this.currentObject is clsRectangle rectangle)
                {
                    rectangle.p2 = e.Location;
                    this.canvas.Invalidate();
                }
                if (this.currentObject is clsSquare square)
                {
                    square.p2 = e.Location;
                    this.canvas.Invalidate();
                }
                if (this.currentObject is clsCircle circle)
                {
                    circle.p2 = e.Location;
                    this.canvas.Invalidate();
                }
                if (this.currentObject is clsCurve curve)
                {
                    curve.p2 = e.Location;
                    this.canvas.Invalidate();
                }
                if (this.currentObject is clsPolygon polygon)
                {
                    polygon.p2 = e.Location;
                    this.canvas.Invalidate();
                }
                if (this.currentObject is clsTriangle triangle)
                {
                    triangle.p2 = e.Location;
                    this.canvas.Invalidate();
                }
            }
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            this.isPress = false;
            this.currentObject = null; 
            this.canvas.Invalidate();
        }

        private void setup() {
            this.isPress = false;
            this.currentObject = null;
            this.bLine = false;
            this.bEllipse = false;
            this.bRectangle = false;
            this.bSquare = false;
            this.bCircle = false;
            this.bCurve = false;
            this.bPolygon = false;
            this.bTriangle = false;
            this.canvas.Invalidate();
        }
        private void setobject(clsDrawObject mydraw, MouseEventArgs e)
        {
            mydraw.myPen = new Pen(myPenColor, myPenWidth);
            mydraw.myPen.DashStyle = CurPen.DashStyle;
            mydraw.myBrush = new SolidBrush(myColorBrush);
            mydraw.myBrush = CurBrush;
            mydraw.p1 = e.Location;
            mydraw.p2 = e.Location;
            mydraw.isFill = this.isFill;
            this.currentObject = mydraw;
            this.lstObject.AddObject(mydraw);
        }
        private void change_color(BunifuImageButton p) 
        {
            if (this.isFill)
            {
                myColorBrush = p.BackColor;
                curcolorBrush.BackColor = myColorBrush;
                colorchanging.BackColor = myColorBrush;
                setcurBrush();
            }
            else
            {
                myPenColor = p.BackColor;
                curcolorPen.BackColor = myPenColor;
                colorchanging.BackColor = myPenColor;

            }
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            change_color(btn1);
        }
      
        private void btn2_Click(object sender, EventArgs e)
        {
            change_color(btn2);
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            change_color(btn3);
        }

        private void btn4_Click(object sender, EventArgs e)
        {
            change_color(btn4);
        }

        private void btn5_Click(object sender, EventArgs e)
        {
            change_color(btn5);
        }

        private void btn6_Click(object sender, EventArgs e)
        {
            change_color(btn6);
        }

        private void btn7_Click(object sender, EventArgs e)
        {
            change_color(btn7);
        }

        private void btn8_Click(object sender, EventArgs e)
        {
            change_color(btn8);
        }

        private void btn9_Click(object sender, EventArgs e)
        {
            change_color(btn9);
        }

        private void btn10_Click(object sender, EventArgs e)
        {
            change_color(btn10);
        }

        private void btn11_Click(object sender, EventArgs e)
        {
            change_color(btn11);
        }

        private void btn12_Click(object sender, EventArgs e)
        {
            change_color(btn12);
        }

        private void brushsize_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
        {
            myPenWidth = brushsize.Value;
            txtBrush.Text = "Pen Size: " +myPenWidth.ToString();
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            setup();
            this.Cursor = Cursors.Default;
        }

        private void btnerase_Click(object sender, EventArgs e)
        {
            
        }

        private void cbxdrawingstyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxdrawingstyle.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn kiểu Draw!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedDraw = cbxdrawingstyle.SelectedItem.ToString();
            switch (selectedDraw)
            {
                case "Dash":
                    CurPen.DashStyle = DashStyle.Dash;
                    break;
                case "DashDot":
                    CurPen.DashStyle = DashStyle.DashDot;
                    break;
                case "DashDotDot":
                    CurPen.DashStyle = DashStyle.DashDotDot;
                    break;
                case "Dot":
                    CurPen.DashStyle = DashStyle.Dot;
                    break;
                case "Solid":
                    CurPen.DashStyle = DashStyle.Solid;
                    break;
            }
        }

        private void setcurBrush()
        {
            CurBrush = new SolidBrush(myColorBrush);
            if (cbxbrushstyle.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn kiểu Brush!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedBrush = cbxbrushstyle.SelectedItem.ToString();

            switch (selectedBrush)
            {

                case "SolidBrush":
                    CurBrush = new SolidBrush(myColorBrush);
                    break;
                case "HatchBrush":
                    if (colorsetting.ShowDialog() == DialogResult.OK )
                    {
                        color2 = colorsetting.Color;
                    } 
                    CurBrush = new HatchBrush(HatchStyle.DiagonalCross, myColorBrush, color2);
                    break;
                case "GradientBrush":
                    if (colorsetting.ShowDialog() == DialogResult.OK )
                    {
                        color2 = colorsetting.Color;
                    } 
                    CurBrush = new LinearGradientBrush(new Rectangle(0, 0, 100, 100), myColorBrush, color2, LinearGradientMode.ForwardDiagonal);
                    break;
            }
        }

        private void cbxbrushstyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            setcurBrush(); 
        }

        private void colorchanging_Click(object sender, EventArgs e)
        {
            if (colorsetting.ShowDialog() == DialogResult.OK)
            {
                if (this.isFill)
                {
                    myColorBrush = colorsetting.Color;
                    curcolorBrush.BackColor = myColorBrush;
                    colorchanging.BackColor = myColorBrush;
                    setcurBrush();
                }
                else
                {
                    myPenColor = colorsetting.Color;
                    curcolorPen.BackColor = myPenColor;
                    colorchanging.BackColor = myPenColor;

                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lstObject.RemoveAll();
            this.canvas.Invalidate();
        }
    }
}
