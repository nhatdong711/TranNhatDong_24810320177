using System.Drawing.Text;

namespace QuanLySinhVien;

public class FormMain : Form
{
    private readonly MenuStrip menuStrip = new();
    private readonly ToolStrip toolStrip = new() { ImageScalingSize = new Size(20, 20) };
    private readonly StatusStrip statusStrip = new();
    private readonly ContextMenuStrip mdiContextMenu = new();

    private readonly ToolStripStatusLabel lblUser = new();
    private readonly ToolStripStatusLabel lblMessage = new("Sẵn sàng") { Spring = true, TextAlign = ContentAlignment.MiddleLeft };
    private readonly ToolStripProgressBar statusProgress = new() { Size = new Size(120, 16), Visible = false };
    private readonly ToolStripStatusLabel lblPercent = new() { Visible = false };
    private readonly ToolStripStatusLabel lblTime = new();
    private readonly System.Windows.Forms.Timer clock = new() { Interval = 1000 };

    private readonly string _userName;

    public bool LoggedOut { get; private set; }

    public FormMain(string userName)
    {
        _userName = userName;

        Text = "Hệ thống quản lý";
        IsMdiContainer = true;
        WindowState = FormWindowState.Maximized;
        MinimumSize = new Size(900, 600);

        BuildMenu();
        BuildToolStrip();
        BuildStatusStrip();
        BuildContextMenu();

        Controls.Add(toolStrip);
        Controls.Add(menuStrip);
        Controls.Add(statusStrip);
        MainMenuStrip = menuStrip;

        clock.Tick += (_, _) => UpdateClock();
        clock.Start();
        UpdateClock();
    }

    private void BuildMenu()
    {
        var mnuSystem = new ToolStripMenuItem("&Hệ thống");
        mnuSystem.DropDownItems.Add("Đăng &xuất", null, (_, _) => Logout());
        mnuSystem.DropDownItems.Add(new ToolStripSeparator());
        mnuSystem.DropDownItems.Add(new ToolStripMenuItem("&Thoát", null, (_, _) => Close()) { ShortcutKeys = Keys.Alt | Keys.F4 });

        var mnuCatalog = new ToolStripMenuItem("&Danh mục");
        mnuCatalog.DropDownItems.Add(new ToolStripMenuItem("Quản lý &Sinh viên", null, (_, _) => OpenChild<FormStudent>()) { ShortcutKeys = Keys.Control | Keys.D1 });
        mnuCatalog.DropDownItems.Add(new ToolStripMenuItem("Quản lý Sản &phẩm", null, (_, _) => OpenChild<FormProduct>()) { ShortcutKeys = Keys.Control | Keys.D2 });

        var mnuWindow = new ToolStripMenuItem("&Cửa sổ");
        mnuWindow.DropDownItems.Add("&Cascade", null, (_, _) => LayoutMdi(MdiLayout.Cascade));
        mnuWindow.DropDownItems.Add("Tile &Horizontal", null, (_, _) => LayoutMdi(MdiLayout.TileHorizontal));
        mnuWindow.DropDownItems.Add("Tile &Vertical", null, (_, _) => LayoutMdi(MdiLayout.TileVertical));
        mnuWindow.DropDownItems.Add(new ToolStripSeparator());
        mnuWindow.DropDownItems.Add("Đóng &tất cả", null, (_, _) => CloseAllChildren());

        menuStrip.Items.AddRange(new ToolStripItem[] { mnuSystem, mnuCatalog, mnuWindow });
        menuStrip.MdiWindowListItem = mnuWindow;
    }

    private void BuildToolStrip()
    {
        toolStrip.Items.Add(ToolButton("Sinh viên", "", "Quản lý Sinh viên (Ctrl+1)", () => OpenChild<FormStudent>()));
        toolStrip.Items.Add(ToolButton("Sản phẩm", "", "Quản lý Sản phẩm (Ctrl+2)", () => OpenChild<FormProduct>()));
        toolStrip.Items.Add(new ToolStripSeparator());
        toolStrip.Items.Add(ToolButton("Nạp dữ liệu", "", "Nạp lại dữ liệu cho cửa sổ đang mở", ReloadActiveChild));
        toolStrip.Items.Add(new ToolStripSeparator());
        toolStrip.Items.Add(ToolButton("Đăng xuất", "", "Đăng xuất", Logout));
    }

    private void BuildStatusStrip()
    {
        lblUser.Text = $"Người dùng: {_userName}";
        lblUser.BorderSides = lblTime.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Right;
        statusStrip.Items.AddRange(new ToolStripItem[] { lblUser, lblMessage, statusProgress, lblPercent, lblTime });
    }

    private void BuildContextMenu()
    {
        mdiContextMenu.Items.Add("Quản lý Sinh viên", null, (_, _) => OpenChild<FormStudent>());
        mdiContextMenu.Items.Add("Quản lý Sản phẩm", null, (_, _) => OpenChild<FormProduct>());
        mdiContextMenu.Items.Add(new ToolStripSeparator());
        mdiContextMenu.Items.Add("Sắp xếp Cascade", null, (_, _) => LayoutMdi(MdiLayout.Cascade));
        mdiContextMenu.Items.Add("Đóng tất cả cửa sổ", null, (_, _) => CloseAllChildren());

        foreach (MdiClient client in Controls.OfType<MdiClient>())
            client.ContextMenuStrip = mdiContextMenu;
    }

    private static ToolStripButton ToolButton(string text, string glyph, string tooltip, Action onClick)
    {
        var button = new ToolStripButton(text, GlyphIcon(glyph))
        {
            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
            ToolTipText = tooltip,
        };
        button.Click += (_, _) => onClick();
        return button;
    }

    private static Bitmap GlyphIcon(string glyph)
    {
        var bmp = new Bitmap(20, 20);
        using var g = Graphics.FromImage(bmp);
        using var font = new Font("Segoe MDL2 Assets", 12f, GraphicsUnit.Pixel);
        using var brush = new SolidBrush(Color.FromArgb(0, 90, 158));
        using var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.TextRenderingHint = TextRenderingHint.AntiAlias;
        g.DrawString(glyph, font, brush, new RectangleF(0, 0, 20, 20), format);
        return bmp;
    }

    private void OpenChild<T>() where T : ChildFormBase, new()
    {
        T? child = MdiChildren.OfType<T>().FirstOrDefault();
        if (child != null)
        {
            if (child.WindowState == FormWindowState.Minimized) child.WindowState = FormWindowState.Normal;
            child.Activate();
            SetStatus($"\"{child.Text}\" đã mở sẵn - đã chuyển tới cửa sổ.");
            return;
        }

        child = new T { MdiParent = this, CurrentUser = _userName };
        child.StatusMessage += SetStatus;
        child.ProgressReported += SetProgress;
        child.FormClosed += (_, _) => { HideProgress(); SetStatus($"Đã đóng \"{child.Text}\"."); };
        child.Show();
        SetStatus($"Đã mở \"{child.Text}\".");
    }

    private async void ReloadActiveChild()
    {
        if (ActiveMdiChild is ChildFormBase child)
            await child.ReloadDataAsync();
        else
            SetStatus("Chưa có cửa sổ nào đang mở.");
    }

    private void CloseAllChildren()
    {
        foreach (Form child in MdiChildren) child.Close();
    }

    public void SetStatus(string message) => lblMessage.Text = message;

    public void SetProgress(int percent)
    {
        statusProgress.Visible = lblPercent.Visible = true;
        statusProgress.Value = Math.Clamp(percent, 0, 100);
        lblPercent.Text = $"{percent}%";
        if (percent >= 100) HideProgress();
    }

    private void HideProgress() => statusProgress.Visible = lblPercent.Visible = false;

    private void UpdateClock() => lblTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

    private void Logout()
    {
        if (MessageBox.Show(this, "Bạn có chắc muốn đăng xuất?", "Đăng xuất",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        LoggedOut = true;
        Close();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        clock.Stop();
        base.OnFormClosed(e);
    }
}
