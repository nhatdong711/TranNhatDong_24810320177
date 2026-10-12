namespace QuanLySinhVien;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        while (true)
        {
            using var login = new FormLogin();
            if (login.ShowDialog() != DialogResult.OK) break;

            using var main = new FormMain(login.UserName);
            Application.Run(main);
            if (!main.LoggedOut) break;
        }
    }
}
