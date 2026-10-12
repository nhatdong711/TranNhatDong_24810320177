namespace QuanLySinhVien;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new FormStudent { StartPosition = FormStartPosition.CenterScreen });
    }
}
