using NAudio.Wave;

namespace rans0m
{
    public partial class ThankYou : Form
    {
        public ThankYou()
        {
            InitializeComponent();
        }

        private void ThankYou_Load(object sender, EventArgs e)
        {
            Global.RansomPayed();
            WaveOut thankYouSfx = SoundHelper.Create(Properties.Resources.cash);
            thankYouSfx.Play();

            Global.CenterControl(this);

            Task.Delay(3000).ContinueWith(_ =>
            {
                Invoke(() => Dispose());
            });

        }
    }
}
