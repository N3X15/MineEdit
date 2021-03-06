﻿/**
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
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using OpenMinecraft;

namespace MineEdit
{
	public class InventoryItemControl:Control
	{
		public delegate void ChangedHandler();
		public event ChangedHandler Changed;

		private string _Name="";
        private InventoryItem Item;


        public Image Img;
        public Bitmap Icon;
        public bool Selected = false;
        public byte Slot { get; set; }
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
            Font f = new Font(Font,FontStyle.Bold);
			Icon = new Bitmap(32, 32);
			Graphics g = Graphics.FromImage(Icon);
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            //g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
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
            StringFormat upperleft = new StringFormat();
            upperleft.Alignment = StringAlignment.Near; // Left
            upperleft.LineAlignment = StringAlignment.Near; // Left

            StringFormat lowerright = new StringFormat();
            lowerright.Alignment = StringAlignment.Far; // Right
            lowerright.LineAlignment = StringAlignment.Far; // Right

			// Item count
			if (Count > 1)
            {
                SizeF size = g.MeasureString(Count.ToString(), f);
                g.DrawString(Count.ToString(), f, Brushes.Black, 31 - (int)size.Width, 31 - (int)size.Height);
                g.DrawString(Count.ToString(), f, Brushes.White, 30 - (int)size.Width, 30 - (int)size.Height);
                //g.DrawString(Count.ToString(), f, Brushes.Black, new Rectangle(0, 0, DisplayRectangle.Width, DisplayRectangle.Width), lowerright);
                //g.DrawString(Count.ToString(), f, Brushes.White, new Rectangle(1, 1, DisplayRectangle.Width - 2, DisplayRectangle.Width - 2), lowerright);
			}
			// Item damage
			if (Damage > 0)
			{
                // TODO: damage bar
                g.DrawString(Damage.ToString(), f, Brushes.Black, 2,2);
                g.DrawString(Damage.ToString(), f, Brushes.Red, 1,1);
			}
            g.Dispose();
            Invalidate();
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
            //this.Inventory.Changed += new InventoryCollection.InventoryChangedDelegate(inventoryCollection_Changed);
            this.AllowDrop = true;
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.InventoryItem_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.InventoryItem_DragEnter);
            this.DragOver += new DragEventHandler(InventoryItemControl_DragOver);
            this.MouseDown += new MouseEventHandler(InventoryItem_MouseDown);
            this.MouseUp += new MouseEventHandler(InventoryItemControl_MouseUp);
            this.MouseMove += new MouseEventHandler(InventoryItemControl_MouseMove);
			Changed += Render;
            Render();
		}

        void InventoryItemControl_DragOver(object sender, DragEventArgs e)
        {
            // Set the effect based upon the KeyState.
            if ((e.KeyState & (8 + 32)) == (8 + 32) &&
                (e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link)
            {
                // KeyState 8 + 32 = CTL + ALT

                // Link drag-and-drop effect.
                e.Effect = DragDropEffects.Link;

            }
            else if ((e.KeyState & 32) == 32 &&
              (e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link)
            {

                // ALT KeyState for link.
                e.Effect = DragDropEffects.Link;

            }
            else if ((e.KeyState & 4) == 4 &&
              (e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {

                // SHIFT KeyState for move.
                e.Effect = DragDropEffects.Move;

            }
            else if ((e.KeyState & 8) == 8 &&
              (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {

                // CTL KeyState for copy.
                e.Effect = DragDropEffects.Copy;

            }
            else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {

                // By default, the drop action should be move, if allowed.
                e.Effect = DragDropEffects.Move;

            }
            else
                e.Effect = DragDropEffects.None;

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
			this.Inventory = inventoryCollection;
			this.Inventory.Changed += new InventoryCollection.InventoryChangedDelegate(inventoryCollection_Changed);
			this.AllowDrop = true;
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.InventoryItem_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.InventoryItem_DragEnter);
			this.MouseDown += new MouseEventHandler(InventoryItem_MouseDown);
            this.MouseUp += new MouseEventHandler(InventoryItemControl_MouseUp);
            this.MouseMove += new MouseEventHandler(InventoryItemControl_MouseMove);
			Changed += Render;
			Render();
		}

        void InventoryItemControl_MouseUp(object sender, MouseEventArgs e)
        {
            dragBoxFromMouseDown = Rectangle.Empty;
        }

        void InventoryItemControl_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left) 
            {
                // If the mouse moves outside the rectangle, start the drag.
                if (dragBoxFromMouseDown != Rectangle.Empty &&
                    !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    // The screenOffset is used to account for any desktop bands 
                    // that may be at the top or left side of the screen when 
                    // determining when to cancel the drag drop operation.
                    screenOffset = SystemInformation.WorkingArea.Location;

                    DragDropEffects dde = DoDragDrop(this, DragDropEffects.All | DragDropEffects.Link);
                    if (dde == DragDropEffects.Move)
                        Empty();
                }
            }
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
			}
		}
		private OpenMinecraft.InventoryCollection Inventory;
        private  Rectangle dragBoxFromMouseDown;
        private Point screenOffset;

		void InventoryItem_MouseDown(object sender, MouseEventArgs e)
        {            
            // Remember the point where the mouse down occurred. The DragSize indicates
            // the size that the mouse can move before a drag event should be started.                
            Size dragSize = SystemInformation.DragSize;

            // Create a rectangle using the DragSize, with the mouse position being
            // at the center of the rectangle.
            dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2),
                                                            e.Y - (dragSize.Height / 2)), dragSize);

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
	}
}
