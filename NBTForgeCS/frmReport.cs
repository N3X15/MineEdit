using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MineEdit
{
    public partial class frmReport : Form
    {
        public enum ChunkMessageType
        {
            ERROR,
            WARNING,
            INFO,
            DEBUG
        }
        public delegate bool FixChunkDelegate(long X, long Y);

        public class ChunkMessage:DataGridViewRow
        {
            private ChunkMessageType mType;
            public ChunkMessageType Type
            {
                get { return (ChunkMessageType)Cells[0].Value; }
                set { 
                    mType = value;
                    switch (mType)
                    {
                        case ChunkMessageType.DEBUG:
                        case ChunkMessageType.INFO:
                            Cells[0].Value = MineEdit.Properties.Resources.dialog_information;
                            break;
                        case ChunkMessageType.WARNING:
                            Cells[0].Value = MineEdit.Properties.Resources.dialog_warning;
                            break;
                        case ChunkMessageType.ERROR:
                            Cells[0].Value = MineEdit.Properties.Resources.dialog_error;
                            break;
                    }
                }
            }
            public long X
            {
                get { return (long)Cells[1].Value; }
                set { Cells[1].Value = value; }
            }
            public long Y
            {
                get { return (long)Cells[2].Value; }
                set { Cells[2].Value = value; }
            }
            public string Message
            {
                get { return (string)Cells[3].Value; }
                set { Cells[3].Value = value; }
            }

            public FixChunkDelegate FixMethod;

            public ChunkMessage(ChunkMessageType type, long x, long y, string message, FixChunkDelegate method)
            {
                Cells.Clear();
                Cells.AddRange(
                    new DataGridViewImageCell(),
                    new DataGridViewTextBoxCell(),
                    new DataGridViewTextBoxCell(),
                    new DataGridViewTextBoxCell());
                Type = type;
                X = x;
                Y = y;
                Message = message;
                FixMethod = method;
            }
        }
        public List<ChunkMessage> Messages = new List<ChunkMessage>();
        public Dictionary<ChunkMessageType, int> Counts = new Dictionary<ChunkMessageType, int>();
        
        public frmReport()
        {
            InitializeComponent();
        }
        public void AddError(long X, long Y, string msg, FixChunkDelegate FixMethod)
        {
            ChunkMessage cm = new ChunkMessage(ChunkMessageType.ERROR, X, Y, msg, FixMethod);
            Messages.Add(cm);
        }
        public void AddWarning(long X, long Y, string msg, FixChunkDelegate FixMethod)
        {
            ChunkMessage cm = new ChunkMessage(ChunkMessageType.WARNING, X, Y, msg, FixMethod);
            Messages.Add(cm);
        }
        public void AddInfo(long X, long Y, string msg, FixChunkDelegate FixMethod)
        {
            ChunkMessage cm = new ChunkMessage(ChunkMessageType.INFO, X, Y, msg, FixMethod);
            Messages.Add(cm);
        }

        // APply filters and recount.
        public void Repopulate()
        {
            Counts.Clear();

            Counts.Add(ChunkMessageType.DEBUG, 0);
            Counts.Add(ChunkMessageType.ERROR, 0);
            Counts.Add(ChunkMessageType.WARNING, 0);
            Counts.Add(ChunkMessageType.INFO, 0);

            foreach (ChunkMessage cm in Messages)
            {
                Counts[cm.Type]++;
                if (
                    (cm.Type == ChunkMessageType.ERROR && chkErrors.Checked) ||
                    (cm.Type == ChunkMessageType.WARNING && chkWarnings.Checked) ||
                    (cm.Type == ChunkMessageType.INFO && chkInfo.Checked) ||
                    (cm.Type == ChunkMessageType.DEBUG && chkDebug.Checked))
                    dgvMain.Rows.Add(cm);
            }

            chkDebug.Text = string.Format("Debugging ({0})", Counts[ChunkMessageType.DEBUG]);
            chkErrors.Text = string.Format("Errors ({0})", Counts[ChunkMessageType.ERROR]);
            chkWarnings.Text = string.Format("Warnings ({0})", Counts[ChunkMessageType.WARNING]);
            chkInfo.Text = string.Format("Info ({0})", Counts[ChunkMessageType.WARNING]);
        }

        private void chkErrors_CheckedChanged(object sender, EventArgs e)
        {
            Repopulate();
        }

        private void cmdFixAll_Click(object sender, EventArgs e)
        {
            
            int mFixed=0;
            foreach (ChunkMessage cm in Messages)
            {
                if (cm.FixMethod != null)
                    if (cm.FixMethod(cm.X, cm.Y))
                        mFixed++;
            }
            MessageBox.Show(mFixed.ToString() + " chunks fixed.");
        }
    }
}
