namespace SavePandaWLE
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            ButtonSelectFile = new Button();
            authorLabel = new Label();
            versionLabel = new Label();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripProgressBar1 = new ToolStripProgressBar();
            statusStrip1 = new StatusStrip();
            ButtonResign = new Button();
            LabelSteamId = new Label();
            LabelFilepath = new Label();
            pb_AppIcon = new PictureBox();
            ButtonEncrypt = new Button();
            ButtonDecrypt = new Button();
            TBFilepath = new TextBox();
            TBSteamId = new TextBox();
            toolTip1 = new ToolTip(components);
            ButtonOpenBackupDir = new Button();
            ButtonOpenJsonDir = new Button();
            backupCheckBox = new CheckBox();
            superUserTimer = new System.Windows.Forms.Timer(components);
            ButtonExportJson = new Button();
            ButtonImportJson = new Button();
            openFileDialog1 = new OpenFileDialog();
            superUserTrigger = new PictureBox();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pb_AppIcon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)superUserTrigger).BeginInit();
            SuspendLayout();
            // 
            // ButtonSelectFile
            // 
            ButtonSelectFile.AllowDrop = true;
            ButtonSelectFile.Location = new Point(340, 29);
            ButtonSelectFile.Name = "ButtonSelectFile";
            ButtonSelectFile.Size = new Size(30, 23);
            ButtonSelectFile.TabIndex = 2;
            ButtonSelectFile.Text = "📁";
            toolTip1.SetToolTip(ButtonSelectFile, "Open the Select File Window");
            ButtonSelectFile.UseVisualStyleBackColor = true;
            ButtonSelectFile.Click += ButtonSelectFile_Click;
            ButtonSelectFile.DragDrop += TBFilepath_DragDrop;
            ButtonSelectFile.DragOver += TBFilepath_DragOver;
            // 
            // authorLabel
            // 
            authorLabel.AutoSize = true;
            authorLabel.Cursor = Cursors.Hand;
            authorLabel.Font = new Font("Segoe UI", 7F);
            authorLabel.Location = new Point(423, 143);
            authorLabel.Name = "authorLabel";
            authorLabel.Size = new Size(75, 12);
            authorLabel.TabIndex = 35;
            authorLabel.Text = "Mi5hmasH 2024";
            authorLabel.TextAlign = ContentAlignment.MiddleRight;
            authorLabel.Click += AuthorLabel_Click;
            // 
            // versionLabel
            // 
            versionLabel.AutoSize = true;
            versionLabel.Location = new Point(452, 128);
            versionLabel.Name = "versionLabel";
            versionLabel.Size = new Size(46, 15);
            versionLabel.TabIndex = 34;
            versionLabel.Text = "v1.0.0.0";
            versionLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(39, 17);
            toolStripStatusLabel1.Text = "Ready";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(60, 16);
            toolStripProgressBar1.Step = 5;
            toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripProgressBar1, toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 163);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(510, 22);
            statusStrip1.SizingGrip = false;
            statusStrip1.TabIndex = 33;
            statusStrip1.Text = "statusStrip1";
            // 
            // ButtonResign
            // 
            ButtonResign.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            ButtonResign.Location = new Point(304, 82);
            ButtonResign.Name = "ButtonResign";
            ButtonResign.Size = new Size(66, 39);
            ButtonResign.TabIndex = 6;
            ButtonResign.Text = "RESIGN";
            ButtonResign.UseVisualStyleBackColor = true;
            ButtonResign.Click += ButtonResign_Click;
            // 
            // LabelSteamId
            // 
            LabelSteamId.AutoSize = true;
            LabelSteamId.Location = new Point(12, 65);
            LabelSteamId.Name = "LabelSteamId";
            LabelSteamId.Size = new Size(66, 15);
            LabelSteamId.TabIndex = 25;
            LabelSteamId.Text = "Steam32 ID";
            // 
            // LabelFilepath
            // 
            LabelFilepath.AutoSize = true;
            LabelFilepath.Location = new Point(12, 11);
            LabelFilepath.Name = "LabelFilepath";
            LabelFilepath.Size = new Size(83, 15);
            LabelFilepath.TabIndex = 23;
            LabelFilepath.Text = "Input File Path";
            // 
            // pb_AppIcon
            // 
            pb_AppIcon.Image = SavePandaWLE.Properties.Resources.SavePanda_Icon_x256;
            pb_AppIcon.Location = new Point(386, 10);
            pb_AppIcon.Name = "pb_AppIcon";
            pb_AppIcon.Size = new Size(112, 117);
            pb_AppIcon.SizeMode = PictureBoxSizeMode.Zoom;
            pb_AppIcon.TabIndex = 20;
            pb_AppIcon.TabStop = false;
            // 
            // ButtonEncrypt
            // 
            ButtonEncrypt.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            ButtonEncrypt.Location = new Point(232, 128);
            ButtonEncrypt.Name = "ButtonEncrypt";
            ButtonEncrypt.Size = new Size(66, 23);
            ButtonEncrypt.TabIndex = 8;
            ButtonEncrypt.Text = "ENCRYPT";
            ButtonEncrypt.UseVisualStyleBackColor = true;
            ButtonEncrypt.Visible = false;
            ButtonEncrypt.Click += ButtonEncrypt_Click;
            // 
            // ButtonDecrypt
            // 
            ButtonDecrypt.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            ButtonDecrypt.Location = new Point(160, 128);
            ButtonDecrypt.Name = "ButtonDecrypt";
            ButtonDecrypt.Size = new Size(66, 23);
            ButtonDecrypt.TabIndex = 7;
            ButtonDecrypt.Text = "DECRYPT";
            ButtonDecrypt.UseVisualStyleBackColor = true;
            ButtonDecrypt.Visible = false;
            ButtonDecrypt.Click += ButtonDecrypt_Click;
            // 
            // TBFilepath
            // 
            TBFilepath.AllowDrop = true;
            TBFilepath.Location = new Point(12, 29);
            TBFilepath.Name = "TBFilepath";
            TBFilepath.Size = new Size(322, 23);
            TBFilepath.TabIndex = 1;
            TBFilepath.DragDrop += TBFilepath_DragDrop;
            TBFilepath.DragOver += TBFilepath_DragOver;
            TBFilepath.Leave += TBFilepath_Leave;
            // 
            // TBSteamId
            // 
            TBSteamId.Location = new Point(12, 83);
            TBSteamId.Name = "TBSteamId";
            TBSteamId.Size = new Size(142, 23);
            TBSteamId.TabIndex = 5;
            TBSteamId.Leave += TBSteamId_Leave;
            // 
            // ButtonOpenBackupDir
            // 
            ButtonOpenBackupDir.ForeColor = SystemColors.ControlDarkDark;
            ButtonOpenBackupDir.Location = new Point(340, 3);
            ButtonOpenBackupDir.Name = "ButtonOpenBackupDir";
            ButtonOpenBackupDir.Size = new Size(30, 23);
            ButtonOpenBackupDir.TabIndex = 4;
            ButtonOpenBackupDir.Text = "⚙";
            toolTip1.SetToolTip(ButtonOpenBackupDir, "Open the _BACKUP directory");
            ButtonOpenBackupDir.UseVisualStyleBackColor = true;
            ButtonOpenBackupDir.Click += ButtonOpenBackupDir_Click;
            // 
            // ButtonOpenJsonDir
            // 
            ButtonOpenJsonDir.AllowDrop = true;
            ButtonOpenJsonDir.Font = new Font("Segoe UI", 5F);
            ButtonOpenJsonDir.Location = new Point(160, 62);
            ButtonOpenJsonDir.Name = "ButtonOpenJsonDir";
            ButtonOpenJsonDir.Size = new Size(21, 19);
            ButtonOpenJsonDir.TabIndex = 10;
            ButtonOpenJsonDir.Text = "📂";
            toolTip1.SetToolTip(ButtonOpenJsonDir, "Open directory of the JSON file");
            ButtonOpenJsonDir.UseVisualStyleBackColor = true;
            ButtonOpenJsonDir.Visible = false;
            ButtonOpenJsonDir.Click += ButtonOpenJsonDir_Click;
            // 
            // backupCheckBox
            // 
            backupCheckBox.AutoSize = true;
            backupCheckBox.Font = new Font("Segoe UI", 8.25F);
            backupCheckBox.Location = new Point(268, 7);
            backupCheckBox.Name = "backupCheckBox";
            backupCheckBox.Size = new Size(66, 17);
            backupCheckBox.TabIndex = 3;
            backupCheckBox.Text = "BACKUP";
            backupCheckBox.UseVisualStyleBackColor = true;
            backupCheckBox.CheckedChanged += BackupCheckBox_CheckedChanged;
            // 
            // superUserTimer
            // 
            superUserTimer.Interval = 500;
            superUserTimer.Tick += SuperUserTimer_Tick;
            // 
            // ButtonExportJson
            // 
            ButtonExportJson.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            ButtonExportJson.Location = new Point(160, 83);
            ButtonExportJson.Name = "ButtonExportJson";
            ButtonExportJson.Size = new Size(66, 38);
            ButtonExportJson.TabIndex = 9;
            ButtonExportJson.Text = "EXPORT JSON";
            ButtonExportJson.UseVisualStyleBackColor = true;
            ButtonExportJson.Visible = false;
            ButtonExportJson.Click += ButtonExportJson_Click;
            // 
            // ButtonImportJson
            // 
            ButtonImportJson.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            ButtonImportJson.Location = new Point(232, 83);
            ButtonImportJson.Name = "ButtonImportJson";
            ButtonImportJson.Size = new Size(66, 38);
            ButtonImportJson.TabIndex = 11;
            ButtonImportJson.Text = "IMPORT JSON";
            ButtonImportJson.UseVisualStyleBackColor = true;
            ButtonImportJson.Visible = false;
            ButtonImportJson.Click += ButtonImportJson_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.DefaultExt = "bin";
            openFileDialog1.FileName = "SAVEDATA.BIN";
            openFileDialog1.Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*";
            openFileDialog1.SelectReadOnly = false;
            // 
            // superUserTrigger
            // 
            superUserTrigger.BackColor = Color.Transparent;
            superUserTrigger.Location = new Point(386, 128);
            superUserTrigger.Name = "superUserTrigger";
            superUserTrigger.Size = new Size(10, 10);
            superUserTrigger.TabIndex = 38;
            superUserTrigger.TabStop = false;
            superUserTrigger.Click += SuperUserTrigger_Click;
            superUserTrigger.DoubleClick += SuperUserTrigger_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(510, 185);
            Controls.Add(ButtonOpenJsonDir);
            Controls.Add(superUserTrigger);
            Controls.Add(ButtonImportJson);
            Controls.Add(ButtonExportJson);
            Controls.Add(ButtonOpenBackupDir);
            Controls.Add(backupCheckBox);
            Controls.Add(authorLabel);
            Controls.Add(versionLabel);
            Controls.Add(statusStrip1);
            Controls.Add(ButtonResign);
            Controls.Add(LabelSteamId);
            Controls.Add(LabelFilepath);
            Controls.Add(pb_AppIcon);
            Controls.Add(ButtonEncrypt);
            Controls.Add(ButtonDecrypt);
            Controls.Add(ButtonSelectFile);
            Controls.Add(TBFilepath);
            Controls.Add(TBSteamId);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            Text = "SavePanda Wolong Edition";
            Load += Form1_Load;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pb_AppIcon).EndInit();
            ((System.ComponentModel.ISupportInitialize)superUserTrigger).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button ButtonSelectFile;
        private Label authorLabel;
        private Label versionLabel;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripProgressBar toolStripProgressBar1;
        private StatusStrip statusStrip1;
        private Button ButtonResign;
        private Label LabelSteamId;
        private Label LabelFilepath;
        private PictureBox pb_AppIcon;
        private Button ButtonEncrypt;
        private Button ButtonDecrypt;
        private TextBox TBFilepath;
        private TextBox TBSteamId;
        private ToolTip toolTip1;
        private CheckBox backupCheckBox;
        private Button ButtonOpenBackupDir;
        private System.Windows.Forms.Timer superUserTimer;
        private Button ButtonExportJson;
        private Button ButtonImportJson;
        private OpenFileDialog openFileDialog1;
        private PictureBox superUserTrigger;
        private Button ButtonOpenJsonDir;
    }
}