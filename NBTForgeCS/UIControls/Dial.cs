using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MineEdit.UIControls
{
    public partial class Dial : UserControl
    {
        private double mValue=0d;
        private double lineValue=0d;
        public double Value 
        {
            get 
            {
                return Lerp((mValue/360),Minimum,Maximum);
            }
            set 
            {
                mValue = ((OpenMinecraft.Utils.Clamp(value, Minimum, Maximum) - Minimum) / (Maximum - Minimum))*360;
                Invalidate();
            }
        }
        [DefaultValue(0d)]
        public double Minimum { get; set; }
        [DefaultValue(1d)]
        public double Maximum { get; set; }
        public string Label { get; set; }

        private PointF mCenter;
        public Dial()
        {
            InitializeComponent();
            Minimum = 0;
            Maximum = 1;
        }

        private double Lerp(double frac, double a, double b)
        {
            return a + (b - a) * frac;
        }

        private PointF ConvertToPoint(PointF offset, double angle, double radius)
        {
	        double radians = angle / (180.0 / 3.141952);
	        float x = offset.X + (float)((double)radius * Math.Cos(radians));
	        float y = offset.Y + (float)((double)radius * Math.Sin(radians));
	        PointF point = new PointF(x,y);
	        return point;
        }

        protected override void OnPaint(PaintEventArgs e) 
        {
            Graphics g = e.Graphics;

            bool DrawText = e.ClipRectangle.Height == e.ClipRectangle.Width && e.ClipRectangle.Width >= 64;
            Pen circlecolor = new Pen(Focused ? Color.Orange : SystemColors.ControlText,2f);
            // Draw dial radius
            g.DrawEllipse(circlecolor, 2, 2, e.ClipRectangle.Width - 5, e.ClipRectangle.Width - 5);
            //g.DrawEllipse(Pens.Aqua, 4, 4, e.ClipRectangle.Width - 8, e.ClipRectangle.Width - 8);
            //g.DrawEllipse(Pens.Orange, 1, 1, e.ClipRectangle.Width - 2, e.ClipRectangle.Width - 2);
            
            // Draw line at value

            mCenter = new PointF((ClientRectangle.Width / 2) - 1, (ClientRectangle.Width / 2) - 1);
            DrawAngle(ref g,lineValue, Pens.Red);
            DrawAngle(ref g,mValue,circlecolor);

            // Draw Text
            StringFormat centerme = new StringFormat();
            centerme.Alignment = StringAlignment.Center;
            centerme.LineAlignment = StringAlignment.Center;

            string display = mValue.ToString("F")+"\n"+Value.ToString("F"); //string.Format("val:{0}\nmax:{1}", val, max);
            if(!string.IsNullOrEmpty(Label) && DrawText)
                display=Label+"\n"+display;
            g.DrawString(display, this.Font,SystemBrushes.ControlText,e.ClipRectangle,centerme);
            //g.Dispose();
        }

        private void DrawAngle(ref Graphics g, double ang,Pen c)
        {
            PointF inner = ConvertToPoint(mCenter, ang, (double)((ClientRectangle.Width / 2) - 6));
            PointF outer = ConvertToPoint(mCenter, ang, (double)((ClientRectangle.Width / 2)));

            g.DrawLine(c, inner, outer);
        }

        private void Dial_MouseDown(object sender, MouseEventArgs e)
        {
            // get mouse position
            double xpos = (double)e.X;
            double ypos = (double)e.Y;
            // calculate the center of the dial
            double xcenter = ClientRectangle.Width / 2;
            double ycenter = ClientRectangle.Width / 2;
            // subtract the center from the actual position to get the relative position to the midpoint
            xpos -= xcenter;
            ypos -= ycenter;

            double max = Maximum-Minimum;

            mValue = Math.Atan2(ypos, xpos) * (180.0 / Math.PI);
            mValue += mValue < 0 ? 360 : 0;
            //Invalidate();
        }
        double ConvertToAngle(PointF point, PointF center)
        {
	        //Calculate the position user click relative to the center
	        double x = point.X - center.X;
	        double y = point.Y - center.Y;

	        //Convert xy position to an angle.
	        return Math.Atan2(y, x);
        }

        private void Dial_MouseMove(object sender, MouseEventArgs e)
        {
            // get mouse position
            double xpos = (double)e.X;
            double ypos = (double)e.Y;
            // calculate the center of the dial
            double xcenter = ClientRectangle.Width / 2;
            double ycenter = ClientRectangle.Width / 2;
            // subtract the center from the actual position to get the relative position to the midpoint
            xpos = xpos-xcenter;
            ypos = ypos-ycenter;

            lineValue = Math.Atan2(ypos, xpos) *(180.0 / Math.PI);
            lineValue += lineValue < 0 ? 360 : 0;
            Invalidate();
        }
    }
}
