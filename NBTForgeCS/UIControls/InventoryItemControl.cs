using System;
using System.Collections.Generic;

using System.Text;
using System.Windows.Forms;
using System.Drawing;
using OpenMinecraft;
using OpenMinecraft.TileEntities;
namespace MineEdit
{
    public class InventoryItemControl:Control
    {
        public delegate void ChangedHandler();
        public event ChangedHandler Changed;

        private string _Name="";
        private InventoryItem Item;
        public string MyName 
        { 
            get 
            { 
                return _Name; 
            } 
        }
        public byte Count
        {
            get { return Item.Count; }
            set
            {
                Item.Count = value;
                DoChanged();
            }
        }
        public short Damage
        {
            get { return Item.Damage; }
            set
            {
                Item.Damage = value;
                DoChanged();
            }
        }
        public short MyType
        {
            get { return Item.ID; }
            set
            {
                Item.ID = value;
                Block b = Blocks.Get(Item.ID);
                _Name = b.Name;
                Img = b.Image;
                DoChanged();
            }
        }

        private void DoChanged()
        {
            if (Changed != null) Changed();
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

        protected override void OnPaint(PaintEventArgs e)
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
        public InventoryItemControl(short type,short damage,byte count)
        {
            Block b = Blocks.Get(0x00);
            try
            {
                b = Blocks.Get(type);
            }
            catch (Exception) { }
            Item.Damage = damage;
            Item.Count = count;
            Img = b.Image;
            _Name = b.Name;
            Item.ID = type;
            this.AllowDrop = true;
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.InventoryItem_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.InventoryItem_DragEnter);
            this.MouseDown += new MouseEventHandler(InventoryItem_MouseDown);
            this.KeyDown += new KeyEventHandler(InventoryItem_KeyDown);
            this.KeyUp += new KeyEventHandler(InventoryItem_KeyUp);
            Changed += Render;
        }

        public InventoryItemControl(byte myslot,ref InventoryCollection inventoryCollection)
        {
            this.Slot = myslot;
            if (!inventoryCollection.ContainsKey(myslot))
            {
                Item = new InventoryItem();
                Item.Count = 0;
                Item.Damage = 0;
                Item.ID = 0;
                Item.Slot = myslot;
            } else 
                Item = inventoryCollection[myslot];
            Block b = Blocks.Get(0x00);
            try
            {
                b = Blocks.Get(Item.ID);
            }
            catch (Exception) { }
            Img = b.Image;
            _Name = b.Name;
            Render();
            this.Inventory = inventoryCollection;
            this.Inventory.Changed += new InventoryCollection.InventoryChangedDelegate(inventoryCollection_Changed);
            this.AllowDrop = true;
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.InventoryItem_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.InventoryItem_DragEnter);
            this.MouseDown += new MouseEventHandler(InventoryItem_MouseDown);
            this.KeyDown += new KeyEventHandler(InventoryItem_KeyDown);
            this.KeyUp += new KeyEventHandler(InventoryItem_KeyUp);
            Changed += Render;
        }

        void inventoryCollection_Changed(byte slot)
        {
            if (Slot == 0)
                Console.WriteLine("[{0}] Slot {1} changed.", GetType().Name, slot);
            if (Slot == slot)
            {
                Item = Inventory[slot];
                Block b = Blocks.Get(0x00);
                try
                {
                    b = Blocks.Get(Item.ID);
                }
                catch (Exception) { }
                Img = b.Image;
                _Name = b.Name;
                Render();
                Refresh();
            }
        }
        bool DoCopy = false;
        private OpenMinecraft.InventoryCollection Inventory;
        void InventoryItem_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Shift)
                DoCopy = false;
        }

        void InventoryItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Shift)
                DoCopy = true;
        }

        void InventoryItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                DoDragDrop(this, DragDropEffects.All);
                if(!DoCopy)
                    Empty();
            }
        }

        private void Empty()
        {
            MyType = 0x00;
            Count = 0;
            Damage = 0;
            Render();
            Refresh();
        }


        /// <summary>
        ///  Drag and drop shit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InventoryItem_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(InventoryItemControl)))
            {
                if ((e.KeyState & (int)Keys.Shift) == (int)Keys.Shift)
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.Move;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void InventoryItem_DragDrop(object sender, DragEventArgs e)
        {
            InventoryItemControl old = this;
            InventoryItemControl derp = (InventoryItemControl)e.Data.GetData(typeof(InventoryItemControl));

            _Name = derp.MyName;
            Item.ID = derp.MyType;
            Item.Damage = derp.Damage;
            Item.Count = derp.Count;

            Block b = Blocks.Get(0x00);
            try
            {
                b = Blocks.Get(Item.ID);
            }
            catch (Exception) { }

            Img = b.Image;
            _Name = b.Name;

            Render();

            Refresh();
            Console.WriteLine("Received data, copying.");
        }

        public byte Slot { get; set; }
    }
}
