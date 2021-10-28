using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using AmbientLightingLib;

namespace AmbientLightingUi
{
    public partial class Form1 : Form
    {
        /* Engine */
        private AmbientLightEngine _engine;

        /* Diagnostics */
        private Stopwatch _stopwatch;

        public Form1()
        {
            InitializeComponent();

            _engine = new AmbientLightEngine();
            _engine.Initialize(new ScreenRgbProvider());
            //_engine.Initialize(new SoundRgbProvider());
            _engine.CalculateRgbValueBegin += EngineOnCalculateRgbValueBegin;
            _engine.CalculateRgbValueEnd += EngineOnCalculateRgbValueEnd;

            _stopwatch = new Stopwatch();

            notifyIcon1.MouseDoubleClick += NotifyIcon1OnMouseDoubleClick;
            notifyIcon1.ContextMenu = new ContextMenu();
            notifyIcon1.ContextMenu.MenuItems.Add(new MenuItem("Enable", btnEnable_Click));
            notifyIcon1.ContextMenu.MenuItems.Add(new MenuItem("Disable", btnDisable_Click));
            notifyIcon1.ContextMenu.MenuItems.Add(new MenuItem("Exit", NotifyIcon1_ContextMenu_Exit_Click));

            this.Closing += OnClosing;
        }

        private void NotifyIcon1_ContextMenu_Exit_Click(object sender, EventArgs e)
        {
            _exit();
        }

        private void NotifyIcon1OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            Visible = true;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            Visible = false;
            e.Cancel = true;
        }

        private void EngineOnCalculateRgbValueBegin(object sender, EventArgs e)
        {
            _stopwatch.Restart();
        }

        private void EngineOnCalculateRgbValueEnd(object sender, ScreenColorEventArgs e)
        {
            _stopwatch.Stop();
            statusLabelProcessingTime.Text = $@"{_stopwatch.ElapsedMilliseconds}ms";
            statusLabelColors.Text = $@"R:{e.Red}, G:{e.Green}, B:{e.Blue}";
        }

        private void btnEnable_Click(object sender, EventArgs e)
        {
            _engine.Start();
        }

        private void btnDisable_Click(object sender, EventArgs e)
        {
            _engine.Stop();
            statusLabelProcessingTime.Text = "N/A";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _exit();
        }

        private void _exit()
        {
            if (_engine.IsRunning)
                _engine.Stop();

            notifyIcon1.Visible = false;

            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            statusLabelProcessingTime.Text = statusLabelColors.Text = string.Empty;
            btnEnable_Click(btnEnable, EventArgs.Empty);
        }
    }
}
