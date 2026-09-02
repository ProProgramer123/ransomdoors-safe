namespace rans0m
{
    public partial class TauntWindow : Form
    {
        public TauntWindow()
        {
            InitializeComponent();
        }

        private void TauntWindow_Load(object sender, EventArgs e)
        {
            // Sets random window title, image and size
            Text = Global.tauntTitles[Global.rng.Next(Global.tauntTitles.Count)];
            BackgroundImage = Global.tauntImages[Global.rng.Next(Global.tauntImages.Count)];
            Size = new Size( Global.rng.Next(200, 400), Global.rng.Next(200, 400) );
            MaximumSize = Size;
            MinimumSize = Size;

            // Random pos
            Global.RandomPosControl(this);

            // Glitch Idle Effect
            new Thread(async () => Global.GlitchIdle(this)) { IsBackground = true }.Start();

            // Closes after 4-10 seconds
            new Thread(async () =>
            {
                await Task.Delay(Global.rng.Next(4000, 10 * 1000));
                if (IsDisposed) return;
                this.Invoke(() => Dispose());
            }) { IsBackground = true }.Start();
        }
    }
}
