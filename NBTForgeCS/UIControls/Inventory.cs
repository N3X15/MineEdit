/**
 * Copyright (c) 2010, Rob "N3X15" Nelson <nexis@7chan.org>
 *  All rights reserved.
 *
 *  Redistribution and use in source and binary forms, with or without 
 *  modification, are permitted provided that the following conditions are met:
 *
 *    * Redistributions of source code must retain the above copyright notice, 
 *      this list of conditions and the following disclaimer.
 *    * Redistributions in binary form must reproduce the above copyright 
 *      notice, this list of conditions and the following disclaimer in the 
 *      documentation and/or other materials provided with the distribution.
 *    * Neither the name of MineEdit nor the names of its contributors 
 *      may be used to endorse or promote products derived from this software 
 *      without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
 * IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, 
 * OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF 
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE 
 * OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OpenMinecraft;

namespace MineEdit
{
    public partial class Inventory : UserControl
    {
        public Dictionary<int, InventoryItemControl> Stuff = new Dictionary<int, InventoryItemControl>();

        private IMapHandler _Map;
        private int ArmorOffset;

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
            for (int i = 0; i < Stuff.Count; i++)
            {
                InventoryItemControl item = Stuff[i];
                if (item.MyType > 0 && item.Count > 0)
                {
                    if (i > Map.InventoryCapacity - 1)
                    {
                        _Map.SetArmor((Enum.GetValues(typeof(ArmorType)) as ArmorType[])[i - ArmorOffset], item.MyType, item.Damage, item.Count);
                    }else{
                        _Map.SetInventory(i, item.MyType, item.Damage, item.Count);
                    }
                }
            }
        }

        public void Reload()
        {
            ClearInventory();
            ReadFromMap();
        }

        private void ClearInventory()
        {
            foreach (KeyValuePair<int, InventoryItemControl> kvp in Stuff)
            {
                Controls.Remove(kvp.Value);
                kvp.Value.Dispose();
            }
            Stuff.Clear();
        }

        private void ReadFromMap()
        {
            if (this.InvokeRequired)
            {
                MethodInvoker del = delegate { ReadFromMap(); };
                Invoke(del);
                return;
            }
            if (_Map == null) return;
            cmbType.Items.Clear();
            cmbType.ValueMember = "ID";
            cmbType.DisplayMember = "Name";
            foreach(KeyValuePair<short,Block> k in Blocks.BlockList)
            {
                cmbType.Items.Add(k.Value);
            }
            this.splitInv.SplitterDistance= (Columns * 32) + 47;
            Console.WriteLine("Inventory set to ({0},{1}).",Width,Height);
            Stuff.Clear();
            for (int i = 0; i < Map.InventoryCapacity; i++)
            {
                short id;
                short dmg;
                byte count;
                string failreason;
                if (_Map.GetInventory(i, out id, out dmg, out count, out failreason))
                {
                    InventoryItemControl inv = new InventoryItemControl(id, dmg, count);
                    inv.Render();
                    inv.Click += new EventHandler(inv_Click);
                    this.splitInv.Panel1.Controls.Add(inv);
                    Console.WriteLine("[Inventory] Adding #{0} - {1} {2} @ {3} damage", i, inv.Count, inv.Name, inv.Damage);
                    Stuff.Add(i, inv);
                }
                else
                {
                    InventoryItemControl inv = new InventoryItemControl(0, 0, 1);
                    inv.Render();
                    inv.Click += new EventHandler(inv_Click);
                    this.splitInv.Panel1.Controls.Add(inv);
                    Stuff.Add(i, inv);
                    Console.WriteLine("[Inventory] Failed to add #{0} - {1}", i, failreason);
                }
            }
            int invc =ArmorOffset= Stuff.Count;
            for (int i = 0; i < 4; i++)
            {
                short id;
                short dmg;
                byte count;
                string failreason;
                ArmorType at = (Enum.GetValues(typeof(ArmorType)) as ArmorType[])[i];
                if (_Map.GetArmor(at, out id, out dmg, out count, out failreason))
                {
                    InventoryItemControl inv = new InventoryItemControl(id, dmg, count);
                    inv.Render();
                    inv.Click += new EventHandler(inv_Click);
                    inv.MouseDown += new MouseEventHandler(inv_MouseDown);
                    inv.MouseUp += new MouseEventHandler(inv_MouseUp);
                    inv.MouseLeave += new EventHandler(inv_MouseLeave);
                    this.splitInv.Panel1.Controls.Add(inv);
                    Console.WriteLine("[Inventory] Adding #{0} - {1} {2} @ {3} damage", i, inv.Count, inv.Name, inv.Damage);
                    Stuff.Add(invc+i, inv);
                }
                else
                {
                    InventoryItemControl inv = new InventoryItemControl(0, 0, 1);
                    inv.Render();
                    inv.Click += new EventHandler(inv_Click);
                    inv.MouseDown += new MouseEventHandler(inv_MouseDown);
                    inv.MouseUp += new MouseEventHandler(inv_MouseUp);
                    inv.MouseLeave += new EventHandler(inv_MouseLeave);
                    this.splitInv.Panel1.Controls.Add(inv);
                    Stuff.Add(invc+i, inv);
                    Console.WriteLine("[Inventory] Failed to add #{0} - {1}", i, failreason);
                }
            }

            DoLayout();
            Refresh();
        }

        void inv_MouseLeave(object sender, EventArgs e)
        {
            if (MouseIsDown)
            {
                (sender as InventoryItemControl).DoDragDrop(sender, DragDropEffects.All);
                (sender as InventoryItemControl).Count = 0;
                (sender as InventoryItemControl).MyType = 0x00;
                (sender as InventoryItemControl).Damage = 0;
                (sender as InventoryItemControl).Refresh();
            }
        }

        void inv_MouseUp(object sender, MouseEventArgs e)
        {
            MouseIsDown = false;
        }

        bool MouseIsDown = false;

        void inv_MouseDown(object sender, MouseEventArgs e)
        {
            MouseIsDown = true;
        }

        void inv_Click(object sender, EventArgs e)
        {
            InventoryItemControl inv = (InventoryItemControl)sender;
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
            for (int i = 0; i < 4; i++)
            {
                int x = 6;
                int y = (i * 32) + 38;
                if (Stuff.ContainsKey(i+ArmorOffset))
                {
                    Stuff[i+ArmorOffset].SetBounds(x, y, 32, 32);
                }
            }
            for (int i = 0; i < NumInHand; i++)
            {
                int x = ((i % Columns) * 32) + 9 + 32;
                int y = (((Capacity / NumInHand) - 1) * 32) + 41;
                if (Stuff.ContainsKey(i))
                {
                    Stuff[i].SetBounds(x, y, 32, 32);
                }
            }
            for (int i = 0; i < Capacity - NumInHand; i++)
            {
                int x = ((i % Columns) * 32) + 9 + 32;
                int y = ((i / Columns) * 32) + 38;
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
                    Stuff[i].SetBounds(6 + (x * 32), (4 * 32) + 9, 32, 32);
                }
            }
        }
        private void LayoutArmor()
        {
            int r = ((Capacity - NumInHand) / Columns);
            for (int y=0; y < 4; y++)
            {
                int i = y+ArmorOffset;
                if (Stuff[i] != null)
                {
                    Stuff[i].SetBounds(6, (y * 32) + 9, 32, 32);
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitInv = new System.Windows.Forms.SplitContainer();
            this.gbEdit = new System.Windows.Forms.GroupBox();
            this.cmdDeleteInv = new System.Windows.Forms.Button();
            this.numDamage = new System.Windows.Forms.NumericUpDown();
            this.cmdApply2Selected = new System.Windows.Forms.Button();
            this.cmdSuperRepair = new System.Windows.Forms.Button();
            this.cmdRepair = new System.Windows.Forms.Button();
            this.lblDamage = new System.Windows.Forms.Label();
            this.numCount = new System.Windows.Forms.NumericUpDown();
            this.lblCount = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.lblObjType = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbSaveTemplate = new System.Windows.Forms.ToolStripButton();
            this.tsbOpenTemplate = new System.Windows.Forms.ToolStripButton();
            this.splitInv.Panel2.SuspendLayout();
            this.splitInv.SuspendLayout();
            this.gbEdit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDamage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitInv
            // 
            this.splitInv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitInv.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitInv.Location = new System.Drawing.Point(0, 0);
            this.splitInv.Name = "splitInv";
            // 
            // splitInv.Panel1
            // 
            this.splitInv.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitInv_Panel1_Paint);
            this.splitInv.Panel1MinSize = 300;
            // 
            // splitInv.Panel2
            // 
            this.splitInv.Panel2.Controls.Add(this.gbEdit);
            this.splitInv.Size = new System.Drawing.Size(552, 229);
            this.splitInv.SplitterDistance = 300;
            this.splitInv.TabIndex = 0;
            // 
            // gbEdit
            // 
            this.gbEdit.Controls.Add(this.cmdDeleteInv);
            this.gbEdit.Controls.Add(this.numDamage);
            this.gbEdit.Controls.Add(this.cmdApply2Selected);
            this.gbEdit.Controls.Add(this.cmdSuperRepair);
            this.gbEdit.Controls.Add(this.cmdRepair);
            this.gbEdit.Controls.Add(this.lblDamage);
            this.gbEdit.Controls.Add(this.numCount);
            this.gbEdit.Controls.Add(this.lblCount);
            this.gbEdit.Controls.Add(this.cmbType);
            this.gbEdit.Controls.Add(this.lblObjType);
            this.gbEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbEdit.Location = new System.Drawing.Point(0, 0);
            this.gbEdit.Name = "gbEdit";
            this.gbEdit.Size = new System.Drawing.Size(248, 229);
            this.gbEdit.TabIndex = 0;
            this.gbEdit.TabStop = false;
            this.gbEdit.Text = "Inventory Editor";
            // 
            // cmdDeleteInv
            // 
            this.cmdDeleteInv.Location = new System.Drawing.Point(80, 194);
            this.cmdDeleteInv.Name = "cmdDeleteInv";
            this.cmdDeleteInv.Size = new System.Drawing.Size(120, 23);
            this.cmdDeleteInv.TabIndex = 11;
            this.cmdDeleteInv.Text = "Delete Selected";
            this.cmdDeleteInv.UseVisualStyleBackColor = true;
            this.cmdDeleteInv.Click += new System.EventHandler(this.cmdDeleteInv_Click);
            // 
            // numDamage
            // 
            this.numDamage.Location = new System.Drawing.Point(80, 81);
            this.numDamage.Maximum = new decimal(new int[] {
            32678,
            0,
            0,
            0});
            this.numDamage.Minimum = new decimal(new int[] {
            32678,
            0,
            0,
            -2147483648});
            this.numDamage.Name = "numDamage";
            this.numDamage.Size = new System.Drawing.Size(120, 20);
            this.numDamage.TabIndex = 10;
            this.numDamage.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cmdApply2Selected
            // 
            this.cmdApply2Selected.Location = new System.Drawing.Point(80, 107);
            this.cmdApply2Selected.Name = "cmdApply2Selected";
            this.cmdApply2Selected.Size = new System.Drawing.Size(120, 23);
            this.cmdApply2Selected.TabIndex = 9;
            this.cmdApply2Selected.Text = "Apply to Selected";
            this.cmdApply2Selected.UseVisualStyleBackColor = true;
            this.cmdApply2Selected.Click += new System.EventHandler(this.button2_Click);
            // 
            // cmdSuperRepair
            // 
            this.cmdSuperRepair.Location = new System.Drawing.Point(80, 165);
            this.cmdSuperRepair.Name = "cmdSuperRepair";
            this.cmdSuperRepair.Size = new System.Drawing.Size(120, 23);
            this.cmdSuperRepair.TabIndex = 8;
            this.cmdSuperRepair.Text = "Super Repair";
            this.cmdSuperRepair.UseVisualStyleBackColor = true;
            this.cmdSuperRepair.Click += new System.EventHandler(this.cmdSuperRepair_Click);
            // 
            // cmdRepair
            // 
            this.cmdRepair.Location = new System.Drawing.Point(80, 136);
            this.cmdRepair.Name = "cmdRepair";
            this.cmdRepair.Size = new System.Drawing.Size(120, 23);
            this.cmdRepair.TabIndex = 8;
            this.cmdRepair.Text = "Repair Selected";
            this.cmdRepair.UseVisualStyleBackColor = true;
            this.cmdRepair.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblDamage
            // 
            this.lblDamage.AutoSize = true;
            this.lblDamage.Location = new System.Drawing.Point(24, 83);
            this.lblDamage.Name = "lblDamage";
            this.lblDamage.Size = new System.Drawing.Size(50, 13);
            this.lblDamage.TabIndex = 6;
            this.lblDamage.Text = "Damage:";
            // 
            // numCount
            // 
            this.numCount.Location = new System.Drawing.Point(80, 55);
            this.numCount.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numCount.Name = "numCount";
            this.numCount.Size = new System.Drawing.Size(120, 20);
            this.numCount.TabIndex = 4;
            this.numCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(36, 57);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(38, 13);
            this.lblCount.TabIndex = 3;
            this.lblCount.Text = "Count:";
            // 
            // cmbType
            // 
            this.cmbType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbType.DropDownWidth = 200;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(80, 28);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(162, 21);
            this.cmbType.TabIndex = 2;
            this.cmbType.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cmbType_DrawItem);
            this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
            this.cmbType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbType_KeyDown);
            // 
            // lblObjType
            // 
            this.lblObjType.AutoSize = true;
            this.lblObjType.Location = new System.Drawing.Point(6, 31);
            this.lblObjType.Name = "lblObjType";
            this.lblObjType.Size = new System.Drawing.Size(68, 13);
            this.lblObjType.TabIndex = 0;
            this.lblObjType.Text = "Object Type:";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripSeparator1,
            this.tsbSaveTemplate,
            this.tsbOpenTemplate});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(552, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(89, 22);
            this.toolStripLabel1.Text = "Template Tools";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbSaveTemplate
            // 
            this.tsbSaveTemplate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSaveTemplate.Image = global::MineEdit.Properties.Resources.document_save;
            this.tsbSaveTemplate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSaveTemplate.Name = "tsbSaveTemplate";
            this.tsbSaveTemplate.Size = new System.Drawing.Size(23, 22);
            this.tsbSaveTemplate.Text = "Save Template";
            this.tsbSaveTemplate.Click += new System.EventHandler(this.tsbSaveTemplate_Click);
            // 
            // tsbOpenTemplate
            // 
            this.tsbOpenTemplate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOpenTemplate.Image = global::MineEdit.Properties.Resources.document_open;
            this.tsbOpenTemplate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpenTemplate.Name = "tsbOpenTemplate";
            this.tsbOpenTemplate.Size = new System.Drawing.Size(23, 22);
            this.tsbOpenTemplate.Text = "Open Template";
            this.tsbOpenTemplate.Click += new System.EventHandler(this.tsbOpenTemplate_Click);
            // 
            // Inventory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.splitInv);
            this.Name = "Inventory";
            this.Size = new System.Drawing.Size(552, 229);
            this.splitInv.Panel2.ResumeLayout(false);
            this.splitInv.ResumeLayout(false);
            this.gbEdit.ResumeLayout(false);
            this.gbEdit.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDamage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitInv;
        private System.Windows.Forms.GroupBox gbEdit;
        private System.Windows.Forms.Button cmdApply2Selected;
        private System.Windows.Forms.Button cmdRepair;
        private System.Windows.Forms.Label lblDamage;
        private System.Windows.Forms.NumericUpDown numCount;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label lblObjType;
        private System.Windows.Forms.NumericUpDown numDamage;
        private System.Windows.Forms.Button cmdDeleteInv;
        private System.Windows.Forms.Button cmdSuperRepair;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbSaveTemplate;
        private System.Windows.Forms.ToolStripButton tsbOpenTemplate;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        public Inventory()
        {
            InitializeComponent();
            // 9*16+12
            this.Width = 156 * 2;
            this.Height = 76 * 2;
            Rendered = new Bitmap(this.Width, this.Height);

            numDamage.Minimum = short.MinValue;
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
                    Stuff[i].Count = (byte)numCount.Value;
                    Stuff[i].Damage =  Utils.Clamp((short)numDamage.Value,short.MinValue,(short)9999);
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
        private void cmdDeleteInv_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Stuff.Count; i++)
            {
                if (Stuff.ContainsKey(i) && Stuff[i].Selected)
                {
                    Stuff[i].MyType = 0;
                    Stuff[i].Count = 0;
                    Stuff[i].Damage = 0;
                    Stuff[i].Render();
                    Stuff[i].Refresh();
                }
            }
            SelectAll(false);
            Save();
        }
        private void cmbType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                short id;
                if (short.TryParse(cmbType.Text, out id))
                {
                    if (Blocks.BlockList.ContainsKey(id))
                    {
                        Block b = Blocks.Get(id);
                        cmbType.SelectedItem = b;
                    }
                }
                else
                {
                    Block b = Blocks.Find(cmbType.Text);
                    if (b == null) return;
                    cmbType.SelectedItem = b;
                }
            }
            else
            {
                ToolTip t = new ToolTip();
                t.Show("Enter a block/item ID (0x0A, 10) or the first few characters of a block/item's name and press enter.", this.ParentForm);
            }
        }

        private void cmbType_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle area = e.Bounds;
            Rectangle iconArea = area;
            iconArea.Width = 16;
            if (e.Index >= 0)
            {
                e.DrawBackground();
                Block ent = (Block)cmbType.Items[e.Index];

                // Draw block icon
                g.DrawImage(ent.Image, iconArea);

                // ID.
                string idtxt = string.Format("{0:D3} 0x{0:X3}", ent.ID);
                SizeF idSz = g.MeasureString(idtxt, this.Font);
                Rectangle idArea = area;
                idArea.X = iconArea.Right + 3;
                idArea.Width = (int)idSz.Width + 1;
                g.DrawString(idtxt, this.Font, new SolidBrush(Color.FromArgb(128, e.ForeColor)), idArea);

                // Block name
                SizeF entName = g.MeasureString(ent.ToString(), this.Font);
                Rectangle ctxt = area;
                ctxt.X = idArea.Right + 3;
                ctxt.Width = (int)entName.Width + 1;
                g.DrawString(ent.ToString(), this.Font, new SolidBrush(e.ForeColor), ctxt);
            }
        }

        private void cmdSuperRepair_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < Stuff.Count; i++)
            {
                if (Stuff.ContainsKey(i) && Stuff[i].Selected)
                {
                    Stuff[i].Damage = short.MinValue;
                    Stuff[i].Render();
                    Stuff[i].Refresh();
                }
            }
            SelectAll(false);
            Save();
        }

        private void splitInv_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tsbSaveTemplate_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "MineEdit Inventory Template|*.mit";
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),"templates"));
            sfd.InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(),"templates");
            if (sfd.ShowDialog() == DialogResult.Cancel) return;
            List<string> mit = new List<string>();
            mit.Add("# MINEEDIT INVENTORY TEMPLATE");
            mit.Add("# Don't edit this file or you'll mess it up.  Trust me.");
            for (int i = 0; i < 104;i++ )
            {
                if(Stuff.ContainsKey(i))
                    mit.Add(string.Format("{0}\t{1}\t{2}\t{3}",i,Stuff[i].Count,Stuff[i].MyType,Stuff[i].Damage));
            }
            File.WriteAllLines(sfd.FileName,mit.ToArray());
        }

        private void tsbOpenTemplate_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "MineEdit Inventory Template|*.mit";
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "templates"));
            ofd.InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(), "templates");
            if (ofd.ShowDialog() == DialogResult.Cancel) return;
            if(!File.Exists(ofd.FileName))
                return;
            foreach (string ln in File.ReadAllLines(ofd.FileName))
            {
                if (ln.StartsWith("#")) continue;
                string[] chunks = ln.Split('\t');
                int idx = int.Parse(chunks[0]);
                Stuff[idx].Count = byte.Parse(chunks[1]);
                Stuff[idx].MyType = short.Parse(chunks[2]);
                Stuff[idx].Damage = short.Parse(chunks[3]);
            }
            Refresh();
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}
