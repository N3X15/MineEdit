using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace MineEdit
{

    public class InventoryItem:Control
    {
        private string _Name="";
        private int _Count=0;
        private int _Damage = 0;
        private short _Type = 0x00;
        public string MyName 
        { 
            get 
            { 
                return _Name; 
            } 
        }
        public int Count
        {
            get { return _Count; }
            set
            {
                _Count = value;
                Render();
            }
        }
        public int Damage
        {
            get { return _Damage; }
            set
            {
                _Damage = value;
                Render();
            }
        }
        public short MyType
        {
            get { return _Type; }
            set
            {
                _Type = value;
                Block b = Blocks.Get(_Type);
                _Name = b.Name;
                Img = b.Image;
                Render();
            }
        }

        public Image Img;
        public Bitmap Icon;
        public bool Selected=false;

        protected override void OnMouseHover(EventArgs e)
        {
            ToolTip butts = new ToolTip();
            string t = "(Empty)";
            string damage = (Damage>0) ? string.Format(" with {0} damage",Damage) : "";
            if (Count>0 || MyType!=0)
            {
                if(Count > 1)
                    t=string.Format("{0} {1}s (#{2}){3}",Count, MyName, MyType,damage);
                else
                    t=string.Format("{0} {1} (#{2}){3}",Count, MyName, MyType,damage);
            }
            butts.SetToolTip(this, t);
        }

        protected override void  OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.DrawImage(Icon, 0, 0);
        }
        public void Render()
        {
            Icon = new Bitmap(32, 32);
            Graphics g = Graphics.FromImage(Icon);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            // Texture
            if (Selected)
            {
                Brush br = new SolidBrush(Color.FromArgb(128, Color.Orange));
                g.FillRectangle(br, 0, 0, 31, 31);
            }
            if (Img == null)
                Console.WriteLine("[Inventory] Img == null for type 0x{0:X2} (Name: {1})", MyType, MyName);
            else
                g.DrawImage(Img, 0, 0,32,32);

            // Border
            if (!Selected)
            {
                g.DrawLine(Pens.DarkGray, 0, 0, 31, 0);
                g.DrawLine(Pens.DarkGray, 0, 0, 0, 31);
                g.DrawLine(Pens.LightGray, 31, 31, 31, 0);
                g.DrawLine(Pens.LightGray, 31, 31, 0, 31);
            }
            else
            {
                g.DrawLine(Pens.Orange, 0, 0, 31, 0);
                g.DrawLine(Pens.Orange, 0, 0, 0, 31);
                g.DrawLine(Pens.Orange, 31, 31, 31, 0);
                g.DrawLine(Pens.Orange, 31, 31, 0, 31);
            }
            // Item count
            if (Count > 1)
            {
                Font f = new Font(FontFamily.GenericSansSerif, 8);
                SizeF size = g.MeasureString(Count.ToString(), f);
                g.DrawString(Count.ToString(), f, Brushes.Black, 31 - (int)size.Width, 31 - (int)size.Height);
                g.DrawString(Count.ToString(), f, Brushes.White, 30 - (int)size.Width, 30 - (int)size.Height);
            }
            // Item damage
            if (Damage > 0)
            {
                Font f = new Font(FontFamily.GenericSansSerif, 8);
                SizeF size = g.MeasureString(Damage.ToString(), f);
                g.DrawString(Damage.ToString(), f, Brushes.Black, 2,2);
                g.DrawString(Damage.ToString(), f, Brushes.Red, 1,1);
            }
        }
        public InventoryItem(short type,int damage,int count)
        {
            Block b = Blocks.Get(0x00);
            try
            {
                b = Blocks.Get(type);
            }
            catch (Exception) { }
            _Damage = damage;
            _Count = count;
            Img = b.Image;
            _Name = b.Name;
            _Type = type;
        }
    }
}
