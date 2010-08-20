using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OpenMinecraft;
using OpenMinecraft.TileEntities;

namespace MineEdit
{
    public partial class ChestEditor : UserControl,ITileEntityEditor
    {
        private Chest Chest;
        private InventoryItemControl[] InvControls = new InventoryItemControl[54];

        public ChestEditor(TileEntity e)
        {
            Chest = (Chest)e;
            InitializeComponent();
            for(byte i=0;i<54;i++)
            {
                InvControls[i] = new InventoryItemControl(i,ref Chest.Inventory);
                InvControls[i].Click += new EventHandler(inv_Click);
                InvControls[i].Changed += new InventoryItemControl.ChangedHandler(ChestEditor_Changed);
            }
            DoLayout();
            cmbType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cmbType.DrawItem +=new DrawItemEventHandler(cmbType_DrawItem);
            cmbType.Items.Clear();
            cmbType.ValueMember = "ID";
            cmbType.DisplayMember = "Name";
            foreach (KeyValuePair<short, Block> k in Blocks.BlockList)
            {
                cmbType.Items.Add(k.Value);
            }
        }

        void ChestEditor_Changed()
        {
            if(EntityModified!=null)
                EntityModified(this,EventArgs.Empty);
        }

        public void Reload()
        {
            //ClearInventory();
            splitInv.Panel1.Controls.Clear();
            DoLayout();
        }

        private void ClearInventory()
        {
            List<InventoryItemControl> inv = new List<InventoryItemControl>();
            foreach (InventoryItemControl c in Controls)
            {
                inv.Add(c);
            }
            foreach (InventoryItemControl c in inv)
            {
                Controls.Remove(c);
                c.Dispose();
            }
            Chest.Inventory.Clear();
        }


        private void DoLayout()
        {
            // 6x9
            // 54 max
            for (byte i = 0; i < 54; i++)
            {
                int x = ((i % 9) * 32) + 9;
                int y = ((i / 9) * 32) + 9 + 32;
                InvControls[i].Render();
                if(!splitInv.Panel1.Controls.Contains(InvControls[i]))
                    splitInv.Panel1.Controls.Add(InvControls[i]);
                InvControls[i].SetBounds(x, y, 32, 32);
            }
        }



        void inv_Click(object sender, EventArgs e)
        {
            InventoryItemControl inv = (InventoryItemControl)sender;
            inv.Selected = !inv.Selected;
            inv.Render();
            inv.Refresh();
            cmbType.SelectedItem = Blocks.Get(inv.MyType);
            numCount.Value = inv.Count;
            numDamage.Value = inv.Damage;
        }

        private void Save()
        {
            if(EntityModified!=null)
                EntityModified(this,EventArgs.Empty);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (byte i = 0; i < Chest.Inventory.Count; i++)
            {
                if (Chest.Inventory.ContainsKey(i) && InvControls[i].Selected)
                {
                    InventoryItem itm = Chest.Inventory[i];
                    if (cmbType.SelectedItem != null)
                        itm.ID = (cmbType.SelectedItem as Block).ID;
                    itm.Count = (byte)numCount.Value;
                    itm.Damage = (short)numDamage.Value;
                    Chest.Inventory.Add(itm.Slot,itm);
                    InvControls[i].Refresh();
                }
            }
            SelectAll(false);
            Save();
        }

        private void SelectAll(bool on)
        {
            for (byte i = 0; i < Chest.Inventory.Count; i++)
            {
                if (Chest.Inventory.ContainsKey(i) && Chest.Inventory[i].ID != 0)
                {
                    InvControls[i].Selected = on;
                    InvControls[i].Render();
                    InvControls[i].Refresh();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (byte i = 0; i < Chest.Inventory.Count; i++)
            {
                if (Chest.Inventory.ContainsKey(i) && InvControls[i].Selected)
                {
                    InventoryItem inv = Chest.Inventory[i];
                    inv.Damage = 0;
                    Chest.Inventory[i] = inv;
                    InvControls[i].Render();
                    InvControls[i].Refresh();
                }
            }
            SelectAll(false);
            Save();
        }
        private void cmdDeleteInv_Click(object sender, EventArgs e)
        {
            for (byte i = 0; i < Chest.Inventory.Count; i++)
            {
                if (Chest.Inventory.ContainsKey(i) && InvControls[i].Selected)
                {
                    InventoryItem inv = Chest.Inventory[i];
                    inv.ID = 0;
                    inv.Count = 0;
                    inv.Damage = 0;
                    Chest.Inventory[i] = inv;
                    InvControls[i].Refresh();
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

            for (byte i = 0; i < Chest.Inventory.Count; i++)
            {
                if (Chest.Inventory.ContainsKey(i) && InvControls[i].Selected)
                {
                    InventoryItem inv = Chest.Inventory[i];
                    inv.Damage = -600;
                    Chest.Inventory[i] = inv;
                    InvControls[i].Refresh();
                }
            }
            SelectAll(false);
            Save();
        }

        private void tsbSaveTemplate_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "MineEdit Chest Template|*.mct";
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "templates"));
            sfd.InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(), "templates");
            if (sfd.ShowDialog() == DialogResult.Cancel) return;
            List<string> mit = new List<string>();
            mit.Add("# MINEEDIT INVENTORY TEMPLATE");
            mit.Add("# Don't edit this file or you'll mess it up.  Trust me.");
            for (byte i = 0; i < 54; i++)
            {
                if (Chest.Inventory.ContainsKey(i))
                {
                    mit.Add(string.Format("{0}\t{1}\t{2}\t{3}",
                        i,
                        Chest.Inventory[i].Count,
                        Chest.Inventory[i].ID,
                        Chest.Inventory[i].Damage));
                }
            }
            File.WriteAllLines(sfd.FileName, mit.ToArray());
        }

        private void tsbOpenTemplate_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "MineEdit Chest Template|*.mct";
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "templates"));
            ofd.InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(), "templates");
            if (ofd.ShowDialog() == DialogResult.Cancel) return;
            if (!File.Exists(ofd.FileName))
                return;
            foreach (string ln in File.ReadAllLines(ofd.FileName))
            {
                if (ln.StartsWith("#")) continue;
                string[] chunks = ln.Split('\t');
                byte idx = byte.Parse(chunks[0]);
                InventoryItem inv = Chest.Inventory[idx];
                inv.Count = byte.Parse(chunks[1]);
                inv.ID = short.Parse(chunks[2]);
                inv.Damage = short.Parse(chunks[3]);
                Chest.Inventory[idx] = inv;
            }
            Refresh();
        }

        public event EventHandler EntityModified;

        public TileEntity TileEntity
        {
            get
            {
                return Chest;
            }
            set
            {
                Chest = (Chest)TileEntity;
                Reload();
            }
        }
    }
}
