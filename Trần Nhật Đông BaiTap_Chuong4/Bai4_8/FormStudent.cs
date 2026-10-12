using System.ComponentModel;
using System.Text;

namespace QuanLySinhVien;

public class FormStudent : ChildFormBase
{
    private BindingList<Student> students = new();
    private readonly BindingSource bindingSource = new();

    private readonly DataGridView dgvStudents = new();
    private readonly TextBox txtId = new();
    private readonly TextBox txtName = new();
    private readonly TextBox txtAge = new();
    private readonly ComboBox cboGender = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly PictureBox picAvatar = new()
    {
        Size = new Size(140, 160),
        SizeMode = PictureBoxSizeMode.Zoom,
        BorderStyle = BorderStyle.FixedSingle,
    };
    private readonly Button btnChooseImage = new() { Text = "Chọn ảnh...", AutoSize = true };
    private readonly Button btnAdd = new() { Text = "Thêm", AutoSize = true };
    private readonly Button btnDelete = new() { Text = "Xóa", AutoSize = true };
    private readonly Button btnUpdate = new() { Text = "Cập nhật", AutoSize = true };
    private readonly Button btnExport = new() { Text = "Xuất file CSV", AutoSize = true };
    private readonly Button btnLoad = new() { Text = "Nạp dữ liệu", AutoSize = true };
    private readonly ProgressBar progressBar = new() { Dock = DockStyle.Fill };
    private readonly Label lblPercent = new() { Text = "0%", Dock = DockStyle.Right, Width = 50, TextAlign = ContentAlignment.MiddleRight };
    private readonly TableLayoutPanel inputTable = new();
    private readonly ContextMenuStrip gridMenu = new();

    private string? _shownAvatarPath;

    public FormStudent()
    {
        Text = "Quản lý Sinh viên";
        ClientSize = new Size(900, 520);
        MinimumSize = new Size(700, 450);

        BuildLayout();
        BuildGridContextMenu();
        SetupBinding();

        btnChooseImage.Click += (_, _) => ChooseAvatar();
        btnAdd.Click += (_, _) => AddStudent();
        btnDelete.Click += (_, _) => DeleteStudent();
        btnUpdate.Click += (_, _) => UpdateStudent();
        btnExport.Click += (_, _) => ExportCsv();
        btnLoad.Click += async (_, _) => await ReloadDataAsync();
        txtName.KeyPress += (_, e) => e.Handled = !char.IsControl(e.KeyChar) && !IsNameChar(e.KeyChar);
        txtAge.KeyPress += (_, e) => e.Handled = !char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar);

        Shown += async (_, _) => await ReloadDataAsync();
    }

    private void BuildLayout()
    {
        dgvStudents.Dock = DockStyle.Fill;
        dgvStudents.AutoGenerateColumns = false;
        dgvStudents.ReadOnly = true;
        dgvStudents.AllowUserToAddRows = false;
        dgvStudents.AllowUserToDeleteRows = false;
        dgvStudents.MultiSelect = false;
        dgvStudents.RowHeadersVisible = false;
        dgvStudents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvStudents.BackgroundColor = SystemColors.Window;
        dgvStudents.Columns.Add(TextColumn(nameof(Student.StudentId), "Mã SV", 15));
        dgvStudents.Columns.Add(TextColumn(nameof(Student.FullName), "Họ tên", 35));
        dgvStudents.Columns.Add(TextColumn(nameof(Student.Age), "Tuổi", 10));
        dgvStudents.Columns.Add(TextColumn(nameof(Student.Gender), "Giới tính", 12));
        dgvStudents.Columns.Add(TextColumn(nameof(Student.AvatarPath), "Ảnh đại diện", 28));

        cboGender.Items.AddRange(new object[] { "Nam", "Nữ" });
        inputTable.Dock = DockStyle.Top;
        inputTable.AutoSize = true;
        inputTable.ColumnCount = 2;
        inputTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        inputTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        AddInputRow("Mã SV:", txtId);
        AddInputRow("Họ tên:", txtName);
        AddInputRow("Tuổi:", txtAge);
        AddInputRow("Giới tính:", cboGender);
        AddInputRow("Avatar:", picAvatar, stretch: false);
        AddInputRow("", btnChooseImage, stretch: false);

        var inputGroup = new GroupBox { Text = "Thông tin sinh viên", Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(8) };
        inputGroup.Controls.Add(inputTable);

        var buttonFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
        buttonFlow.Controls.AddRange(new Control[] { btnAdd, btnDelete, btnUpdate, btnExport, btnLoad });
        var buttonGroup = new GroupBox { Text = "Chức năng", Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(8) };
        buttonGroup.Controls.Add(buttonFlow);

        var rightPanel = new Panel { Dock = DockStyle.Right, Width = 300, Padding = new Padding(8) };
        rightPanel.Controls.Add(buttonGroup);
        rightPanel.Controls.Add(inputGroup);
        inputGroup.SendToBack();

        var progressPanel = new Panel { Dock = DockStyle.Bottom, Height = 30, Padding = new Padding(8, 4, 8, 4) };
        progressPanel.Controls.Add(progressBar);
        progressPanel.Controls.Add(lblPercent);

        Controls.Add(dgvStudents);
        Controls.Add(rightPanel);
        Controls.Add(progressPanel);
        dgvStudents.BringToFront();
    }

    private static DataGridViewTextBoxColumn TextColumn(string property, string header, int weight) => new()
    {
        DataPropertyName = property,
        HeaderText = header,
        FillWeight = weight,
    };

    private void AddInputRow(string caption, Control control, bool stretch = true)
    {
        int row = inputTable.RowCount++;
        inputTable.Controls.Add(new Label { Text = caption, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(3, 6, 3, 6) }, 0, row);
        control.Anchor = stretch ? AnchorStyles.Left | AnchorStyles.Right : AnchorStyles.Left;
        inputTable.Controls.Add(control, 1, row);
    }

    private void BuildGridContextMenu()
    {
        var mnuDetail = gridMenu.Items.Add("Xem chi tiết", null, (_, _) => ShowDetail());
        var mnuDelete = gridMenu.Items.Add("Xóa dòng", null, (_, _) => DeleteStudent());
        gridMenu.Items.Add(new ToolStripSeparator());
        gridMenu.Items.Add("Làm mới dữ liệu", null, async (_, _) => await ReloadDataAsync());
        gridMenu.Opening += (_, _) => mnuDetail.Enabled = mnuDelete.Enabled = Current != null;

        dgvStudents.ContextMenuStrip = gridMenu;
        dgvStudents.CellMouseDown += (_, e) =>
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0) bindingSource.Position = e.RowIndex;
        };
        dgvStudents.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) ShowDetail(); };
    }

    private Student? Current => bindingSource.Current as Student;

    private void SetupBinding()
    {
        bindingSource.DataSource = students;
        dgvStudents.DataSource = bindingSource;

        txtId.DataBindings.Add("Text", bindingSource, nameof(Student.StudentId), true, DataSourceUpdateMode.OnPropertyChanged);
        txtName.DataBindings.Add("Text", bindingSource, nameof(Student.FullName), true, DataSourceUpdateMode.OnPropertyChanged);

        Binding ageBinding = txtAge.DataBindings.Add("Text", bindingSource, nameof(Student.Age), true, DataSourceUpdateMode.OnValidation);
        ageBinding.Parse += (_, e) =>
        {
            if (!int.TryParse(e.Value?.ToString(), out int age) || age is < 1 or > 120)
                e.Value = Current?.Age ?? 18;
        };

        Binding genderBinding = cboGender.DataBindings.Add("Text", bindingSource, nameof(Student.Gender), true, DataSourceUpdateMode.OnPropertyChanged);
        cboGender.SelectionChangeCommitted += (_, _) => genderBinding.WriteValue();

        bindingSource.CurrentItemChanged += (_, _) => { ShowAvatar(); UpdateInputState(); };
        bindingSource.ListChanged += (_, _) => UpdateInputState();
        UpdateInputState();
    }

    private void UpdateInputState()
    {
        bool hasCurrent = Current != null;
        inputTable.Enabled = btnDelete.Enabled = btnUpdate.Enabled = hasCurrent;
        btnExport.Enabled = students.Count > 0;
    }

    private void ShowAvatar()
    {
        string path = Current?.AvatarPath ?? "";
        if (path == _shownAvatarPath) return;
        _shownAvatarPath = path;

        Image? old = picAvatar.Image;
        picAvatar.Image = LoadImage(path);
        old?.Dispose();
    }

    private static Image? LoadImage(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return null;
        try
        {
            using var stream = File.OpenRead(path);
            using var source = Image.FromStream(stream);
            return new Bitmap(source);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private void AddStudent()
    {
        int n = students.Count + 1;
        while (students.Any(s => s.StudentId == $"SV{n:000}")) n++;

        var student = new Student { StudentId = $"SV{n:000}", FullName = "Sinh viên mới" };
        students.Add(student);
        bindingSource.Position = students.Count - 1;
        txtName.Focus();
        txtName.SelectAll();
        NotifyStatus($"Thêm thành công sinh viên {student.StudentId}. Tổng: {students.Count}.");
    }

    private void DeleteStudent()
    {
        if (Current is not { } student) return;
        if (MessageBox.Show(this, $"Xóa sinh viên {student.StudentId} - {student.FullName}?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        bindingSource.RemoveCurrent();
        NotifyStatus($"Xóa thành công sinh viên {student.StudentId}. Còn lại: {students.Count}.");
    }

    private void UpdateStudent()
    {
        if (Current is not { } student) return;
        ValidateChildren();
        bindingSource.EndEdit();

        student.StudentId = student.StudentId.Trim();
        student.FullName = student.FullName.Trim();
        if (student.StudentId.Length == 0 || student.FullName.Length == 0)
        {
            Warn("Mã SV và Họ tên không được để trống.");
            return;
        }
        if (!student.FullName.All(IsNameChar))
        {
            Warn("Họ tên chỉ được chứa chữ cái và khoảng trắng.");
            return;
        }
        if (students.Any(s => s != student && s.StudentId.Equals(student.StudentId, StringComparison.OrdinalIgnoreCase)))
        {
            Warn($"Mã SV \"{student.StudentId}\" đã tồn tại.");
            return;
        }

        bindingSource.ResetCurrentItem();
        NotifyStatus($"Cập nhật thành công sinh viên {student.StudentId}.");
    }

    private static bool IsNameChar(char c) =>
        char.IsLetter(c) || c == ' ' || char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.NonSpacingMark;

    private void Warn(string message) =>
        MessageBox.Show(this, message, "Dữ liệu chưa hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    private void ShowDetail()
    {
        if (Current is not { } s) return;

        using var dialog = new Form
        {
            Text = $"Chi tiết sinh viên {s.StudentId}",
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            MaximizeBox = false,
            MinimizeBox = false,
            ShowInTaskbar = false,
            ClientSize = new Size(420, 200),
            Padding = new Padding(12),
        };
        using Image? avatar = LoadImage(s.AvatarPath);
        dialog.Controls.Add(new Label
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12, 0, 0, 0),
            Text = $"Mã SV: {s.StudentId}\n\nHọ tên: {s.FullName}\n\nTuổi: {s.Age}\n\nGiới tính: {s.Gender}\n\n" +
                   $"Ảnh: {(s.AvatarPath.Length == 0 ? "(chưa có)" : Path.GetFileName(s.AvatarPath))}",
        });
        dialog.Controls.Add(new PictureBox
        {
            Dock = DockStyle.Left,
            Width = 150,
            SizeMode = PictureBoxSizeMode.Zoom,
            BorderStyle = BorderStyle.FixedSingle,
            Image = avatar,
        });
        dialog.ShowDialog(this);
    }

    private void ChooseAvatar()
    {
        if (Current is not { } student) return;

        using var dialog = new OpenFileDialog
        {
            Title = "Chọn ảnh đại diện",
            Filter = "File hình ảnh (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp",
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        student.AvatarPath = dialog.FileName;
        NotifyStatus($"Đã chọn ảnh cho sinh viên {student.StudentId}.");
    }

    private void ExportCsv()
    {
        using var dialog = new SaveFileDialog
        {
            Title = "Xuất danh sách sinh viên",
            Filter = "File CSV (*.csv)|*.csv",
            DefaultExt = "csv",
            FileName = $"DanhSachSinhVien_{DateTime.Now:yyyyMMdd}.csv",
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            using var writer = new StreamWriter(dialog.FileName, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
            writer.WriteLine("StudentId,FullName,Age,Gender,AvatarPath");
            foreach (Student s in students)
            {
                writer.WriteLine(string.Join(",",
                    Csv(s.StudentId), Csv(s.FullName), s.Age.ToString(), Csv(s.Gender), Csv(s.AvatarPath)));
            }
            NotifyStatus($"Đã xuất {students.Count} sinh viên ra {Path.GetFileName(dialog.FileName)}.");
            MessageBox.Show(this, "Xuất file CSV thành công.", "Xuất CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show(this, ex.Message, "Không ghi được file", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static string Csv(string value) =>
        value.IndexOfAny(new[] { ',', '"', '\r', '\n' }) < 0 ? value : $"\"{value.Replace("\"", "\"\"")}\"";

    public override Task ReloadDataAsync() => LoadWithProgressAsync(
        DataService.LoadStudentsAsync,
        data =>
        {
            students = new BindingList<Student>(data);
            bindingSource.DataSource = students;
        },
        btnLoad, progressBar, lblPercent, "sinh viên");
}
