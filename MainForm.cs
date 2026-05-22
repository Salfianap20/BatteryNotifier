using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace AdvancedBatteryNotifier
{
    public class MainForm : Form
    {
        Label lblTitle;
        Label lblBattery;
        Label lblStatus;
        Label lblAlarm;
        Label lblAlarmFile;

        ProgressBar progressBattery;

        NumericUpDown numLow;
        NumericUpDown numFull;

        NumericUpDown numLowInterval;
        NumericUpDown numFullInterval;

        Button btnSave;
        Button btnBrowse;
        Button btnStopAlarm;

        NotifyIcon notifyIcon;
        Timer timer;

        OpenFileDialog openFileDialog;

        private SettingsModel settings;

        private SoundPlayer player;

        private DateTime lastLowBatteryAlarm =
            DateTime.MinValue;

        private DateTime lastFullBatteryAlarm =
            DateTime.MinValue;

        private string settingsPath =
            "settings.json";

        public MainForm()
        {
            InitializeUI();

            LoadSettings();

            timer.Start();

            CheckBattery();
        }

        private void InitializeUI()
        {
            Text = "Advanced Battery Notifier";

            Size = new Size(550, 830);

            StartPosition =
                FormStartPosition.CenterScreen;

            FormBorderStyle =
                FormBorderStyle.FixedSingle;

            MaximizeBox = false;

            BackColor = Color.White;

            Font = new Font("Segoe UI", 10);

            // AUTO STARTUP
            Microsoft.Win32.RegistryKey rk =
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
                true);

            rk.SetValue(
                "AdvancedBatteryNotifier",
                Application.ExecutablePath);

            // TITLE
            lblTitle = new Label();

            lblTitle.Text =
                "ADVANCED BATTERY NOTIFIER";

            lblTitle.Font =
                new Font(
                    "Segoe UI",
                    16,
                    FontStyle.Bold);

            lblTitle.AutoSize = true;

            lblTitle.Location =
                new Point(75, 20);

            // BATTERY %
            lblBattery = new Label();

            lblBattery.Text = "0%";

            lblBattery.Font =
                new Font(
                    "Segoe UI",
                    36,
                    FontStyle.Bold);

            lblBattery.ForeColor =
                Color.Green;

            lblBattery.AutoSize = true;

            lblBattery.Location =
                new Point(190, 70);

            // STATUS
            lblStatus = new Label();

            lblStatus.Text =
                "Status";

            lblStatus.Font =
                new Font(
                    "Segoe UI",
                    12);

            lblStatus.ForeColor =
                Color.Gray;

            lblStatus.AutoSize = true;

            lblStatus.Location =
                new Point(205, 145);

            // PROGRESS BAR
            progressBattery =
                new ProgressBar();

            progressBattery.Location =
                new Point(80, 190);

            progressBattery.Size =
                new Size(360, 30);

            progressBattery.Maximum = 100;

            // =========================
            // LOW BATTERY SECTION
            // =========================

            Label lblLowTitle =
                new Label();

            lblLowTitle.Text =
                "LOW BATTERY";

            lblLowTitle.Font =
                new Font(
                    "Segoe UI",
                    11,
                    FontStyle.Bold);

            lblLowTitle.Location =
                new Point(80, 245);

            lblLowTitle.AutoSize = true;

            // LOW PERCENTAGE
            Label lblLowPercent =
                new Label();

            lblLowPercent.Text =
                "Percentage";

            lblLowPercent.Location =
                new Point(100, 285);

            lblLowPercent.AutoSize = true;

            numLow =
                new NumericUpDown();

            numLow.Location =
                new Point(320, 280);

            numLow.Minimum = 1;

            numLow.Maximum = 100;

            numLow.Value = 30;

            // LOW INTERVAL
            Label lblLowInterval =
                new Label();

            lblLowInterval.Text =
                "Interval (Seconds)";

            lblLowInterval.Location =
                new Point(100, 325);

            lblLowInterval.AutoSize = true;

            numLowInterval =
                new NumericUpDown();

            numLowInterval.Location =
                new Point(320, 320);

            numLowInterval.Minimum = 1;

            numLowInterval.Maximum = 3600;

            numLowInterval.Value = 15;

            // =========================
            // FULL BATTERY SECTION
            // =========================

            Label lblFullTitle =
                new Label();

            lblFullTitle.Text =
                "FULL BATTERY";

            lblFullTitle.Font =
                new Font(
                    "Segoe UI",
                    11,
                    FontStyle.Bold);

            lblFullTitle.Location =
                new Point(80, 380);

            lblFullTitle.AutoSize = true;

            // FULL PERCENTAGE
            Label lblFullPercent =
                new Label();

            lblFullPercent.Text =
                "Percentage";

            lblFullPercent.Location =
                new Point(100, 420);

            lblFullPercent.AutoSize = true;

            numFull =
                new NumericUpDown();

            numFull.Location =
                new Point(320, 415);

            numFull.Minimum = 1;

            numFull.Maximum = 100;

            numFull.Value = 100;

            // FULL INTERVAL
            Label lblFullInterval =
                new Label();

            lblFullInterval.Text =
                "Interval (Seconds)";

            lblFullInterval.Location =
                new Point(100, 460);

            lblFullInterval.AutoSize = true;

            numFullInterval =
                new NumericUpDown();

            numFullInterval.Location =
                new Point(320, 455);

            numFullInterval.Minimum = 1;

            numFullInterval.Maximum = 3600;

            numFullInterval.Value = 60;

            // CHOOSE ALARM BUTTON
            btnBrowse =
                new Button();

            btnBrowse.Text =
                "Choose Alarm";

            btnBrowse.Size =
                new Size(170, 40);

            btnBrowse.Location =
                new Point(80, 530);

            btnBrowse.BackColor =
                Color.DodgerBlue;

            btnBrowse.ForeColor =
                Color.White;

            btnBrowse.FlatStyle =
                FlatStyle.Flat;

            btnBrowse.Click +=
                BtnBrowse_Click;

            // SAVE BUTTON
            btnSave =
                new Button();

            btnSave.Text =
                "Save Settings";

            btnSave.Size =
                new Size(170, 40);

            btnSave.Location =
                new Point(270, 530);

            btnSave.BackColor =
                Color.MediumSeaGreen;

            btnSave.ForeColor =
                Color.White;

            btnSave.FlatStyle =
                FlatStyle.Flat;

            btnSave.Click +=
                BtnSave_Click;

            // ALARM PATH
            lblAlarm =
                new Label();

            lblAlarm.Text =
                "No alarm selected";

            lblAlarm.Location =
                new Point(80, 600);

            lblAlarm.Size =
                new Size(380, 30);

            // ALARM FILE NAME
            lblAlarmFile =
                new Label();

            lblAlarmFile.Text = "";

            lblAlarmFile.Location =
                new Point(80, 630);

            lblAlarmFile.Size =
                new Size(380, 30);

            lblAlarmFile.Font =
                new Font(
                    "Segoe UI",
                    9,
                    FontStyle.Bold);

            lblAlarmFile.ForeColor =
                Color.DimGray;

            // STOP ALARM BUTTON
            btnStopAlarm =
                new Button();

            btnStopAlarm.Text =
                "STOP ALARM";

            btnStopAlarm.Size =
                new Size(360, 45);

            btnStopAlarm.Location =
                new Point(80, 690);

            btnStopAlarm.BackColor =
                Color.IndianRed;

            btnStopAlarm.ForeColor =
                Color.White;

            btnStopAlarm.FlatStyle =
                FlatStyle.Flat;

            btnStopAlarm.Click +=
                BtnStopAlarm_Click;

            // NOTIFY ICON
            notifyIcon =
                new NotifyIcon();

            notifyIcon.Icon =
                SystemIcons.Information;

            notifyIcon.Visible = true;

            notifyIcon.Text =
                "Advanced Battery Notifier";

            notifyIcon.DoubleClick +=
                NotifyIcon_DoubleClick;

            // TIMER
            timer =
                new Timer();

            timer.Interval = 5000;

            timer.Tick +=
                Timer_Tick;

            // FILE DIALOG
            openFileDialog =
                new OpenFileDialog();

            openFileDialog.Filter =
                "Audio Files|*.wav";

            // ADD CONTROLS
            Controls.Add(lblTitle);
            Controls.Add(lblBattery);
            Controls.Add(lblStatus);
            Controls.Add(progressBattery);

            Controls.Add(lblLowTitle);
            Controls.Add(lblLowPercent);
            Controls.Add(numLow);
            Controls.Add(lblLowInterval);
            Controls.Add(numLowInterval);

            Controls.Add(lblFullTitle);
            Controls.Add(lblFullPercent);
            Controls.Add(numFull);
            Controls.Add(lblFullInterval);
            Controls.Add(numFullInterval);

            Controls.Add(btnBrowse);
            Controls.Add(btnSave);

            Controls.Add(lblAlarm);
            Controls.Add(lblAlarmFile);

            Controls.Add(btnStopAlarm);
        }

        private void Timer_Tick(
            object sender,
            EventArgs e)
        {
            CheckBattery();
        }

        private void CheckBattery()
        {
            PowerStatus power =
                SystemInformation.PowerStatus;

            int batteryPercent =
                (int)(power.BatteryLifePercent * 100);

            bool isCharging =
                power.PowerLineStatus
                == PowerLineStatus.Online;

            lblBattery.Text =
                batteryPercent + "%";

            lblStatus.Text =
                isCharging
                ? "Charging"
                : "Not Charging";

            progressBattery.Value =
                batteryPercent;

            notifyIcon.Text =
                "Battery "
                + batteryPercent
                + "%";

            // BATTERY COLOR
            if (batteryPercent <= 30)
            {
                lblBattery.ForeColor =
                    Color.Red;
            }
            else if (batteryPercent <= 70)
            {
                lblBattery.ForeColor =
                    Color.DarkOrange;
            }
            else
            {
                lblBattery.ForeColor =
                    Color.Green;
            }

            // LOW BATTERY ALERT
            if (batteryPercent
                <= settings.LowBatteryLimit
                && !isCharging)
            {
                if ((DateTime.Now
                    - lastLowBatteryAlarm)
                    .TotalSeconds
                    >= settings.LowBatteryIntervalSeconds)
                {
                    ShowNotification(
                        "Low Battery",
                        "Battery tinggal "
                        + batteryPercent
                        + "%");

                    PlayAlarm();

                    lastLowBatteryAlarm =
                        DateTime.Now;
                }
            }
            else
            {
                StopAlarm();
            }

            // FULL BATTERY ALERT
            if (batteryPercent
                >= settings.FullBatteryLimit
                && isCharging)
            {
                if ((DateTime.Now
                    - lastFullBatteryAlarm)
                    .TotalSeconds
                    >= settings.FullBatteryIntervalSeconds)
                {
                    ShowNotification(
                        "Battery Full",
                        "Cabut charger laptop");

                    PlayAlarm();

                    lastFullBatteryAlarm =
                        DateTime.Now;
                }
            }
            else
            {
                StopAlarm();
            }
        }

        private void ShowNotification(
            string title,
            string message)
        {
            notifyIcon.ShowBalloonTip(
                5000,
                title,
                message,
                ToolTipIcon.Info);
        }

        private void PlayAlarm()
        {
            try
            {
                if (!string.IsNullOrEmpty(
                    settings.AlarmPath))
                {
                    player =
                        new SoundPlayer(
                            settings.AlarmPath);

                    player.PlayLooping();
                }
            }
            catch
            {
            }
        }

        private void StopAlarm()
        {
            try
            {
                if (player != null)
                {
                    player.Stop();
                }
            }
            catch
            {
            }
        }

        private void BtnStopAlarm_Click(
            object sender,
            EventArgs e)
        {
            StopAlarm();
        }

        private void BtnBrowse_Click(
            object sender,
            EventArgs e)
        {
            if (openFileDialog.ShowDialog()
                == DialogResult.OK)
            {
                settings.AlarmPath =
                    openFileDialog.FileName;

                lblAlarm.Text =
                    settings.AlarmPath;

                lblAlarmFile.Text =
                    Path.GetFileName(
                        settings.AlarmPath);

                SaveSettings();
            }
        }

        private void BtnSave_Click(
            object sender,
            EventArgs e)
        {
            settings.LowBatteryLimit =
                (int)numLow.Value;

            settings.FullBatteryLimit =
                (int)numFull.Value;

            settings.LowBatteryIntervalSeconds =
                (int)numLowInterval.Value;

            settings.FullBatteryIntervalSeconds =
                (int)numFullInterval.Value;

            SaveSettings();

            MessageBox.Show(
                "Settings saved",
                "Information",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void SaveSettings()
        {
            string json =
                JsonConvert.SerializeObject(
                    settings,
                    Formatting.Indented);

            File.WriteAllText(
                settingsPath,
                json);
        }

        private void LoadSettings()
        {
            if (File.Exists(settingsPath))
            {
                string json =
                    File.ReadAllText(
                        settingsPath);

                settings =
                    JsonConvert.DeserializeObject
                    <SettingsModel>(json);
            }
            else
            {
                settings =
                    new SettingsModel();
            }

            numLow.Value =
                settings.LowBatteryLimit;

            numFull.Value =
                settings.FullBatteryLimit;

            numLowInterval.Value =
                settings.LowBatteryIntervalSeconds;

            numFullInterval.Value =
                settings.FullBatteryIntervalSeconds;

            lblAlarm.Text =
                settings.AlarmPath;

            lblAlarmFile.Text =
                Path.GetFileName(
                    settings.AlarmPath);
        }

        protected override void OnResize(
            EventArgs e)
        {
            base.OnResize(e);

            if (WindowState
                == FormWindowState.Minimized)
            {
                Hide();

                notifyIcon.ShowBalloonTip(
                    2000,
                    "Battery Notifier",
                    "App minimized to tray",
                    ToolTipIcon.Info);
            }
        }

        private void NotifyIcon_DoubleClick(
            object sender,
            EventArgs e)
        {
            Show();

            WindowState =
                FormWindowState.Normal;
        }

        protected override void OnFormClosing(
            FormClosingEventArgs e)
        {
            if (e.CloseReason
                == CloseReason.UserClosing)
            {
                e.Cancel = true;

                Hide();
            }

            base.OnFormClosing(e);
        }
    }
}