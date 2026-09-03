using NAudio.Wave;

namespace rans0m
{
    public partial class Overlay : Form
    {
        private NotifyIcon? trayIcon;
        private System.Windows.Forms.Timer? topMostTimer;

        private static class NativeMethods
        {
            public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
            public const uint SWP_NOMOVE = 0x0002;
            public const uint SWP_NOSIZE = 0x0001;
            public const uint SWP_NOACTIVATE = 0x0010;

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        }

        public Overlay() { InitializeComponent(); }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020;
                cp.ExStyle |= 0x00080000;
                return cp;
            }
        }

        public async Task<bool> RansomWarning()
        {
            WaveOut spawnSound = SoundHelper.Create(Properties.Resources.spawn);
            spawnSound.Play();

            Global.RandomPosControl(pc_ransom);
            pc_ransom.Visible = true;

            await Task.Delay(500);
            Global.lastRegisteredMousePos = MousePosition;
            Global.spyingMouse = true;

            pc_stopsign.Visible = true;
            pc_ransom.Visible = false;

            await Task.Delay(500);
            Global.spyingMouse = false;

            pc_stopsign.Visible = false;
            pc_ransom.Visible = true;
            Global.CenterControl(pc_ransom);
            this.BackColor = Color.DarkRed;

            await Task.Delay(100);

            this.BackColor = this.TransparencyKey;
            pc_ransom.Visible = false;

            return MousePosition != Global.lastRegisteredMousePos;
        }

        public async Task DownloadJumpscare()
        {
            WaveOut attackSound = SoundHelper.Create(Properties.Resources.attack);
            attackSound.Play();

            Global.CenterControl(pc_attack);
            Point attack_center = pc_attack.Location;
            pc_attack.Visible = true;
            this.BackColor = Color.DarkRed;

            new Thread(async () =>
            {
                for (int i = 0; i <= 25; i++)
                {
                    await Task.Delay(20);
                    try { this.Invoke(() => pc_attack.Location = new Point(attack_center.X + Global.rng.Next(-40, 40), attack_center.Y + Global.rng.Next(-40, 40))); }
                    catch { break; }
                }
            }) { IsBackground = true }.Start();

            await Task.Delay(800);

            pc_ransom.Visible = false;
            pc_attack.Visible = false;

            WaveOut installSound = SoundHelper.Create(Properties.Resources.install);
            installSound.Play();

            new Thread(async () =>
            {
                List<PictureBox> list = new();
                try
                {
                    for (int i = 0; i <= 70; i++)
                    {
                        await Task.Delay(10);
                        this.Invoke(() =>
                        {
                            PictureBox stopsigndup = new PictureBox();
                            stopsigndup.Image = Properties.Resources.stop_sign;
                            stopsigndup.Size = new Size(192, 192);
                            Global.RandomPosControl(stopsigndup);
                            stopsigndup.SizeMode = PictureBoxSizeMode.StretchImage;

                            this.Controls.Add(stopsigndup);
                            list.Add(stopsigndup);
                        });
                    }
                }
                catch { }

                foreach (PictureBox item in list)
                {
                    try { this.Invoke(() => item.Dispose()); }
                    catch { break; }
                }
            }) { IsBackground = true }.Start();

            Global.CenterControl(txt_download);
            Global.CenterControl(pb_download);
            pb_download.Location = new Point(pb_download.Location.X, pb_download.Location.Y + 50);

            txt_download.Visible = true;
            pb_download.Visible = true;
            pb_download.Value = 100;

            new Thread(() =>
            {
                for (int i = 0; i <= 40; i++)
                {
                    Thread.Sleep(40);
                    try
                    {
                        this.Invoke(() =>
                        {
                            txt_download.Font = new Font(txt_download.Font.FontFamily, txt_download.Font.Size + Global.rng.Next(-2, 3));
                            txt_download.Location = new Point(txt_download.Location.X + Global.rng.Next(-5, 5), txt_download.Location.Y + Global.rng.Next(-5, 5));
                            pb_download.Location = new Point(pb_download.Location.X + Global.rng.Next(-5, 5), pb_download.Location.Y + Global.rng.Next(-5, 5));
                        });
                    }
                    catch { break; }
                }
            }) { IsBackground = true }.Start();

            await Task.Delay(1200);

            pc_ransom.Visible = false;
            pc_attack.Visible = false;
            txt_download.Visible = false;
            pb_download.Visible = false;
            pb_download.Value = 0;
        }

        public async Task<bool> Ransomed()
        {
            this.BackColor = Color.Red;
            new Thread(async () =>
            {
                try
                {
                    for (int i = 0; i <= 50; i++)
                    {
                        await Task.Delay(1);
                        this.Invoke(() => this.Opacity = 1 - (i * 2 / 100.0));
                    }
                    this.Invoke(() =>
                    {
                        this.BackColor = this.TransparencyKey;
                        this.Opacity = 100;
                    });
                }
                catch { }
            }) { IsBackground = true }.Start();

            WaveOut layer1 = SoundHelper.Create(Properties.Resources.layer1);
            WaveOut layer2 = SoundHelper.Create(Properties.Resources.layer2);
            WaveOut layer3 = SoundHelper.Create(Properties.Resources.layer3);

            Global.ransomLeft = 500;
            Global.underRansom = true;
            Global.RansomPayed = () =>
            {
                layer1.Stop();
                layer2.Stop();
                layer3.Stop();

                this.Invoke(ResetRansom);
            };

            new Thread(async () =>
            {
                while (Global.underRansom)
                {
                    await Task.Delay(Global.rng.Next(5000));
                    try
                    {
                        this.Invoke((MethodInvoker)async delegate
                        {
                            for (int i = 0; i <= Global.rng.Next(1, 5); i++)
                            {
                                PictureBox ransomFace = new PictureBox();
                                ransomFace.Image = Properties.Resources.ransom_random;
                                int size = Global.rng.Next(50, 400);
                                ransomFace.Size = new Size(size, size);

                                Global.RandomPosControl(ransomFace);
                                ransomFace.SizeMode = PictureBoxSizeMode.StretchImage;

                                this.Controls.Add(ransomFace);
                                await Task.Delay(25);
                                this.Controls.Remove(ransomFace);
                            }
                        });
                    }
                    catch { break; }
                }
            }) { IsBackground = true }.Start();

            Ransomed ransomedForm = new Ransomed();
            ransomedForm.Show();

            layer1.Play();
            await Task.Delay(26000);
            if (!Global.underRansom) return false;

            layer2.Play();
            await Task.Delay(26000);
            if (!Global.underRansom) return false;

            layer3.Play();
            await Task.Delay(26000);
            if (!Global.underRansom) return false;

            ransomedForm.Close();
            return true;
        }

        /// <summary>
        /// Shows the losing jumpscare and ends the ransom game without
        /// shutting down Windows, killing processes, or creating a critical process.
        /// </summary>
        public async Task LoseGame()
        {
            WaveOut attackSound = SoundHelper.Create(Properties.Resources.attack);
            attackSound.Play();

            this.BackColor = Color.DarkRed;
            Global.CenterControl(pc_attack);
            Point attack_center = pc_attack.Location;
            pc_attack.Visible = true;

            new Thread(async () =>
            {
                for (int i = 0; i <= 25; i++)
                {
                    await Task.Delay(10);
                    try
                    {
                        this.Invoke(() => pc_attack.Location = new Point(
                            attack_center.X + Global.rng.Next(-40, 40),
                            attack_center.Y + Global.rng.Next(-40, 40)));
                    }
                    catch { break; }
                }
            }) { IsBackground = true }.Start();

            await Task.Delay(1000);

            // Losing the ransom is now just a game-over state.
            // ResetRansom removes the .gold files and clears the ransom state.
            ResetRansom();
            Close();
        }

        public void ResetRansom()
        {
            GoldCoinManager.DeleteAllCoins();
            Global.canAttack = true;
            Global.RansomPayed = null;
            Global.underRansom = false;
            Global.ransomLeft = 0;

            txt_download.Font = new Font("Consolas", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            pc_ransom.Visible = false;
            pc_attack.Visible = false;
            txt_download.Visible = false;
            pb_download.Visible = false;
            pc_stopsign.Visible = false;
            pb_download.Value = 0;

            this.BackColor = this.TransparencyKey;
        }

        public async void SpawnRansom()
        {
            if (!Global.canAttack) return;
            Global.canAttack = false;

            bool mouseMoved = await RansomWarning();
            if (mouseMoved)
            {
                try { GoldCoinManager.CreateRandomCoins(8); }
                catch { }

                await DownloadJumpscare();

                if (await Ransomed())
                {
                    Global.underRansom = false;
                    await LoseGame();
                }
            }
            else ResetRansom();
        }

        private async void RansomLoop()
        {
            while (!this.IsDisposed)
            {
                try { this.Invoke(SpawnRansom); }
                catch { break; }

                await Task.Delay(Global.rng.Next(Global.minRansomTime * 1000, Global.maxRansomTime * 1000));
            }
        }

        private void SetupTrayIcon()
        {
            ContextMenuStrip trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Close").Click += (s, e) =>
            {
                if (!Global.canAttack) return;
                Close();
            };

            trayIcon = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) ?? SystemIcons.Application,
                ContextMenuStrip = trayMenu,
                Text = "RANS0M",
                Visible = true
            };

            this.FormClosed += (s, e) =>
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
            };
        }

        private void Overlay_Load(object sender, EventArgs e)
        {
            this.Bounds = SystemInformation.VirtualScreen;
            this.Location = new Point(0, 0);

            FileTypeRegister.RegisterIconForExtension(".gold", Properties.Resources.GoldIco, "GoldFile");
            ResetRansom();
            SetupTrayIcon();
            SetupTopMostTimer();

            new Thread(RansomLoop) { IsBackground = true }.Start();
        }

        private void SetupTopMostTimer()
        {
            topMostTimer = new System.Windows.Forms.Timer { Interval = 500 };
            topMostTimer.Tick += (s, e) =>
            {
                if (IsDisposed) return;
                NativeMethods.SetWindowPos(Handle, NativeMethods.HWND_TOPMOST, 0, 0, 0, 0,
                    NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE);
            };
            topMostTimer.Start();

            this.FormClosed += (s, e) => topMostTimer.Stop();
        }

        private void Overlay_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Global.underRansom)
            {
                e.Cancel = true;
                return;
            }
        }
    }
}
