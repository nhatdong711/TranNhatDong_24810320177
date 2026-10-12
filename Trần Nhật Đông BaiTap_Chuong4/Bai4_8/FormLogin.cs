namespace QuanLySinhVien;

public class FormLogin : Form
{
    private const string DemoUser = "admin";
    private const string DemoPassword = "123";

    private readonly TextBox txtUser = new() { Width = 180, Text = DemoUser };
    private readonly TextBox txtPassword = new() { Width = 180, UseSystemPasswordChar = true };

    public string UserName => txtUser.Text.Trim();

    public FormLogin()
    {
        Text = "Đăng nhập";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Padding = new Padding(12);

        var btnLogin = new Button { Text = "Đăng nhập", AutoSize = true };
        var btnExit = new Button { Text = "Thoát", AutoSize = true, DialogResult = DialogResult.Cancel };
        btnLogin.Click += (_, _) => Login();

        var buttons = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill };
        buttons.Controls.AddRange(new Control[] { btnExit, btnLogin });

        var table = new TableLayoutPanel { AutoSize = true, ColumnCount = 2, Dock = DockStyle.Fill };
        table.Controls.Add(new Label { Text = "Tên đăng nhập:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        table.Controls.Add(txtUser, 1, 0);
        table.Controls.Add(new Label { Text = "Mật khẩu:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
        table.Controls.Add(txtPassword, 1, 1);
        var hint = new Label { Text = $"(Demo: {DemoUser} / {DemoPassword})", AutoSize = true, ForeColor = SystemColors.GrayText };
        table.Controls.Add(hint, 1, 2);
        table.Controls.Add(buttons, 0, 3);
        table.SetColumnSpan(buttons, 2);
        Controls.Add(table);

        AcceptButton = btnLogin;
        CancelButton = btnExit;
        ActiveControl = txtPassword;
    }

    private void Login()
    {
        if (UserName == DemoUser && txtPassword.Text == DemoPassword)
        {
            DialogResult = DialogResult.OK;
            return;
        }
        MessageBox.Show(this, "Sai tên đăng nhập hoặc mật khẩu.", "Đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        txtPassword.SelectAll();
        txtPassword.Focus();
    }
}
