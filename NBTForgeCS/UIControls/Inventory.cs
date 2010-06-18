﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MineEdit
{
    public partial class Inventory : UserControl
    {
        public Dictionary<int, InventoryItem> Stuff = new Dictionary<int, InventoryItem>();

        private IMapHandler _Map;

        public int Capacity
        {
            get
            {
                if (_Map == null) return 0;
                return _Map.InventoryCapacity;
            }
        }
        public int NumInHand
        {
            get
            {
                if (_Map == null) return 0;
                return _Map.InventoryOnHandCapacity;
            }
        }
        public int Columns
        {
            get
            {
                if (_Map == null) return 0;
                return _Map.InventoryColumns;
            }
        }
        public IMapHandler Map
        {
            get { return _Map; }
            set
            {
                _Map = value;
                ReadFromMap();
                Draw();
            }
        }

        public void Save()
        {
            if (_Map == null) return;

            _Map.ClearInventory();
            for (int i = 0; i < Map.InventoryCapacity; i++)
            {
                InventoryItem item = Stuff[i];
                if (item.MyType > 0 && item.Count > 0)
                {
                    _Map.SetInventory(i, item.MyType, item.Damage, item.Count);
                }
            }
        }

        private void ReadFromMap()
        {
            if (_Map == null) return;
            cmbType.Items.Clear();
            cmbType.ValueMember = "ID";
            cmbType.DisplayMember = "Name";
            foreach(KeyValuePair<short,Block> k in Blocks.BlockList)
            {
                cmbType.Items.Add(k.Value);
            }
            this.Width = (Columns * 32) + 12;
            this.Height = ((Capacity / Columns) * 32) + 12;
            Console.WriteLine("Inventory set to ({0},{1}).",Width,Height);
            Stuff.Clear();
            for (int i = 0; i < Map.InventoryCapacity; i++)
            {
                short id;
                int dmg;
                int count;
                string failreason;
                if (_Map.GetInventory(i, out id, out dmg, out count, out failreason))
                {
                    InventoryItem inv = new InventoryItem(id, dmg, count);
                    inv.Render();
                    inv.Click += new EventHandler(inv_Click);
                    this.splitInv.Panel1.Controls.Add(inv);
                    Console.WriteLine("[Inventory] Adding #{0} - {1} {2} @ {3} damage", i, inv.Count, inv.Name, inv.Damage);
                    Stuff.Add(i, inv);
                }
                else
                {
                    InventoryItem inv = new InventoryItem(0, 0, 1);
                    inv.Render();
                    inv.Click += new EventHandler(inv_Click);
                    this.splitInv.Panel1.Controls.Add(inv);
                    Stuff.Add(i, inv);
                    Console.WriteLine("[Inventory] Failed to add #{0} - {1}", i, failreason);
                }
            }

            DoLayout();
            Refresh();
        }

        void inv_Click(object sender, EventArgs e)
        {
            InventoryItem inv = (InventoryItem)sender;
            inv.Selected = !inv.Selected;
            inv.Render(); 
            inv.Refresh();
            cmbType.SelectedItem = Blocks.Get(inv.MyType);
            numCount.Value=inv.Count;
            numDamage.Value=inv.Damage;
        }

        private void Draw()
        {
            if (_Map == null) return;
            /*
            Bitmap back=DrawBackpack();
            Bitmap onhand = DrawOnHand();
            Rendered = new Bitmap(this.Width,this.Height);
            
            Graphics g = Graphics.FromImage(Rendered);
            g.DrawImage(back, 6, 6);
            g.DrawImage(onhand, 6, back.Height + 9);*/
        }

        private void DoLayout()
        {
            int c = Columns;
            Console.WriteLine("C: {0}, H: {1}, Capacity: {2}", Columns, NumInHand, Capacity);
            int r = ((Capacity - NumInHand) / Columns);
            /*
             [ | | | | | ]
             [ | | | | | ]
             [ | | | | | ]
             
             [ | | | | | ] */
            for (int i = 0; i < NumInHand; i++)
            {
                int x = ((i % Columns) * 32) + 6;
                int y = (((Capacity / NumInHand) - 1) * 32) + 9;
                if (Stuff.ContainsKey(i))
                {
                    Stuff[i].SetBounds(x, y, 32, 32);
                }
            }
            for (int i = 0; i < Capacity - NumInHand; i++)
            {
                int x = ((i % Columns) * 32) + 6;
                int y = ((i / Columns) * 32) + 6;
                if (Stuff.ContainsKey(i))
                {
                    Stuff[i + NumInHand].SetBounds(x, y, 32, 32);
                }
            }
        }

        private void LayOutOnHand()
        {
            int c = Columns;
            Console.WriteLine("C: {0}, H: {1}, Capacity: {2}", Columns, NumInHand, Capacity);
            int r = ((Capacity - NumInHand) / Columns);
            for (int x = 0; x < c; x++)
            {
                int i = x;
                if (Stuff[i] != null)
                {
                    Stuff[i].SetBounds(6 + (x * 32), (4 * 32)+ 9, 32, 32);
                }
            }
        }
        private Bitmap DrawBackpack()
        {
            int c = Columns;
            Console.WriteLine("C: {0}, H: {1}, Capacity: {2}", Columns, NumInHand, Capacity);
            int r = ((Capacity - NumInHand) / Columns);
            Bitmap b = new Bitmap(32 * c, 32 * r);
            Graphics g = Graphics.FromImage(b);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            for (int y = 0; y < r; y++)
            {
                for (int x = 0; x < c; x++)
                {
                    int i = NumInHand+x + (y * r);
                    if (Stuff.ContainsKey(i) && Stuff[i].Icon != null)
                    {
                        Stuff[i].Render();
                        g.DrawImage(Stuff[i].Icon, x * 32, y * 32, 32, 32);
                        g.DrawLine(Pens.Red, x * 32, y * 32, x * 32, (y * 32) + 32);
                        g.DrawLine(Pens.Red, x * 32, y * 32, (x * 32) + 32, y * 32);
                        g.DrawLine(Pens.Red, (x * 32) + 32, (y * 32) + 32, x * 32, (y * 32) + 32);
                        g.DrawLine(Pens.Red, (x * 32) + 32, (y * 32) + 32, (x * 32) + 32, y * 32);
                    }
                    else
                    {
                        Console.WriteLine("Couldn't find slot #{0}.", i);
                        g.DrawLine(Pens.DarkGray, x * 32, y * 32, x * 32, (y * 32) + 32);
                        g.DrawLine(Pens.DarkGray, x * 32, y * 32, (x * 32) + 32, y * 32);
                        g.DrawLine(Pens.LightGray, (x * 32) + 32, (y * 32) + 32, x * 32, (y * 32) + 32);
                        g.DrawLine(Pens.LightGray, (x * 32) + 32, (y * 32) + 32, (x * 32) + 32, y * 32);
                    }
                }
            }
            g.Dispose();
            return b;
        }

        private Bitmap DrawOnHand()
        {
            int c = Columns;
            int r = 1;
            Bitmap b = new Bitmap(16 * 2 * c, 16 * 2 * r);
            Graphics g = Graphics.FromImage(b);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            for (int y = 0; y < r; y++)
            {
                for (int x = 0; x < c; x++)
                {
                    int i = x + (y * r);
                    if (Stuff.ContainsKey(i))
                    {
                        Stuff[i].Render();
                        if (Stuff[i].Icon != null)
                            g.DrawImage(Stuff[i].Icon, x * 32, y * 32, 32, 32);
                        else
                            g.DrawImage(Blocks.Get(0x15).Image, x * 32, y * 32, 32, 32);
                    }
                    else
                    {
                        g.DrawLine(Pens.DarkGray, x * 32, y * 32, x * 32, (y * 32) + 32);
                        g.DrawLine(Pens.DarkGray, x * 32, y * 32, (x * 32) + 32, y * 32);
                        g.DrawLine(Pens.LightGray, (x * 32) + 32, (y * 32) + 32, x * 32, (y * 32) + 32);
                        g.DrawLine(Pens.LightGray, (x * 32) + 32, (y * 32) + 32, (x * 32) + 32, y * 32);
                    }
                }
            }
            g.Dispose();
            return b;
        }
        public Inventory()
        {
            InitializeComponent();
            // 9*16+12
            this.Width = 156 * 2;
            this.Height = 76 * 2;
            Rendered = new Bitmap(this.Width, this.Height);
        }
        Bitmap Rendered;
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            /* Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            g.DrawImage(Rendered,0,0);*/
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for(int i = 0;i<Stuff.Count;i++)
            {
                if (Stuff.ContainsKey(i) && Stuff[i].Selected)
                {
                    if(cmbType.SelectedItem!=null)
                        Stuff[i].MyType = (cmbType.SelectedItem as Block).ID;
                    Stuff[i].Count = (int)numCount.Value;
                    Stuff[i].Damage = (int)numDamage.Value;
                    Stuff[i].Render();
                    Stuff[i].Refresh();
                }
            }
            SelectAll(false);
            Save();
        }

        private void SelectAll(bool on)
        {
            for (int i = 0; i < Stuff.Count; i++)
            {
                if (Stuff.ContainsKey(i) && Stuff[i].MyType!=0)
                {
                    Stuff[i].Selected = on;
                    Stuff[i].Render();
                    Stuff[i].Refresh();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Stuff.Count; i++)
            {
                if (Stuff.ContainsKey(i) && Stuff[i].Selected)
                {
                    Stuff[i].Damage = 0;
                    Stuff[i].Render();
                    Stuff[i].Refresh();
                }
            }
            SelectAll(false);
            Save();
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


    }
}
