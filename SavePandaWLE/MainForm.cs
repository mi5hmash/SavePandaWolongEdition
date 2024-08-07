using System.Media;
using SavePandaWLE.Helpers;
using SavePandaWolongEditionCore;
using SavePandaWolongEditionCore.Helpers;
using static SavePandaWolongEditionCore.Helpers.SimpleLogger;

namespace SavePandaWLE;

public partial class MainForm : Form
{
    // Program Core
    private readonly Core _programCore;

    public MainForm()
    {
        var mediator = new SimpleMediatorWinForms();
        var pText = new Progress<string>(s => toolStripStatusLabel1.Text = s);
        var pValue = new Progress<int>(i => toolStripProgressBar1.Value = i);
        _programCore = new Core(mediator, pText, pValue, new SimpleLogger(new SimpleLoggerOptions(AppInfo.RootPath)
        {
            AllowDebugMessages = true,
            LoggedAppName = $"{AppInfo.Title} v{AppInfo.Version}",
            MaxLogFiles = 1,
            MinSeverityLevel = LogSeverity.Information
        }));
        _programCore.ActivateLogger();

        InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        // set controls
        versionLabel.Text = $@"v{AppInfo.Version}";
        authorLabel.Text = $@"{AppInfo.Author} 2024";
        TBFilepath.Text = "";
        TBSteamId.Text = @"0";
        backupCheckBox.Checked = _programCore.BackupEnabled;

        // transparent SuperUserTrigger hack
        versionLabel.Controls.Add(superUserTrigger);
        superUserTrigger.Size = versionLabel.Size;
        superUserTrigger.Location = new Point(0, 0);
    }

    #region SUPER_USER

    // Super User
    private const int SuperUserThreshold = 3;
    private bool _isSuperUser;
    private int _superUserClicks;

    private void SuperUserTrigger_Click(object sender, EventArgs e)
    {
        if (_isSuperUser) return;

        _superUserClicks += 1;

        if (_superUserClicks >= SuperUserThreshold) return;

        // restart superUserTimer
        superUserTimer.Stop();
        superUserTimer.Start();
    }

    private void SuperUserTimer_Tick(object sender, EventArgs e)
    {
        superUserTimer.Stop();
        if (_superUserClicks >= SuperUserThreshold) EnableSuperUser();
        _superUserClicks = 0;
    }

    private void EnableSuperUser()
    {
        _isSuperUser = true;
        // things to unlock
        ButtonEncrypt.Visible = true;
        ButtonDecrypt.Visible = true;
        ButtonExportJson.Visible = true;
        ButtonImportJson.Visible = true;
        ButtonOpenJsonDir.Visible = true;
        // play sound
        SystemSounds.Beep.Play();
    }

    #endregion

    #region BACKUP

    private void BackupCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        if (sender is not CheckBox checkBox) return;
        _programCore.SetBackupEnabled(checkBox.Checked);
        // detach event to prevent stack overflow
        checkBox.CheckedChanged -= BackupCheckBox_CheckedChanged!;
        // update checkBox state
        checkBox.Checked = _programCore.BackupEnabled;
        // re-attach the event
        checkBox.CheckedChanged += BackupCheckBox_CheckedChanged!;
    }

    private void ButtonOpenBackupDir_Click(object sender, EventArgs e)
        => Core.OpenBackupDirectory();

    #endregion

    #region STEAM_ID

    private void TBSteamId_Leave(object sender, EventArgs e)
    {
        if (sender is not TextBox textBox) return;
        _programCore.SetSteamId(textBox.Text);
        textBox.Text = _programCore.SteamId.ToString();
    }

    #endregion

    #region INPUT_PATH

    private void ValidatePath(object sender)
    {
        if (sender is not TextBox textBox) return;
        _programCore.SetInputFilePath(textBox.Text);
        textBox.Text = _programCore.InputFilePath;
        TBSteamId.Text = _programCore.SteamId.ToString();
    }

    private void TBFilepath_Leave(object sender, EventArgs e)
        => ValidatePath(sender);

    private void TBFilepath_DragDrop(object sender, DragEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        if (!e.Data!.GetDataPresent(DataFormats.FileDrop)) return;
        var filePaths = (string[])e.Data.GetData(DataFormats.FileDrop)!;
        var filePath = filePaths[0];
        textBox.Text = filePath;
        ValidatePath(textBox);
    }

    private void TBFilepath_DragOver(object sender, DragEventArgs e)
        => e.Effect = DragDropEffects.Copy;

    private void ButtonSelectFile_Click(object sender, EventArgs e)
    {
        if (_programCore.IsBusy) return;
        if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
        TBFilepath.Text = openFileDialog1.FileName;
        ValidatePath(TBFilepath);
    }

    #endregion

    #region OPERATIONS

    private void AuthorLabel_Click(object sender, EventArgs e)
        => AppInfo.VisitAuthorsGithub();

    private void ButtonOpenJsonDir_Click(object sender, EventArgs e)
        => Core.OpenJsonDirectory();

    private delegate Task OperationDelegate();

    private async Task ProcessAsyncOperation(OperationDelegate operationDelegate)
    {
        if (_programCore.IsBusy) return;

        await operationDelegate();

        // play sound
        SoundPlayer sp = new(SavePandaWLE.Properties.Resources.typewritter_machine);
        sp.Play();
    }

    private async void ButtonDecrypt_Click(object sender, EventArgs e)
        => await ProcessAsyncOperation(_programCore.DecryptAsync);

    private async void ButtonEncrypt_Click(object sender, EventArgs e)
        => await ProcessAsyncOperation(_programCore.EncryptAsync);

    private async void ButtonResign_Click(object sender, EventArgs e)
        => await ProcessAsyncOperation(_programCore.ResignAsync);

    private async void ButtonExportJson_Click(object sender, EventArgs e)
        => await ProcessAsyncOperation(_programCore.ExportJsonAsync);

    private async void ButtonImportJson_Click(object sender, EventArgs e)
        => await ProcessAsyncOperation(_programCore.ImportJsonAsync);

    #endregion
}