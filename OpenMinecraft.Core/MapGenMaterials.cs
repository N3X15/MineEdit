using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Drawing;

namespace OpenMinecraft
{
    public class MaterialsConverter 
        : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(MapGenMaterials))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is MapGenMaterials)
            {
                MapGenMaterials mgm = (MapGenMaterials)value;
                return mgm.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    internal class UIMaterialDropdown : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editorService=null;
            if (provider != null)
            {
                editorService =
                    provider.GetService(
                    typeof(IWindowsFormsEditorService))
                    as IWindowsFormsEditorService;
            }

            if (editorService != null)
            {
                ComboBox selectionControl = new ComboBox();
                selectionControl.DrawItem += new DrawItemEventHandler(selectionControl_DrawItem);
                selectionControl.ValueMember = "ID";
                selectionControl.DisplayMember = "Name";
                selectionControl.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
                selectionControl.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
                selectionControl.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
                selectionControl.DropDownWidth = 200;
                selectionControl.FormattingEnabled = true;
                foreach (short k in Blocks.BlockList.Keys)
                {
                    selectionControl.Items.Add((byte)k);
                }
                selectionControl.SelectedItem=(byte)value;

                editorService.DropDownControl(selectionControl);

                value = (byte)selectionControl.SelectedItem;
            }

            return value;
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle area = e.Bounds;
            Rectangle iconArea = area;
            iconArea.Width = 16;
            if (e.Value != null)
            {
                //e.DrawBackground();
                Block ent = Blocks.Get((byte)e.Value);

                // Draw block icon
                g.DrawImage(ent.Image, iconArea);
                /*
                Font monospace = new Font(FontFamily.GenericMonospace,12f);
                Font sans = new Font(FontFamily.GenericSansSerif,12f);
                // ID.
                string idtxt = string.Format("{0:D3} 0x{0:X3}", ent.ID);
                SizeF idSz = g.MeasureString(idtxt, monospace);
                Rectangle idArea = area;
                idArea.X = iconArea.Right + 3;
                idArea.Width = (int)idSz.Width + 1;
                g.DrawString(idtxt, monospace, Brushes.Gray, idArea);

                // Block name
                SizeF entName = g.MeasureString(ent.ToString(), sans);
                Rectangle ctxt = area;
                ctxt.X = idArea.Right + 3;
                ctxt.Width = (int)entName.Width + 1;
                g.DrawString(ent.ToString(), sans, Brushes.Black, ctxt);*/
            }
        }

        void selectionControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle area = e.Bounds;
            Rectangle iconArea = area;
            iconArea.Width = 16;
            if (e.Index >= 0)
            {
                e.DrawBackground();
                Block ent = Blocks.Get((byte)(sender as ComboBox).Items[e.Index]);

                // Draw block icon
                g.DrawImage(ent.Image, iconArea);

                // ID
                string idtxt = string.Format("{0:D3} 0x{0:X3}", ent.ID);
                SizeF idSz = g.MeasureString(idtxt, (sender as ComboBox).Font);
                Rectangle idArea = area;
                idArea.X = iconArea.Right + 3;
                idArea.Width = (int)idSz.Width + 1;
                g.DrawString(idtxt, (sender as ComboBox).Font, new SolidBrush(Color.FromArgb(128, e.ForeColor)), idArea);

                // Block name
                SizeF entName = g.MeasureString(ent.ToString(), (sender as ComboBox).Font);
                Rectangle ctxt = area;
                ctxt.X = idArea.Right + 3;
                ctxt.Width = (int)entName.Width + 1;
                g.DrawString(ent.ToString(), (sender as ComboBox).Font, new SolidBrush(e.ForeColor), ctxt);
            }
        }

    }

    /// <summary>
    /// Tell the map generator which materials it should use.
    /// </summary>
    [TypeConverter(typeof(MaterialsConverter)), 
    Description("Expand to see soil options.")]
    public class MapGenMaterials
    {
        [EditorAttribute(typeof(UIMaterialDropdown), typeof(UITypeEditor))]
        public byte Rock { get; set; }

        [EditorAttribute(typeof(UIMaterialDropdown), typeof(UITypeEditor))]
        public byte Soil { get; set; }

        [EditorAttribute(typeof(UIMaterialDropdown), typeof(UITypeEditor))]
        public byte Grass { get; set; }

        [EditorAttribute(typeof(UIMaterialDropdown), typeof(UITypeEditor))]
        public byte Sand { get; set; }

        [EditorAttribute(typeof(UIMaterialDropdown), typeof(UITypeEditor))]
        public byte Snow { get; set; }

        [EditorAttribute(typeof(UIMaterialDropdown), typeof(UITypeEditor))]
        public byte Gravel { get; set; }

        [EditorAttribute(typeof(UIMaterialDropdown), typeof(UITypeEditor))]
        public byte Water { get; set; }

        [EditorAttribute(typeof(UIMaterialDropdown), typeof(UITypeEditor))]
        public byte Lava { get; set; }

        [EditorAttribute(typeof(UIMaterialDropdown), typeof(UITypeEditor))]
        public byte Ice { get; set; }

        public MapGenMaterials()
        {
            Rock = 1;
            Soil = 3;
            Grass = 2;
            Sand = 12;
            Snow = 78;
            Gravel = 13;
            Water = 8; // 8 = auto lightfix
            Lava = 11;
            Ice = 79;
        }
        public override string ToString()
        {
            return "{ "+
                string.Format("Rock=0x{0:X2}, ", Rock) +
                string.Format("Soil=0x{0:X2}, ", Soil) +
                string.Format("Grass=0x{0:X2}, ", Grass) +
                string.Format("Sand=0x{0:X2}, ", Sand) +
                string.Format("Snow=0x{0:X2}, ", Snow) +
                string.Format("Gravel=0x{0:X2}, ", Gravel) +
                string.Format("Water=0x{0:X2}, ", Water) +
                string.Format("Lava=0x{0:X2}, ", Lava) +
                string.Format("Ice=0x{0:X2}", Ice) +
                " }";
        }
    }
}
