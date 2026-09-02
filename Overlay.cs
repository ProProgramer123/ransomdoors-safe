using NAudio.Wave;
using System.Diagnostics;
using System.Runtime.InteropServices;

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

            [DllImport("user32.dll")]
            public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        }

        // ----------------------------- CONSTRUCTOR AND OVERLAY SETUP -----------------------------
        public Overlay() { InitializeComponent(); }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
                cp.ExStyle |= 0x00080000; // WS_EX_LAYERED
                return cp;
            }
        }






        // ----------------------------- RANSOM PHASES -----------------------------

        /// <summary>
        /// First phase of Ransom
        /// </summary>
        /// <returns>true if the mouse moved during the warning phase</returns>
        public async Task<bool> RansomWarning()
        {
            WaveOut spawnSound = SoundHelper.Create(Properties.Resources.spawn);
            spawnSound.Play();

            // Shows Ransom's face randomly on the screen
            Global.RandomPosControl(pc_ransom);
            pc_ransom.Visible = true;

            await Task.Delay(500);
            // Mouse spy phase
            Global.lastRegisteredMousePos = MousePosition;
            Global.spyingMouse = true;

            // Hide Ransom's face and show the warning sign
            pc_stopsign.Visible = true;
            pc_ransom.Visible = false;

            await Task.Delay(500);
            // End of spy phase

            Global.spyingMouse = false;

            // Hide the warning sign, show Ransom face's on the center with a red background
            pc_stopsign.Visible = false;
            pc_ransom.Visible = true;
            Global.CenterControl(pc_ransom);
            this.BackColor = Color.DarkRed;

            await Task.Delay(100);

            this.BackColor = this.TransparencyKey;
            pc_ransom.Visible = false;

            return MousePosition != Global.lastRegisteredMousePos; // Returns true if the mouse moved during the spy phase
        }

        /// <summary>
        /// Second phase of Ransom, Jumpscare+Downloading effect
        /// </summary>
        public async Task DownloadJumpscare()
        {
            WaveOut attackSound = SoundHelper.Create(Properties.Resources.attack);
            attackSound.Play();

            Global.CenterControl(pc_attack);
            Point attack_center = pc_attack.Location; // Store the center position of the attack image for the shake effect
            pc_attack.Visible = true;
            this.BackColor = Color.DarkRed;

            // Ransom Face Shake Effect
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

            // Downloading screen
            pc_ransom.Visible = false;
            pc_attack.Visible = false;

            WaveOut installSound = SoundHelper.Create(Properties.Resources.install);
            installSound.Play();

            // Background signs effect
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

                // Clear the background signs
                foreach (PictureBox item in list)
                {
                    try { this.Invoke(() => item.Dispose()); }
                    catch { break; }
                }
            }) { IsBackground = true }.Start();

            Global.CenterControl(txt_download);
            Global.CenterControl(pb_download);
            pb_download.Location = new Point(pb_download.Location.X, pb_download.Location.Y+50); // Add a lil offset

            txt_download.Visible = true;
            pb_download.Visible = true;
            pb_download.Value = 100;

            // Text and download bar shake/glitch effect
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

        /// <summary>
        /// Third phase of Ransom, the actual ransom, plays the music, shows the Ransomed window, etc...
        /// </summary>
        public async Task<bool> Ransomed()
        {
            // Red Flash
            this.BackColor = Color.Red;
            new Thread(async () =>
            {
                try
                {
                    for (int i = 0; i <= 50; i++)
                    {
                        await Task.Delay(1);
                        this.Invoke(() => this.Opacity = 1 - (i * 2 / 100.0) );
                    }
                    this.Invoke(() =>
                    {
                        this.BackColor = this.TransparencyKey;
                        this.Opacity = 100;
                    });
                }
                catch { }
            }) { IsBackground = true }.Start();

            // OST
            WaveOut layer1 = SoundHelper.Create(Properties.Resources.layer1);
            WaveOut layer2 = SoundHelper.Create(Properties.Resources.layer2);
            WaveOut layer3 = SoundHelper.Create(Properties.Resources.layer3);

            Global.ransomLeft = 500;
            Global.underRansom = true;
            Global.RansomPayed = () => // Ransom payed event
            {
                layer1.Stop();
                layer2.Stop();
                layer3.Stop();

                this.Invoke(ResetRansom);
            };

            // Random flashing Ransom faces
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

            // Shows the main ransom window
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

            // If the code reaches here, this means the user didn't pay the ransom in time 
            ransomedForm.Close();
            return true;
        }

        /// <summary>
        /// Shows the jumpscare and crashes/shutdowns the computer depending if it's started as admin or not
        /// </summary>
        public async Task CrashJumpscare()
        {
            WaveOut attackSound = SoundHelper.Create(Properties.Resources.attack);
            attackSound.Play();

            this.BackColor = Color.DarkRed;
            Global.CenterControl(pc_attack);
            Point attack_center = pc_attack.Location; // Store the center position of the attack image for the shake effect
            pc_attack.Visible = true;
            // Shake effect
            new Thread(async () =>
            {
                for (int i = 0; i <= 25; i++)
                {
                    await Task.Delay(10);
                    try { this.Invoke(() => pc_attack.Location = new Point(attack_center.X + Global.rng.Next(-40, 40), attack_center.Y + Global.rng.Next(-40, 40))); }
                    catch { break; }
                }
            }) { IsBackground = true }.Start();

            await Task.Delay(1000);

            if (Global.IsAdministrator())
            {
                // Basically morphs the process into a critical process, then kills it, causing a BSOD
                Global.IntoCriticalProcess();
                this.Close();
            } 
            else
            {
                // If not admin, just shutdown the computer
                Process.Start("shutdown", "/s /t 0");
            }
        }








        // ----------------------------- CORE -----------------------------

        /// <summary>
        /// Reset the app to it's ready state
        /// </summary>
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

        /// <summary>
        /// Summons Ransom, triggers each step of the ransom process
        /// </summary>
        public async void SpawnRansom()
        {
            if (!Global.canAttack) return;
            Global.canAttack = false;

            bool mouseMoved = await RansomWarning();
            if (mouseMoved) // User moved the mouse
            {
                try { GoldCoinManager.CreateRandomCoins(8); }
                catch { }

                await DownloadJumpscare();

                if (await Ransomed()) // If user didn't pay the ransom in time
                {
                    Global.underRansom = false;
                    await CrashJumpscare();
                    this.Close();
                }
            }
            else ResetRansom(); // User didn't move the mouse, dodged the ransom
        }

        /// <summary>
        /// Ransom loop, keeps summoning ransom randomly
        /// </summary>
        private async void RansomLoop()
        {
            while (!this.IsDisposed)
            {
                try { this.Invoke(SpawnRansom); } // Invoke via the UI thread to avoid cross-thread exceptions (RansomLoop will be called only from another thread soooo)
                catch { break; }

                await Task.Delay(Global.rng.Next(Global.minRansomTime*1000, Global.maxRansomTime*1000)); // Ransom Debounce
            }
        }

        /// <summary>
        /// Setup the TrayIcon to close the app (cause there's no other way to close it)
        /// </summary>
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

        // ----------------------------- EVENT HANDLERS -----------------------------

        private void Overlay_Load(object sender, EventArgs e)
        {
            // Set the overlay to cover the entire screen
            this.Bounds = SystemInformation.VirtualScreen; // Idk if ScreenBounds or this is better
            this.Location = new Point(0, 0);

            // Register .gold files and set the app to it's ready state
            FileTypeRegister.RegisterIconForExtension(".gold", Properties.Resources.GoldIco, "GoldFile");
            ResetRansom();
            SetupTrayIcon();
            SetupTopMostTimer();

            // Starts the RansomLoop (in a new thread to avoid blocking the UI thread)
            new Thread(RansomLoop) { IsBackground = true }.Start();
        }

        /// <summary>
        /// TopMost alone doesn't stick, other windows (especially elevated ones) can still cover the overlay,
        /// so this keeps shoving it back to the front without stealing focus/keyboard input
        /// </summary>
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
            if (Global.underRansom) // If the user is under ransom, prevent closing the overlay
            {
                e.Cancel = true;
                return;
            }
        }
    }
}
