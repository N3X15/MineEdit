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
using System.Diagnostics;

namespace MineEdit
{
    public partial class frmMain : Form
    {
        private int childFormNumber = 0;
        List<IMapHandler> FileHandlers = new List<IMapHandler>();
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripComboBox tsbDimension;
        List<frmMap> OpenFiles = new List<frmMap>();

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.browseToMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuQOinfdev = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuWorld1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuWorld2 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuWorld3 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuWorld4 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuWorld5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuReload = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.randomSeedToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.randomSeedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmReplace = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.chkGridLines = new System.Windows.Forms.ToolStripMenuItem();
            this.chkWaterDepth = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.fixLavalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateTerrainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recalcLightingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.newWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cascadeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileVerticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileHorizontalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arrangeIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.contentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.newToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.openToolStripButton = new System.Windows.Forms.ToolStripSplitButton();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.tsbReload = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbHeal = new System.Windows.Forms.ToolStripButton();
            this.tsbGoHome = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.helpToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbDimension = new System.Windows.Forms.ToolStripComboBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsbStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsbProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.editMenu,
            this.viewMenu,
            this.toolsMenu,
            this.windowsMenu,
            this.helpMenu});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.MdiWindowListItem = this.windowsMenu;
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(632, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "MenuStrip";
            // 
            // fileMenu
            // 
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.mnuOpen,
            this.mnuReload,
            this.toolStripSeparator3,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator11,
            this.importToolStripMenuItem,
            this.exportMapToolStripMenuItem,
            this.toolStripSeparator4,
            this.exitToolStripMenuItem});
            this.fileMenu.ImageTransparentColor = System.Drawing.SystemColors.ActiveBorder;
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(37, 20);
            this.fileMenu.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Image = global::MineEdit.Properties.Resources.document_new;
            this.newToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.newToolStripMenuItem.Text = "&New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.ShowNewForm);
            // 
            // mnuOpen
            // 
            this.mnuOpen.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.browseToMapToolStripMenuItem,
            this.mnuQOinfdev,
            this.toolStripSeparator10});
            this.mnuOpen.Image = global::MineEdit.Properties.Resources.document_open;
            this.mnuOpen.ImageTransparentColor = System.Drawing.Color.Black;
            this.mnuOpen.Name = "mnuOpen";
            this.mnuOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.mnuOpen.Size = new System.Drawing.Size(151, 22);
            this.mnuOpen.Text = "&Open";
            // 
            // browseToMapToolStripMenuItem
            // 
            this.browseToMapToolStripMenuItem.Name = "browseToMapToolStripMenuItem";
            this.browseToMapToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.browseToMapToolStripMenuItem.Text = "Browse to map...";
            this.browseToMapToolStripMenuItem.Click += new System.EventHandler(this.OpenFile);
            // 
            // mnuQOinfdev
            // 
            this.mnuQOinfdev.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuWorld1,
            this.mnuWorld2,
            this.mnuWorld3,
            this.mnuWorld4,
            this.mnuWorld5});
            this.mnuQOinfdev.Name = "mnuQOinfdev";
            this.mnuQOinfdev.Size = new System.Drawing.Size(162, 22);
            this.mnuQOinfdev.Text = "QuickOpen...";
            this.mnuQOinfdev.Visible = false;
            // 
            // mnuWorld1
            // 
            this.mnuWorld1.Enabled = false;
            this.mnuWorld1.Name = "mnuWorld1";
            this.mnuWorld1.Size = new System.Drawing.Size(115, 22);
            this.mnuWorld1.Text = "World 1";
            this.mnuWorld1.Click += new System.EventHandler(this.mnuWorld1_Click);
            // 
            // mnuWorld2
            // 
            this.mnuWorld2.Enabled = false;
            this.mnuWorld2.Name = "mnuWorld2";
            this.mnuWorld2.Size = new System.Drawing.Size(115, 22);
            this.mnuWorld2.Text = "World 2";
            this.mnuWorld2.Click += new System.EventHandler(this.mnuWorld2_Click);
            // 
            // mnuWorld3
            // 
            this.mnuWorld3.Enabled = false;
            this.mnuWorld3.Name = "mnuWorld3";
            this.mnuWorld3.Size = new System.Drawing.Size(115, 22);
            this.mnuWorld3.Text = "World 3";
            this.mnuWorld3.Click += new System.EventHandler(this.mnuWorld3_Click);
            // 
            // mnuWorld4
            // 
            this.mnuWorld4.Enabled = false;
            this.mnuWorld4.Name = "mnuWorld4";
            this.mnuWorld4.Size = new System.Drawing.Size(115, 22);
            this.mnuWorld4.Text = "World 4";
            this.mnuWorld4.Click += new System.EventHandler(this.mnuWorld4_Click);
            // 
            // mnuWorld5
            // 
            this.mnuWorld5.Enabled = false;
            this.mnuWorld5.Name = "mnuWorld5";
            this.mnuWorld5.Size = new System.Drawing.Size(115, 22);
            this.mnuWorld5.Text = "World 5";
            this.mnuWorld5.Click += new System.EventHandler(this.mnuWorld5_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(159, 6);
            // 
            // mnuReload
            // 
            this.mnuReload.Image = global::MineEdit.Properties.Resources.view_refresh;
            this.mnuReload.Name = "mnuReload";
            this.mnuReload.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.mnuReload.Size = new System.Drawing.Size(151, 22);
            this.mnuReload.Text = "Reload";
            this.mnuReload.Click += new System.EventHandler(this.tsbReload_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(148, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = global::MineEdit.Properties.Resources.document_save;
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripButton_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(148, 6);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.randomSeedToolStripMenuItem1});
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.importToolStripMenuItem.Text = "Import";
            // 
            // randomSeedToolStripMenuItem1
            // 
            this.randomSeedToolStripMenuItem1.Name = "randomSeedToolStripMenuItem1";
            this.randomSeedToolStripMenuItem1.Size = new System.Drawing.Size(156, 22);
            this.randomSeedToolStripMenuItem1.Text = "Random Seed...";
            this.randomSeedToolStripMenuItem1.Click += new System.EventHandler(this.randomSeedToolStripMenuItem1_Click);
            // 
            // exportMapToolStripMenuItem
            // 
            this.exportMapToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.randomSeedToolStripMenuItem});
            this.exportMapToolStripMenuItem.Name = "exportMapToolStripMenuItem";
            this.exportMapToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.exportMapToolStripMenuItem.Text = "Export";
            // 
            // randomSeedToolStripMenuItem
            // 
            this.randomSeedToolStripMenuItem.Name = "randomSeedToolStripMenuItem";
            this.randomSeedToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.randomSeedToolStripMenuItem.Text = "Random Seed...";
            this.randomSeedToolStripMenuItem.Click += new System.EventHandler(this.randomSeedToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(148, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolsStripMenuItem_Click);
            // 
            // editMenu
            // 
            this.editMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmReplace,
            this.toolStripSeparator13,
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator6,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator7,
            this.selectAllToolStripMenuItem});
            this.editMenu.Name = "editMenu";
            this.editMenu.Size = new System.Drawing.Size(39, 20);
            this.editMenu.Text = "&Edit";
            // 
            // tsmReplace
            // 
            this.tsmReplace.Name = "tsmReplace";
            this.tsmReplace.Size = new System.Drawing.Size(164, 22);
            this.tsmReplace.Text = "Replace...";
            this.tsmReplace.Click += new System.EventHandler(this.tsmReplace_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(161, 6);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.undoToolStripMenuItem.Text = "&Undo";
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.redoToolStripMenuItem.Text = "&Redo";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(161, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.cutToolStripMenuItem.Text = "Cu&t";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.CutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.CopyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.pasteToolStripMenuItem.Text = "&Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.PasteToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(161, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.selectAllToolStripMenuItem.Text = "Select &All";
            // 
            // viewMenu
            // 
            this.viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolBarToolStripMenuItem,
            this.statusBarToolStripMenuItem,
            this.toolStripSeparator9,
            this.chkGridLines,
            this.chkWaterDepth});
            this.viewMenu.Name = "viewMenu";
            this.viewMenu.Size = new System.Drawing.Size(44, 20);
            this.viewMenu.Text = "&View";
            // 
            // toolBarToolStripMenuItem
            // 
            this.toolBarToolStripMenuItem.Checked = true;
            this.toolBarToolStripMenuItem.CheckOnClick = true;
            this.toolBarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolBarToolStripMenuItem.Name = "toolBarToolStripMenuItem";
            this.toolBarToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.toolBarToolStripMenuItem.Text = "&Toolbar";
            this.toolBarToolStripMenuItem.Click += new System.EventHandler(this.ToolBarToolStripMenuItem_Click);
            // 
            // statusBarToolStripMenuItem
            // 
            this.statusBarToolStripMenuItem.Checked = true;
            this.statusBarToolStripMenuItem.CheckOnClick = true;
            this.statusBarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.statusBarToolStripMenuItem.Name = "statusBarToolStripMenuItem";
            this.statusBarToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.statusBarToolStripMenuItem.Text = "&Status Bar";
            this.statusBarToolStripMenuItem.Click += new System.EventHandler(this.StatusBarToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(137, 6);
            // 
            // chkGridLines
            // 
            this.chkGridLines.Checked = true;
            this.chkGridLines.CheckOnClick = true;
            this.chkGridLines.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGridLines.Enabled = false;
            this.chkGridLines.Name = "chkGridLines";
            this.chkGridLines.Size = new System.Drawing.Size(140, 22);
            this.chkGridLines.Text = "Grid Lines";
            this.chkGridLines.Click += new System.EventHandler(this.chkGridLines_Click);
            // 
            // chkWaterDepth
            // 
            this.chkWaterDepth.CheckOnClick = true;
            this.chkWaterDepth.Enabled = false;
            this.chkWaterDepth.Name = "chkWaterDepth";
            this.chkWaterDepth.Size = new System.Drawing.Size(140, 22);
            this.chkWaterDepth.Text = "Water Depth";
            this.chkWaterDepth.Click += new System.EventHandler(this.waterDepthToolStripMenuItem_Click);
            // 
            // toolsMenu
            // 
            this.toolsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.toolStripSeparator12,
            this.fixLavalToolStripMenuItem,
            this.generateTerrainToolStripMenuItem,
            this.recalcLightingToolStripMenuItem});
            this.toolsMenu.Name = "toolsMenu";
            this.toolsMenu.Size = new System.Drawing.Size(48, 20);
            this.toolsMenu.Text = "&Tools";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Enabled = false;
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.optionsToolStripMenuItem.Text = "&Options";
            this.optionsToolStripMenuItem.ToolTipText = "Not needed... Yet.";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(167, 6);
            // 
            // fixLavalToolStripMenuItem
            // 
            this.fixLavalToolStripMenuItem.Name = "fixLavalToolStripMenuItem";
            this.fixLavalToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.fixLavalToolStripMenuItem.Text = "Fix Lava";
            this.fixLavalToolStripMenuItem.Click += new System.EventHandler(this.fixLavalToolStripMenuItem_Click);
            // 
            // generateTerrainToolStripMenuItem
            // 
            this.generateTerrainToolStripMenuItem.Name = "generateTerrainToolStripMenuItem";
            this.generateTerrainToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.generateTerrainToolStripMenuItem.Text = "Generate Terrain...";
            this.generateTerrainToolStripMenuItem.Click += new System.EventHandler(this.generateTerrainToolStripMenuItem_Click);
            // 
            // recalcLightingToolStripMenuItem
            // 
            this.recalcLightingToolStripMenuItem.Name = "recalcLightingToolStripMenuItem";
            this.recalcLightingToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.recalcLightingToolStripMenuItem.Text = "Recalc Lighting...";
            this.recalcLightingToolStripMenuItem.Click += new System.EventHandler(this.recalcLightingToolStripMenuItem_Click);
            // 
            // windowsMenu
            // 
            this.windowsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newWindowToolStripMenuItem,
            this.cascadeToolStripMenuItem,
            this.tileVerticalToolStripMenuItem,
            this.tileHorizontalToolStripMenuItem,
            this.closeAllToolStripMenuItem,
            this.arrangeIconsToolStripMenuItem});
            this.windowsMenu.Name = "windowsMenu";
            this.windowsMenu.Size = new System.Drawing.Size(68, 20);
            this.windowsMenu.Text = "&Windows";
            // 
            // newWindowToolStripMenuItem
            // 
            this.newWindowToolStripMenuItem.Name = "newWindowToolStripMenuItem";
            this.newWindowToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.newWindowToolStripMenuItem.Text = "&New Window";
            this.newWindowToolStripMenuItem.Click += new System.EventHandler(this.ShowNewForm);
            // 
            // cascadeToolStripMenuItem
            // 
            this.cascadeToolStripMenuItem.Name = "cascadeToolStripMenuItem";
            this.cascadeToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.cascadeToolStripMenuItem.Text = "&Cascade";
            this.cascadeToolStripMenuItem.Click += new System.EventHandler(this.CascadeToolStripMenuItem_Click);
            // 
            // tileVerticalToolStripMenuItem
            // 
            this.tileVerticalToolStripMenuItem.Name = "tileVerticalToolStripMenuItem";
            this.tileVerticalToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.tileVerticalToolStripMenuItem.Text = "Tile &Vertical";
            this.tileVerticalToolStripMenuItem.Click += new System.EventHandler(this.TileVerticalToolStripMenuItem_Click);
            // 
            // tileHorizontalToolStripMenuItem
            // 
            this.tileHorizontalToolStripMenuItem.Name = "tileHorizontalToolStripMenuItem";
            this.tileHorizontalToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.tileHorizontalToolStripMenuItem.Text = "Tile &Horizontal";
            this.tileHorizontalToolStripMenuItem.Click += new System.EventHandler(this.TileHorizontalToolStripMenuItem_Click);
            // 
            // closeAllToolStripMenuItem
            // 
            this.closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
            this.closeAllToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.closeAllToolStripMenuItem.Text = "C&lose All";
            this.closeAllToolStripMenuItem.Click += new System.EventHandler(this.CloseAllToolStripMenuItem_Click);
            // 
            // arrangeIconsToolStripMenuItem
            // 
            this.arrangeIconsToolStripMenuItem.Name = "arrangeIconsToolStripMenuItem";
            this.arrangeIconsToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.arrangeIconsToolStripMenuItem.Text = "&Arrange Icons";
            this.arrangeIconsToolStripMenuItem.Click += new System.EventHandler(this.ArrangeIconsToolStripMenuItem_Click);
            // 
            // helpMenu
            // 
            this.helpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contentsToolStripMenuItem,
            this.toolStripSeparator8,
            this.aboutToolStripMenuItem,
            this.mnuUpdate});
            this.helpMenu.Name = "helpMenu";
            this.helpMenu.Size = new System.Drawing.Size(44, 20);
            this.helpMenu.Text = "&Help";
            // 
            // contentsToolStripMenuItem
            // 
            this.contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
            this.contentsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F1)));
            this.contentsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.contentsToolStripMenuItem.Text = "&Contents";
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(177, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aboutToolStripMenuItem.Text = "&About MineEdit ...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click_1);
            // 
            // mnuUpdate
            // 
            this.mnuUpdate.Name = "mnuUpdate";
            this.mnuUpdate.Size = new System.Drawing.Size(180, 22);
            this.mnuUpdate.Text = "Check for Updates...";
            this.mnuUpdate.Click += new System.EventHandler(this.mnuUpdate_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripButton,
            this.openToolStripButton,
            this.saveToolStripButton,
            this.tsbReload,
            this.toolStripSeparator1,
            this.tsbHeal,
            this.tsbGoHome,
            this.toolStripSeparator2,
            this.helpToolStripButton,
            this.toolStripSeparator5,
            this.tsbDimension});
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(632, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "ToolStrip";
            // 
            // newToolStripButton
            // 
            this.newToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newToolStripButton.Image = global::MineEdit.Properties.Resources.document_new;
            this.newToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
            this.newToolStripButton.Name = "newToolStripButton";
            this.newToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.newToolStripButton.Text = "New";
            this.newToolStripButton.Click += new System.EventHandler(this.ShowNewForm);
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton.Image = global::MineEdit.Properties.Resources.document_open;
            this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Size = new System.Drawing.Size(32, 22);
            this.openToolStripButton.Text = "Open";
            // 
            // saveToolStripButton
            // 
            this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToolStripButton.Image = global::MineEdit.Properties.Resources.document_save;
            this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.saveToolStripButton.Text = "Save";
            this.saveToolStripButton.Click += new System.EventHandler(this.saveToolStripButton_Click);
            // 
            // tsbReload
            // 
            this.tsbReload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbReload.Image = global::MineEdit.Properties.Resources.view_refresh;
            this.tsbReload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbReload.Name = "tsbReload";
            this.tsbReload.Size = new System.Drawing.Size(23, 22);
            this.tsbReload.Text = "Reload";
            this.tsbReload.Click += new System.EventHandler(this.tsbReload_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbHeal
            // 
            this.tsbHeal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbHeal.Image = global::MineEdit.Properties.Resources.list_add;
            this.tsbHeal.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHeal.Name = "tsbHeal";
            this.tsbHeal.Size = new System.Drawing.Size(23, 22);
            this.tsbHeal.Text = "Heal";
            this.tsbHeal.Click += new System.EventHandler(this.tsbHeal_Click);
            // 
            // tsbGoHome
            // 
            this.tsbGoHome.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbGoHome.Image = global::MineEdit.Properties.Resources.go_home;
            this.tsbGoHome.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGoHome.Name = "tsbGoHome";
            this.tsbGoHome.Size = new System.Drawing.Size(23, 22);
            this.tsbGoHome.Text = "Go back to Spawn";
            this.tsbGoHome.Click += new System.EventHandler(this.tsbGoHome_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // helpToolStripButton
            // 
            this.helpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.helpToolStripButton.Image = global::MineEdit.Properties.Resources.dialog_information;
            this.helpToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
            this.helpToolStripButton.Name = "helpToolStripButton";
            this.helpToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.helpToolStripButton.Text = "Help";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbDimension
            // 
            this.tsbDimension.Items.AddRange(new object[] {
            "Normal",
            "Nether"});
            this.tsbDimension.Name = "tsbDimension";
            this.tsbDimension.Size = new System.Drawing.Size(121, 25);
            this.tsbDimension.SelectedIndexChanged += new System.EventHandler(this.tsbDimension_SelectedIndexChanged);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.tsbStatus,
            this.tsbProgress});
            this.statusStrip.Location = new System.Drawing.Point(0, 431);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(632, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "Test";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // tsbStatus
            // 
            this.tsbStatus.Name = "tsbStatus";
            this.tsbStatus.Size = new System.Drawing.Size(51, 17);
            this.tsbStatus.Text = "Waiting.";
            // 
            // tsbProgress
            // 
            this.tsbProgress.Name = "tsbProgress";
            this.tsbProgress.Size = new System.Drawing.Size(100, 16);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 453);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "frmMain";
            this.Text = "MineEdit v1.0";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tileHorizontalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuOpen;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editMenu;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewMenu;
        private System.Windows.Forms.ToolStripMenuItem toolBarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statusBarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsMenu;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowsMenu;
        private System.Windows.Forms.ToolStripMenuItem newWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cascadeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tileVerticalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem arrangeIconsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpMenu;
        private System.Windows.Forms.ToolStripMenuItem contentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton newToolStripButton;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.ToolStripButton helpToolStripButton;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem chkGridLines;
        private System.Windows.Forms.ToolStripButton tsbHeal;
        private System.Windows.Forms.ToolStripButton tsbGoHome;
        private System.Windows.Forms.ToolStripButton tsbReload;
        private System.Windows.Forms.ToolStripMenuItem mnuReload;
        private System.Windows.Forms.ToolStripMenuItem mnuUpdate;
        private System.Windows.Forms.ToolStripMenuItem browseToMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuQOinfdev;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem mnuWorld1;
        private System.Windows.Forms.ToolStripMenuItem mnuWorld2;
        private System.Windows.Forms.ToolStripMenuItem mnuWorld3;
        private System.Windows.Forms.ToolStripMenuItem mnuWorld4;
        private System.Windows.Forms.ToolStripMenuItem mnuWorld5;
        public System.Windows.Forms.StatusStrip statusStrip;
        public System.Windows.Forms.ToolStripStatusLabel tsbStatus;
        public System.Windows.Forms.ToolStripProgressBar tsbProgress;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem exportMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem randomSeedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem randomSeedToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem chkWaterDepth;
        private System.Windows.Forms.ToolStripSplitButton openToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripMenuItem fixLavalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateTerrainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recalcLightingToolStripMenuItem;
        private System.ComponentModel.IContainer components;
        private ToolStripSeparator toolStripSeparator13;
        private ToolStripMenuItem tsmReplace;
        public frmMain()
        {
            InitializeComponent();
            Console.WriteLine("Loading /game/ handler.");
            FileHandlers.Add(new InfdevHandler()); // infdev

            // Load TileEntity handlers.
            OpenMinecraft.Entities.Entity.LoadEntityTypes();
            OpenMinecraft.TileEntities.TileEntity.LoadEntityTypes();
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
                
                map.Map.SetDimension(0);
                tsbDimension.Items.Clear();
                foreach (Dimension d in map.Map.GetDimensions())
                {
                    tsbDimension.Items.Add(d);
                }
                tsbDimension.SelectedIndex = 0;

                map.Show();
                SetMap(mn, map);

                Settings.SetLUF(FileName);
            }
        }

        List<string> BrokenChunks = new List<string>();
        void OnCorruptChunk(long X, long Y, string error, string file)
        {
            BrokenChunks.Add(file);
        }

        public void ShowReport()
        {
            if (BrokenChunks.Count > 0)
            {
                DialogResult dr = MessageBox.Show(string.Format("{0} chunks are broken.  Fixing them requires removing them completely so that Minecraft can regenerate them.\n\nDo you want MineEdit to attempt to remove these broken chunks? ALL DATA ON THESE CHUNKS ARE LOST.", BrokenChunks.Count), "Broken chunks", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (dr == System.Windows.Forms.DialogResult.Yes)
                {
                    foreach (string file in BrokenChunks)
                        File.Delete(file);
                }
                else
                {
                    Environment.Exit(0);
                }
            }
        }

        public void ClearReport()
        {
            BrokenChunks.Clear();
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

            Open(FileName,0);
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
                    (ActiveMdiChild as frmMap).Map.Save();
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
            Open(Path.Combine(appdata, @".minecraft\saves\World1\level.dat"),0);
        }

        private void Open(string FileName,int dim)
        {
            IMapHandler mh;
            if (!GetFileHandler(FileName, out mh))
            {
                MessageBox.Show(string.Format("Unable to open file {0}: Unrecognised format", Path.GetFileName(FileName)), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            mh.CorruptChunk +=new CorruptChunkHandler(OnCorruptChunk);
            mh.Load(FileName);
            mh.SetDimension(dim);

            //dlgLoading load = new dlgLoading(mh);
            //load.ShowDialog();

            string mn = NewForm();
            frmMap map = GetMap(mn);
            map.Map = mh;

            tsbDimension.Items.Clear();
            foreach (Dimension d in map.Map.GetDimensions())
            {
                tsbDimension.Items.Add(d);
            }
            tsbDimension.SelectedIndex = 0;

            map.Show();

            SetMap(mn, map);

            Settings.SetLUF(FileName);
        }

        private void mnuWorld2_Click(object sender, EventArgs e)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Open(Path.Combine(appdata, @".minecraft\saves\World2\level.dat"),0);
        }

        private void mnuWorld3_Click(object sender, EventArgs e)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Open(Path.Combine(appdata, @".minecraft\saves\World3\level.dat"),0);
        }

        private void mnuWorld4_Click(object sender, EventArgs e)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Open(Path.Combine(appdata, @".minecraft\saves\World4\level.dat"),0);
        }

        private void mnuWorld5_Click(object sender, EventArgs e)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Open(Path.Combine(appdata, @".minecraft\saves\World5\level.dat"),0);
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
                                (ActiveMdiChild as frmMap).Map.ForEachChunk(delegate(IMapHandler mh, long x, long y)
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
                                });
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
                        // ACTIVATE AUTOREPAIR
                        (ActiveMdiChild as frmMap).Map.Autorepair = true;

                        /////////////////////////////////////////////////////////////////
                        // UI Stuff
                        /////////////////////////////////////////////////////////////////
                        dlt.SetMarquees(true, true);
                        dlt.VocabSubtask = "chunk";
                        dlt.VocabSubtasks = "chunks";
                        dlt.Title = "Generating chunks.";
                        dlt.Subtitle = "This will take a while.  Go take a break.";
                        dlt.SetMarquees(false, false);
                        dlt.CurrentTask = "Replacing stuff in chunks...";
                        dlt.TasksComplete = 0;
                        dlt.TasksTotal = 1;
                        dlt.SubtasksTotal = 1;

                        int numchunks = 0;
                        // Generate terrain
                        // Drop soil
                        // Add pbarriers
                        // Add dungeons
                        // Add trees
                        // Fix fluids
                        // Fix lava

                        int stage = 0;
                        int numstages = 2;
                        ForEachProgressHandler feph = new ForEachProgressHandler(delegate(int Total, int Progress)
                        {
                            numchunks = Total;
                            dlt.TasksTotal = numchunks * numstages;
                            dlt.TasksComplete = Progress + (stage * numchunks);
                            dlt.SubtasksComplete = Progress;
                            dlt.SubtasksTotal = Total;
                        });
                        ForEachProgressHandler fl_feph = new ForEachProgressHandler(delegate(int Total, int Progress)
                        {
                            numchunks = Total;
                            dlt.TasksTotal = numchunks * numstages;
                            dlt.TasksComplete = Progress + (stage * numchunks);
                            dlt.SubtasksComplete = Progress;
                            dlt.SubtasksTotal = Total;

                            dlt.perfChart.AddValue((ActiveMdiChild as frmMap).Map.ChunksLoaded);
                        });

                        dlt.CurrentTask = "Regenerating chunks...";
                        (ActiveMdiChild as frmMap).Map.ForEachProgress += feph;
                        dlt.ShowPerfChart(true);
                        dlt.perfChart.Clear();
                        dlt.perfChart.ScaleMode = SpPerfChart.ScaleMode.Relative;
                        (ActiveMdiChild as frmMap).Map.ForEachChunk(delegate(IMapHandler _mh, long X, long Y)
                        {
                            if (dlt.STOP) return;
                            dlt.CurrentSubtask = string.Format("Generating chunk ({0},{1})", X, Y);
                            double min, max;
                            (ActiveMdiChild as frmMap).Map.Generate((ActiveMdiChild as frmMap).Map, X, Y, out min, out max);
                            dlt.grpPerformance.Text = string.Format("Terrain Profile [{0},{1}]m",(int)(min*100),(int)(max*100));
                            dlt.perfChart.AddValue((decimal)max);
                        }); 
                        /*
                        stage++;
                        dlt.CurrentTask = "Eroding chunk surfaces...";
                        (ActiveMdiChild as frmMap).Map.ForEachProgress += feph;
                        (ActiveMdiChild as frmMap).Map.ForEachChunk(delegate(IMapHandler _mh, long X, long Y)
                        {
                            dlt.CurrentSubtask = string.Format("Eroding chunk ({0},{1}, thermal)", X, Y);
                            //(ActiveMdiChild as frmMap).Map.ErodeThermal(5, 10, (int)X, (int)Y);

                            dlt.CurrentSubtask = string.Format("Eroding chunk ({0},{1}, hydraulic)", X, Y);
                            //(ActiveMdiChild as frmMap).Map.Erode(5, 10, (int)X, (int)Y);

                            dlt.CurrentSubtask = string.Format("Eroding chunk ({0},{1}, silt)", X, Y);
                            //(ActiveMdiChild as frmMap).Map.Silt(63,true, (int)X, (int)Y);

                            dlt.grpPerformance.Text = string.Format("Chunks in-memory ({0})", _mh.ChunksLoaded);
                            dlt.perfChart.AddValue(_mh.ChunksLoaded);
                        });
                        */
                        stage++;
                        dlt.CurrentTask = "Finalizing chunks...";
                        (ActiveMdiChild as frmMap).Map.ForEachProgress += feph;
                        dlt.perfChart.Clear();
                        (ActiveMdiChild as frmMap).Map.ForEachChunk(delegate(IMapHandler _mh, long X, long Y)
                        {
                            dlt.CurrentSubtask = string.Format("Finalizing chunk ({0},{1})...", X, Y);
                            (ActiveMdiChild as frmMap).Map.FinalizeGeneration((ActiveMdiChild as frmMap).Map, X, Y);

                            dlt.grpPerformance.Text = string.Format("Chunks in-memory ({0})", _mh.ChunksLoaded);
                            dlt.perfChart.AddValue(_mh.ChunksLoaded);
                        });


                        dlt.CurrentTask = "Fixing fluids, may take a while...";
                        dlt.SubtasksTotal = 2;
                        dlt.SubtasksComplete = 0;
                        IMapHandler mh = (ActiveMdiChild as frmMap).Map;
                        dlt.CurrentSubtask = "Fixing water...";
                        int hurr = 1;
                        int hurrT = 0;
                        int passes = 0;
                        dlt.SetMarquees(true, true);
                        //while (hurr != 0)
                        //{
                        passes++;
                        dlt.CurrentSubtask = string.Format("Fixing water (Pass #{0}, {1} blocks added)...", passes, hurrT);
                        dlt.perfChart.Clear();
                        (ActiveMdiChild as frmMap).Map.ForEachProgress += fl_feph;
                        hurr = mh.ExpandFluids(08, false, delegate(int Total,int Complete){
                            dlt.SubtasksTotal = Total;
                            dlt.SubtasksComplete = Complete;

                            dlt.grpPerformance.Text = string.Format("Chunks in-memory ({0})", mh.ChunksLoaded);
                            dlt.perfChart.AddValue(mh.ChunksLoaded);
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
                            (ActiveMdiChild as frmMap).Map.ForEachProgress += fl_feph;
                            hurr = mh.ExpandFluids(11, false, delegate(int Total, int Complete)
                            {
                                dlt.SubtasksTotal = Total;
                                dlt.SubtasksComplete = Complete;

                                dlt.grpPerformance.Text = string.Format("Chunks in-memory ({0})", mh.ChunksLoaded);
                                dlt.perfChart.AddValue(mh.ChunksLoaded);
                            });
                            hurrT += hurr;
                        //}
                        dlt.SubtasksComplete++;
                        (ActiveMdiChild as frmMap).Map = mh;
                        dlt.CurrentSubtask = "SAVING CHUNKS";
                        mh.SaveAll();
                        dlt.SetMarquees(false,false);
                        dlt.Done();
                        ClearReport();
                        mh.Time = 0;
                        Utils.FixPlayerPlacement(ref mh);
                        mh.Save();
                        MessageBox.Show("Done.  Keep in mind that loading may initially be slow.");
                        
                        // DEACTIVATE AUTOREPAIR
                        (ActiveMdiChild as frmMap).Map.Autorepair = false;
                    });
                    dlt.ShowDialog();
                }
            }
        }

        protected void GenChunk(long X, long Y)
        {
            double min, max;
            (ActiveMdiChild as frmMap).Map.Generate((ActiveMdiChild as frmMap).Map, X, Y, out min, out max);
        }

        private void recalcLightingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
            {
                if ((ActiveMdiChild as frmMap).Map != null)
                {
                    tsbStatus.Text = "Waiting for user response lol";
                    DialogResult dr = MessageBox.Show("MineEdit will try and recalculate lighting GLOBALLY using quartz-lightgen.\n\nThis will inevitably take a long time.  ARE YOU SURE?", "DO YOU HAVE THE PATIENCE", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.No)
                    {
                        ResetStatus();
                        return;
                    }
                    dlgLongTask dlt = new dlgLongTask();
                    dlt.Start(delegate()
                    {
                        ShittyLighter lighter = new ShittyLighter();
                        dlt.VocabSubtask = "chunk";
                        dlt.VocabSubtasks = "chunks";
                        dlt.Title = "Relighting Map";
                        dlt.Subtitle = "This will take a while.  Go take a break.";
                        dlt.SetMarquees(false, false);
                        dlt.CurrentTask = "Relighting...";
                        dlt.TasksComplete = 0;
                        dlt.TasksTotal = 1;
                        dlt.SubtasksTotal = 1;

                        IMapHandler mh = (ActiveMdiChild as frmMap).Map;
                        ForEachProgressHandler FEPH = new ForEachProgressHandler(delegate(int Total, int Progress)
                        {
                            dlt.TasksTotal = Total;
                            dlt.TasksComplete = Progress;

                            dlt.CurrentSubtask = "Processes";
                            dlt.SubtasksComplete = mh.ChunksLoaded;
                            dlt.SubtasksTotal = 200;
                            if (mh.ChunksLoaded>200)
                            {
                                string ot = dlt.CurrentTask;
                                Console.WriteLine("****SAVING****");
                                dlt.CurrentTask = "[Saving to avoid overusing RAM]";
                                (ActiveMdiChild as frmMap).Map.SaveAll();
                                dlt.CurrentTask = ot;
                            }
                            (ActiveMdiChild as frmMap).Map.CullUnchanged();
                        });

                        
                        dlt.CurrentTask = "Gathering chunks needing light...";
                        mh.ForEachProgress += FEPH;
                        List<string> chunks = new List<string>();
                        string tf = Path.GetTempFileName();
                        mh.ForEachChunk(delegate(IMapHandler _map,long X, long Y)
                        {
                            chunks.Add(string.Format("{0},{1}",X,Y));
                            //_map.RegenerateLighting(X, Y);
                        });
                        File.WriteAllLines(tf, chunks.ToArray());
                        string args = "'" + Path.GetDirectoryName(mh.Filename) + Path.DirectorySeparatorChar + "' '"+tf+"'";
                        Process child = Process.Start("lightgen.exe", args);
                        //child.WaitForInputIdle();
                        /*
                        // Skylight
                        dlt.CurrentTask = "Skylight...";
                        mh.ForEachProgress += FEPH;
                        lighter.SkylightGlobal(ref mh);
                        mh.SaveAll();

                        // Blocklight
                        dlt.CurrentTask = "Relighting (BlockLight)...";
                        mh.ForEachProgress += FEPH;
                        lighter.BlocklightGlobal(ref mh);
                        mh.SaveAll();
                        */
                        (ActiveMdiChild as frmMap).Map = mh;
                        dlt.Done();
                    });

                    (ActiveMdiChild as frmMap).Map.Autorepair = true;
                    dlt.ShowDialog();
                    (ActiveMdiChild as frmMap).Map.Autorepair = false;
                    MessageBox.Show("Lighting regenerated for "+dlt.TasksComplete+" chunks.", "Report");
                    //(ActiveMdiChild as frmMap).Enabled = true;
                    (ActiveMdiChild as frmMap).ReloadAll();
                    ResetStatus();
                }
            }
        }

        private void tsmReplace_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
            {
                if ((ActiveMdiChild as frmMap).Map != null)
                {
                    dlgReplace rep = new dlgReplace((ActiveMdiChild as frmMap).Map);
                    rep.ShowDialog();
                }
            }
        }

        private void tsbDimension_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tsbDimension.SelectedItem==null) return;

            if (ActiveMdiChild != null)
            {
                if ((ActiveMdiChild as frmMap).Map != null)
                {
                    (ActiveMdiChild as frmMap).Map.SetDimension((tsbDimension.SelectedItem as Dimension).ID);

                    ClearReport();

                    dlgLoading load = new dlgLoading((ActiveMdiChild as frmMap).Map);
                    load.ShowDialog();

                    ShowReport();

                    ResetStatus();
                    (ActiveMdiChild as frmMap).ReloadAll();
                }
            }
        }
    }
}
