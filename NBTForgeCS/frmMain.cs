using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MineEdit
{
    public partial class frmMain : Form
    {
        private int childFormNumber = 0;
        List<IMapHandler> FileHandlers = new List<IMapHandler>();
        List<frmMap> OpenFiles = new List<frmMap>();
        public frmMain()
        {
            InitializeComponent();
            FileHandlers.Add(new InfdevHandler()); // infdev
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            frmMap childForm = new frmMap();
            childForm.MdiParent = this;
            childForm.Text = "Untitled Map " + childFormNumber++;
            childForm.Show();
            childForm.Map = null;
            OpenFiles.Add(childForm);
        }

        private string NewForm()
        {
            frmMap childForm = new frmMap();
            childForm.MdiParent = this;
            childForm.Text = "Untitled Map " + childFormNumber++;
            childForm.Show();
            childForm.Map = null;
            OpenFiles.Add(childForm);
            return childForm.Text;
        }

        private frmMap GetMap(string Filename)
        {
            foreach (frmMap m in OpenFiles)
            {
                if (m.Text == Filename)
                    return m;
            }
            return null;
        }

        private void SetMap(string Filename,frmMap MapToSet)
        {
            frmMap fm = GetMap(Filename);
            if (fm == null) return;
            int i = OpenFiles.IndexOf(fm);
            OpenFiles[i] = MapToSet;
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // %APPDATA%/.minecraft/saves/
            string appdata=Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            openFileDialog.InitialDirectory = Path.Combine(appdata,@".minecraft\saves\");
            openFileDialog.Filter = "All recognised files|*.mclevel;level.dat|/indev/ levels (*.mclevel)|*.mclevel|/infdev/ maps (level.dat)|level.dat|All files (*)|*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                IMapHandler mh;
                string FileName = openFileDialog.FileName;
                if (!GetFileHandler(FileName, out mh))
                {
                    MessageBox.Show(string.Format("Unable to open file {0}: Unrecognised format",Path.GetFileName(FileName)), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                mh.Load(FileName);
                string mn = NewForm();
                frmMap map = GetMap(mn);
                map.Map = mh;

                map.Show();
                SetMap(mn, map);
            }
        }

        private bool GetFileHandler(string FileName, out IMapHandler mh)
        {
            mh = null;
            foreach(IMapHandler _mh in FileHandlers)
            {
                if (_mh.IsMyFiletype(FileName))
                {
                    mh = _mh;
                    return true;
                }
            }
            return false;
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMap map = (frmMap)this.ActiveMdiChild;
            if (map == null) return;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            // %APPDATA%/.minecraft/saves/
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            saveFileDialog.InitialDirectory = Path.Combine(appdata, ".minecraft/saves/");
            saveFileDialog.Filter = "/indev/ levels (*.mclevel)|*.mclevel|/infdev/ maps (level.dat)|level.dat|All files (*)|*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                map.Map.Save(saveFileDialog.FileName);
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            Blocks.Init();
        }

        public void SetStatus(string p)
        {
            lblStatus.Text = p;
        }

        private void chkGridLines_Click(object sender, EventArgs e)
        {
            Settings.ShowGridLines = chkGridLines.Checked;
            foreach (Form c in MdiChildren)
            {
                ((frmMap)c).Refresh();
            }
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            if(ActiveMdiChild!=null)
                (ActiveMdiChild as frmMap).Map.Save();
        }

        private void mnuReload_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
                (ActiveMdiChild as frmMap).Map.Load();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            frmAbout hurp = new frmAbout();
            hurp.ShowDialog();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void tsbHeal_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
            {
                if ((ActiveMdiChild as frmMap).Map != null)
                {
                    (ActiveMdiChild as frmMap).Map.Health = 100;
                }
            }
        }

        private void tsbGoHome_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
            {
                if ((ActiveMdiChild as frmMap).Map != null)
                {
                    (ActiveMdiChild as frmMap).Map.PlayerPos.X = (ActiveMdiChild as frmMap).Map.Spawn.X;
                    (ActiveMdiChild as frmMap).Map.PlayerPos.Y = (ActiveMdiChild as frmMap).Map.Spawn.Y;
                    (ActiveMdiChild as frmMap).Map.PlayerPos.Z = (ActiveMdiChild as frmMap).Map.Spawn.Z;
                }
            }
        }

        private void tsbReload_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
            {
                if ((ActiveMdiChild as frmMap).Map != null)
                {
                    (ActiveMdiChild as frmMap).ReloadAll();
                }
            }

        }

        private void mnuUpdate_Click(object sender, EventArgs e)
        {
            using (frmUpdate up = new frmUpdate())
            {
                up.ShowDialog();
            }
        }
    }
}
