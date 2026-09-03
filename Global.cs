using System.Security.Principal;

namespace rans0m
{
    public class Global
    {
        // ----------------------------- CONFIGURATION -----------------------------
        public static readonly int minRansomTime = 26 * 3;
        public static readonly int maxRansomTime = 10 * 60;

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
            if (spyingMouse)
            {
                lastRegisteredMousePos = new Point(-1, -1);
            }
        }

        public static void RandomPosControl(Control control)
        {
            int x = Global.rng.Next(0, Global.screenBounds.Width - control.Width);
            int y = Global.rng.Next(0, Global.screenBounds.Height - control.Height);

            control.Location = new Point(x, y);
        }

        public static void CenterControl(Control control)
        {
            control.Location = new Point(
                (Global.screenBounds.Width / 2) - control.Width / 2,
                (Global.screenBounds.Height / 2) - control.Height / 2);
        }

        public async static void GlitchIdle(Control control, bool divideAndTaunt = false)
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
                        if (divideAndTaunt && Global.rng.Next(1, 100) <= 2)
                        {
                            x = Global.rng.Next(0, Global.screenBounds.Width - control.Width);
                            y = Global.rng.Next(0, Global.screenBounds.Height - control.Height);

                            TauntWindow tauntWindow = new TauntWindow();
                            tauntWindow.Show();
                        }

                        control.Location = new Point(
                            x + Global.rng.Next(-5, 5),
                            y + Global.rng.Next(-5, 5));
                    });
                }
                catch { break; }
            }
        }
    }
}
