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
        bool isSelectionMode = false;

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

        private List<clsDrawObject> selectedObjects = new List<clsDrawObject>();
        private Point offset;
        private bool isMove = false;

        private bool isResizing = false;
        private int resizeHandleIndex = -1;
        private clsDrawObject resizeObject = null;

        bool isPress = false;

        public Form1()
        {
            InitializeComponent();
            gp = this.canvas.CreateGraphics();
            this.KeyPreview = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            curcolorPen.Image = null;
            curcolorBrush.Image = null;
            cbxdrawingstyle.SelectedIndex = 0;
            cbxbrushstyle.SelectedIndex = 0;
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
        }
        private void btnfill_Click(object sender, EventArgs e)
        {
            this.isFill = true;
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

            if (isSelectionMode)
            {
              
                foreach (var selectedObj in selectedObjects) 
                {
                    var handles = selectedObj.HandlePoints;
                    for (int i = 0; i < handles.Length; i++)
                    {
                        var pt = handles[i];
                        var rect = new Rectangle(pt.X - clsDrawObject.HandleSize / 2, pt.Y - clsDrawObject.HandleSize / 2,
                                                 clsDrawObject.HandleSize, clsDrawObject.HandleSize);
                        if (rect.Contains(e.Location))
                        {
                            isResizing = true;
                            resizeObject = selectedObj;
                            resizeHandleIndex = i;
                            return;
                        }
                    }
                }

                clsDrawObject obj = lstObject.FindObjectAt(e.Location);
                bool ctrlPressed = (ModifierKeys & Keys.Control) == Keys.Control;
                
                if (obj != null)
                {
                   
                    if (selectedObjects.Contains(obj) && !ctrlPressed)
                    {
                        
                        isMove = true;
                        offset = new Point(e.X - obj.Bounds.X, e.Y - obj.Bounds.Y);
                        lstObject.BringToFront(obj);
                    }
                    else
                    {
                       
                        if (!ctrlPressed)
                        {
                            
                            if (!selectedObjects.Contains(obj))
                            {
                                foreach (var selected in selectedObjects)
                                {
                                    selected.isSelected = false;
                                }
                                selectedObjects.Clear();
                            }
                        }
                        
          
                        if (selectedObjects.Contains(obj) && ctrlPressed)
                        {
                            obj.isSelected = false;
                            selectedObjects.Remove(obj);
                        }
                        else
                        {
                            obj.isSelected = true;
                            selectedObjects.Add(obj);
                            lstObject.BringToFront(obj);
                        }
                        
                        if (selectedObjects.Count > 0)
                        {
                            isMove = true;
                            offset = new Point(e.X - obj.Bounds.X, e.Y - obj.Bounds.Y);
                        }
                    }
                }
                else if (!ctrlPressed)
                {
                    foreach (var selected in selectedObjects)
                    {
                        selected.isSelected = false;
                    }
                    selectedObjects.Clear();
                    isMove = false;
                }
                
                this.canvas.Invalidate();
                return;
            }

            clsDrawObject drawObject = null;
            
            if (this.bLine) drawObject = new clsLine();
            else if (this.bEllipse) drawObject = new clsEllipse();
            else if (this.bRectangle) drawObject = new clsRectangle();
            else if (this.bSquare) drawObject = new clsSquare();
            else if (this.bCircle) drawObject = new clsCircle();
            else if (this.bCurve) drawObject = new clsCurve();
            else if (this.bPolygon) drawObject = new clsPolygon();
            else if (this.bTriangle) drawObject = new clsTriangle();
            
            if (drawObject != null)
            {
                setobject(drawObject, e);
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.isPress)
            {
                if (isSelectionMode && selectedObjects.Count > 0 && isMove)
                {
                    int deltaX = 0;
                    int deltaY = 0;
                    
                
                    if (selectedObjects.Count > 0)
                    {
                        var firstObj = selectedObjects[0];
                        deltaX = e.X - (firstObj.Bounds.X + offset.X);
                        deltaY = e.Y - (firstObj.Bounds.Y + offset.Y);
                    }
                    
                
                    if (deltaX != 0 || deltaY != 0)
                    {
        
                        foreach (var obj in selectedObjects)
                        {
                            obj.Move(deltaX, deltaY);
                        }
                        this.canvas.Invalidate();
                    }
                    return;
                }

                if (isResizing && resizeObject != null)
                {
                    resizeObject.Resize(resizeHandleIndex, e.Location);
                    this.canvas.Invalidate();
                    return;
                }
                
                if (this.currentObject != null)
                {
                    this.currentObject.p2 = e.Location;
                    this.canvas.Invalidate();
                }
            }
            else if (isSelectionMode)
            {
                clsDrawObject obj = lstObject.FindObjectAt(e.Location);
                if (obj != null)
                {
                    this.canvas.Cursor = Cursors.SizeAll;
                }
                else
                {
                    this.canvas.Cursor = Cursors.Default;
                }
            }
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (isSelectionMode)
            {
                isMove = false;
                isResizing = false;
                resizeObject = null;
                resizeHandleIndex = -1;
                this.isPress = false;
                return;
            }
            
            if (this.currentObject != null)
            {
                this.currentObject = null;
                this.isPress = false;
                this.canvas.Invalidate();
            }
        }

        private void setup() {
            this.bLine = false;
            this.bEllipse = false;
            this.bRectangle = false;
            this.bSquare = false;
            this.bCircle = false;
            this.bCurve = false;
            this.bPolygon = false;
            this.bTriangle = false;
            this.isSelectionMode = false; 
            this.currentObject = null;
            this.Cursor = Cursors.Default;
            
            foreach (var obj in selectedObjects)
            {
                obj.isSelected = false;
            }
            selectedObjects.Clear();
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
            this.isSelectionMode = true;
            this.canvas.Cursor = Cursors.Default;
            this.canvas.Invalidate();
        }

        private void btnerase_Click(object sender, EventArgs e)
        {
            if (selectedObjects.Count > 0)
            {
                foreach (var obj in selectedObjects.ToList())
                {
                    lstObject.RemoveObject(obj);
                }
                selectedObjects.Clear();
                this.canvas.Invalidate();
            }
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

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && isSelectionMode && selectedObjects.Count > 0)
            {

                foreach (var obj in selectedObjects.ToList())
                {
                    lstObject.RemoveObject(obj);
                }

                selectedObjects.Clear();

                this.canvas.Invalidate();

                e.Handled = true;
            }
        }

        private void btn_group_Click(object sender, EventArgs e)
        {
            if (selectedObjects.Count > 1)
            {
                var group = new clsComplexobject();
                foreach (var obj in selectedObjects)
                {
                    lstObject.RemoveObject(obj);
                    obj.isSelected = false;
                    group.AddObject(obj);
                }
                lstObject.AddObject(group);
                selectedObjects.Clear();
                group.isSelected = true;
                selectedObjects.Add(group);
                this.canvas.Invalidate();
            }
        }

        private void btn_ungroup_Click(object sender, EventArgs e)
        {
            if (selectedObjects.Count == 1 && selectedObjects[0] is clsComplexobject group)
            {
                var children = group.GetObjects().ToList();
                lstObject.RemoveObject(group);
                group.isSelected = false;
                selectedObjects.Clear();
                foreach (var obj in children)
                {
                    lstObject.AddObject(obj);
                    obj.isSelected = true;
                    selectedObjects.Add(obj);
                }
                this.canvas.Invalidate();
            }
        }
    }
}
