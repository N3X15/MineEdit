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
using System.IO;
using System.Windows.Forms;
using OpenMinecraft;

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
            Console.WriteLine("Loading /game/ handler.");
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
                mh.CorruptChunk += new CorruptChunkHandler(OnCorruptChunk);
                mh.Load(FileName);
                string mn = NewForm();
                frmMap map = GetMap(mn);
                map.Map = mh;

                map.Show();
                SetMap(mn, map);

                Settings.SetLUF(FileName);
            }
        }

        void OnCorruptChunk(string error, string file)
        {
            DialogResult dr = MessageBox.Show("A chunk is corrupt.  Would you like to delete and regenerate it?\n\n"+error, "Corrupt chunk!", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                File.Delete(file);
                MessageBox.Show(file + " deleted!");
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
            Console.WriteLine("frmMain loaded.");

            chkGridLines.Checked = Settings.ShowGridLines;
            chkWaterDepth.Checked = Settings.ShowWaterDepth;
            foreach (string luf in Settings.LastUsedFiles)
            {
                ToolStripMenuItem mnui = new ToolStripMenuItem(luf, null, new EventHandler(LUF_Click));
                //mnui.Enabled=false;
                mnuOpen.DropDownItems.Add(mnui);
            }
            ToolStripMenuItem[] menues = new ToolStripMenuItem[]
            {
                mnuWorld1,
                mnuWorld2,
                mnuWorld3,
                mnuWorld4,
                mnuWorld5,
            };
            foreach (KeyValuePair<short, float> w in Settings.Worlds)
            {
                menues[w.Key].Enabled = true;
                menues[w.Key].Text = string.Format("World {0} ({1} MB)", w.Key+1, w.Value);
            }
            openToolStripButton.DropDownItems.Add(new ToolStripMenuItem("Browse...",null,new EventHandler(OpenFile)));
            openToolStripButton.DropDownItems.Add(new ToolStripSeparator());
            openToolStripButton.DropDownItems.AddRange(menues);
#if DEBUG
            Text = string.Format("MineEdit - v.{0} (DEBUG)", Blocks.Version);
#else
            Text = string.Format("MineEdit - v.{0}", Blocks.Version);
#endif
        }

        private void LUF_Click(object s, EventArgs derp)
        {
            string FileName = (s as ToolStripMenuItem).Text;

            Open(FileName);
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

        private void mnuWorld1_Click(object sender, EventArgs e)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Open(Path.Combine(appdata, @".minecraft\saves\World1\level.dat"));
        }

        private void Open(string FileName)
        {


            IMapHandler mh;
            if (!GetFileHandler(FileName, out mh))
            {
                MessageBox.Show(string.Format("Unable to open file {0}: Unrecognised format", Path.GetFileName(FileName)), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            mh.CorruptChunk +=new CorruptChunkHandler(OnCorruptChunk);
            mh.Load(FileName);

            dlgLoading load = new dlgLoading(mh);
            load.ShowDialog();

            string mn = NewForm();
            frmMap map = GetMap(mn);
            map.Map = mh;

            map.Show();
            SetMap(mn, map);

            Settings.SetLUF(FileName);
        }

        private void mnuWorld2_Click(object sender, EventArgs e)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Open(Path.Combine(appdata, @".minecraft\saves\World2\level.dat"));
        }

        private void mnuWorld3_Click(object sender, EventArgs e)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Open(Path.Combine(appdata, @".minecraft\saves\World3\level.dat"));
        }

        private void mnuWorld4_Click(object sender, EventArgs e)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Open(Path.Combine(appdata, @".minecraft\saves\World4\level.dat"));
        }

        private void mnuWorld5_Click(object sender, EventArgs e)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Open(Path.Combine(appdata, @".minecraft\saves\World5\level.dat"));
        }

        private void randomSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
            {
                if ((ActiveMdiChild as frmMap).Map != null)
                {
                    long random = (ActiveMdiChild as frmMap).Map.RandomSeed;
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter="Random Seed File|*.rnd|Any file|*.*";
                    DialogResult dr = sfd.ShowDialog();
                    if (dr == System.Windows.Forms.DialogResult.OK)
                    {
                        File.WriteAllText(sfd.FileName, random.ToString());
                        MessageBox.Show("Saved.");
                    }
                }
            }
        }

        private void randomSeedToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
            {
                if ((ActiveMdiChild as frmMap).Map != null)
                {
                    OpenFileDialog sfd = new OpenFileDialog();
                    sfd.Filter = "Random Seed File|*.rnd|Any file|*.*";
                    DialogResult dr = sfd.ShowDialog();
                    if (dr == System.Windows.Forms.DialogResult.OK)
                    {
                        long random;
                        if(!long.TryParse(File.ReadAllText(sfd.FileName),out random))
                        {
                            MessageBox.Show("Use a valid Random Seed File (all it can contain is the random seed value).");
                            return;
                        }
                        (ActiveMdiChild as frmMap).Map.RandomSeed = random;
                        (ActiveMdiChild as frmMap).Map.Save();
                        DialogResult dr2 = MessageBox.Show("Would you also like to REMOVE ALL CHUNKS?  This will allow you to regenerate the entire map, but it will REMOVE ALL CHANGES APPLIED TO THE MAP.", "Regenerate?", MessageBoxButtons.YesNo);
                        if (dr2 == System.Windows.Forms.DialogResult.Yes)
                        {
                            dlgLongTask dlt = new dlgLongTask();
                            dlt.Start(delegate()
                            {
                                int NumChunks = 0;
                                dlt.SetMarquees(true, true);
                                dlt.VocabSubtask = "Chunk";
                                dlt.VocabSubtasks = "Chunks";
                                dlt.Title = "Removing chunks.";
                                dlt.Subtitle = "This will take a while.  Go take a break.";
                                dlt.CurrentSubtask = "Counting chunks (0)...";
                                dlt.CurrentTask = "Replacing stuff in chunks...";
                                dlt.TasksComplete = 0;
                                dlt.TasksTotal = NumChunks;
                                dlt.SubtasksTotal = 2;
                                (ActiveMdiChild as frmMap).Map.ForEachProgress += new ForEachProgressHandler(delegate(int Total, int Progress)
                                {
                                    dlt.TasksTotal = Total;
                                    dlt.TasksComplete = Progress;
                                });
                                (ActiveMdiChild as frmMap).Map.ForEachChunk(new Chunk.ChunkModifierDelegate(delegate(long x, long y)
                                {
                                    if (dlt.STOP) return;
                                    dlt.CurrentTask = string.Format("Deleting chunk ({0},{1})...", x, y); 
                                    dlt.CurrentSubtask = string.Format("Loading chunk ({0},{1})...", x, y);
                                    dlt.SubtasksComplete = 0;
                                    Chunk c = (ActiveMdiChild as frmMap).Map.GetChunk(x, y);
                                    if (c == null) return;
                                    dlt.CurrentSubtask = string.Format("Deleting chunk ({0},{1})...", x, y);
                                    dlt.SubtasksComplete = 1;
                                    File.Delete(c.Filename);
                                    dlt.SubtasksComplete = 2;
                                }));
                                dlt.Done();
                                MessageBox.Show("Done.");
                            });
                            dlt.ShowDialog();
                        }
                    }
                }
            }
        }

        public void ResetStatus()
        {
            tsbStatus.Text = "Ready.";
            tsbProgress.Style = ProgressBarStyle.Continuous;
            tsbProgress.Maximum = 100;
            tsbProgress.Value = 0;
        }

        private void waterDepthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.ShowWaterDepth = chkWaterDepth.Checked;

            if (ActiveMdiChild != null)
            {
                if ((ActiveMdiChild as frmMap).Map != null)
                {
                    (ActiveMdiChild as frmMap).Refresh();
                }
            }
        }

        private void fixLavalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
            {
                if ((ActiveMdiChild as frmMap).Map != null)
                {
                    (ActiveMdiChild as frmMap).FixLava();
                }
            }
        }

        private void generateTerrainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
            {
                if ((ActiveMdiChild as frmMap).Map != null)
                {
                    dlgTerrainGen terragen = new dlgTerrainGen((ActiveMdiChild as frmMap).Map);
                    if(terragen.ShowDialog() == DialogResult.Cancel)
                    {
                        ResetStatus();
                        return;
                    }
                    DialogResult dr = MessageBox.Show("This could DELETE EVERYTHING. ARE YOU SURE?", "ARE YOU NUTS", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.No)
                    {
                        ResetStatus();
                        return;
                    }
                    IMapGenerator mg = (IMapGenerator)terragen.pgMapGen.SelectedObject;
                    (ActiveMdiChild as frmMap).Map.Generator = mg;
                    dlgLongTask dlt = new dlgLongTask();
                    dlt.Start(delegate()
                    {
                        ///////////////////////////////////////////////////////////////
                        // Set up corrupt chunk handlers.
                        ///////////////////////////////////////////////////////////////
                        (ActiveMdiChild as frmMap).Map.CorruptChunk -= OnCorruptChunk;
                        frmReport report = new frmReport();
                        report.Hide();
                        CorruptChunkHandler cch = delegate(string error, string file)
                        {
                            //Console.WriteLine(file);
                            Vector2i coords = (ActiveMdiChild as frmMap).Map.GetChunkCoordsFromFile(file);
                            report.AddError(coords.X, coords.Y, error, delegate(long X, long Y)
                            {
                                File.Delete(file);
                                return true;
                            });
                        };
                        (ActiveMdiChild as frmMap).Map.CorruptChunk += cch;

                        /////////////////////////////////////////////////////////////////
                        // UI Stuff
                        /////////////////////////////////////////////////////////////////
                        dlt.SetMarquees(true, true);
                        dlt.VocabSubtask = "Chunk";
                        dlt.VocabSubtasks = "Chunks";
                        dlt.Title = "Generating chunks.";
                        dlt.Subtitle = "This will take a while.  Go take a break.";
                        dlt.SetMarquees(false, false);
                        dlt.CurrentTask = "Replacing stuff in chunks...";
                        dlt.TasksComplete = 0;
                        dlt.TasksTotal = 1;
                        dlt.SubtasksTotal = 1;

                        (ActiveMdiChild as frmMap).Map.ForEachProgress +=new ForEachProgressHandler(delegate(int Total, int Progress){
                            dlt.TasksTotal = Total;
                            dlt.TasksComplete = Progress;
                        });
                        (ActiveMdiChild as frmMap).Map.ForEachChunk(delegate(long X, long Y)
                        {
                            if (dlt.STOP) return;
                            dlt.SubtasksComplete = 0;
                            dlt.CurrentSubtask = string.Format("Generating chunk ({0},{1})", X, Y);
                            (ActiveMdiChild as frmMap).Map.Generate((ActiveMdiChild as frmMap).Map, X, Y);
                            dlt.SubtasksComplete = 1;
                        });
                        dlt.CurrentTask = "Fixing fluids, may take a while...";
                        dlt.SubtasksTotal = 2;
                        dlt.SubtasksComplete = 0;
                        IMapHandler mh = (ActiveMdiChild as frmMap).Map;
                        dlt.CurrentSubtask = "Fixing water...";
                        int hurr = 1;
                        int hurrT = 0;
                        int passes = 0;
                        //while (hurr != 0)
                        //{
                            passes++;
                            dlt.CurrentSubtask = string.Format("Fixing water (Pass #{0}, {1} blocks added)...", passes, hurrT);
                            hurr = mh.ExpandFluids(09, false, delegate(int Total,int Complete){
                                dlt.SubtasksTotal = Total;
                                dlt.SubtasksComplete = Complete;
                            });
                            hurrT += hurr;
                        //}
                        dlt.SubtasksComplete++;
                        dlt.CurrentSubtask = "Fixing lava...";
                        passes = 0;
                        hurrT = 0;
                        hurr = 1;
                        //while (hurr != 0)
                        //{
                            passes++; 
                            dlt.CurrentSubtask = string.Format("Fixing lava ({0} passes, {1} blocks added)...", passes, hurrT);
                            hurr = mh.ExpandFluids(11, false, delegate(int Total, int Complete)
                            {
                                dlt.SubtasksTotal = Total;
                                dlt.SubtasksComplete = Complete;
                            });
                            hurrT += hurr;
                        //}
                        dlt.SubtasksComplete++;
                        (ActiveMdiChild as frmMap).Map = mh;
                        dlt.Done();
                        MessageBox.Show("Done.  Keep in mind that loading may initially be slow.");
                        (ActiveMdiChild as frmMap).Map.CorruptChunk -= cch;
                        (ActiveMdiChild as frmMap).Map.CorruptChunk += OnCorruptChunk;
                        report.Repopulate();
                        report.ShowDialog();
                    });
                    dlt.ShowDialog();
                }
            }
        }

        protected void GenChunk(long X, long Y)
        {
            (ActiveMdiChild as frmMap).Map.Generate((ActiveMdiChild as frmMap).Map, X, Y);
        }

        private void recalcLightingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
            {
                if ((ActiveMdiChild as frmMap).Map != null)
                {
                    Profiler profSky = new Profiler("Sky Lighting");
                    Profiler profBlock = new Profiler("Block Lighting");
                    tsbStatus.Text = "Waiting for user response lol";
                    DialogResult dr = MessageBox.Show("MineEdit will remove lighting data from all chunks, forcing a lighting recalculation (according to advice from Notch, anyway).\n\nThis will inevitably take a long time.  ARE YOU SURE?", "DO YOU HAVE THE PATIENCE", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.No)
                    {
                        ResetStatus();
                        return;
                    }
                    dlgLongTask dlt = new dlgLongTask();
                    dlt.Start(delegate()
                    {
                        int NumChunks = 0;
                        dlt.VocabSubtask = "Chunk";
                        dlt.VocabSubtasks = "Chunks";
                        dlt.Title = "Stripping lighting from chunks.";
                        dlt.Subtitle = "This will take a while.  Go take a break.";
                        dlt.SetMarquees(false, false);
                        dlt.CurrentTask = "Replacing stuff in chunks...";
                        dlt.TasksComplete = 0;
                        dlt.TasksTotal = 1;
                        dlt.SubtasksTotal = 1;
                        (ActiveMdiChild as frmMap).Map.ForEachProgress += new ForEachProgressHandler(delegate(int Total, int Progress)
                        {
                            dlt.TasksTotal = Total;
                            dlt.TasksComplete = Progress;
                        });
                        int NumSkipped=0;
                        (ActiveMdiChild as frmMap).Map.ForEachChunk(delegate(long X, long Y)
                        {
                            Chunk c = (ActiveMdiChild as frmMap).Map.GetChunk(X, Y);
                            if (c == null)
                            {
                                ++NumSkipped;
                                return;
                            }
                            c.Save();
                            dlt.CurrentTask= string.Format("Stripping lighting... ({0}/{1}, {2} skipped)", dlt.TasksComplete, dlt.TasksTotal, NumSkipped);
                        });
                        dlt.Done();
                    });
                    dlt.ShowDialog();
                    MessageBox.Show("Lighting stripped from "+dlt.TasksComplete+" chunks.", "Report");
                    //(ActiveMdiChild as frmMap).Enabled = true;
                    (ActiveMdiChild as frmMap).ReloadAll();
                    ResetStatus();
                }
            }
        }
    }
}
