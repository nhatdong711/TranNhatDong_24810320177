using System.ComponentModel;

namespace QuanLySinhVien;

public class FormProduct : ChildFormBase
{
    private BindingList<Product> products = new();
    private readonly BindingSource bindingSource = new();

    private readonly DataGridView dgvProducts = new();
    private readonly Button btnAdd = new() { Text = "Thêm", AutoSize = true };
    private readonly Button btnDelete = new() { Text = "Xóa", AutoSize = true };
    private readonly ContextMenuStrip gridMenu = new();

    public FormProduct()
    {
        Text = "Quản lý Sản phẩm";
        ClientSize = new Size(720, 460);
        MinimumSize = new Size(500, 350);

        dgvProducts.Dock = DockStyle.Fill;
        dgvProducts.AutoGenerateColumns = false;
        dgvProducts.AllowUserToAddRows = false;
        dgvProducts.AllowUserToDeleteRows = false;
        dgvProducts.MultiSelect = false;
        dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvProducts.BackgroundColor = SystemColors.Window;
        dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Product.ProductId), HeaderText = "Mã SP", FillWeight = 15 });
        dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Product.Name), HeaderText = "Tên sản phẩm", FillWeight = 45 });
        dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Product.Price), HeaderText = "Đơn giá", FillWeight = 20, DefaultCellStyle = { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight } });
        dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Product.Quantity), HeaderText = "Số lượng", FillWeight = 20, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight } });
        dgvProducts.DataError += (_, e) =>
        {
            NotifyStatus("Giá trị nhập không hợp lệ - Đơn giá và Số lượng phải là số.");
            e.Cancel = true;
        };

        bindingSource.DataSource = products;
        dgvProducts.DataSource = bindingSource;

        var buttonFlow = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(4) };
        buttonFlow.Controls.AddRange(new Control[] { btnAdd, btnDelete });


        Controls.Add(dgvProducts);
        Controls.Add(buttonFlow);
        dgvProducts.BringToFront();

        var mnuDetail = gridMenu.Items.Add("Xem chi tiết", null, (_, _) => ShowDetail());
        var mnuDelete = gridMenu.Items.Add("Xóa dòng", null, (_, _) => DeleteProduct());
        gridMenu.Items.Add(new ToolStripSeparator());
        gridMenu.Items.Add("Làm mới dữ liệu", null, (_, _) => ReloadData());
        gridMenu.Opening += (_, _) => mnuDetail.Enabled = mnuDelete.Enabled = Current != null;
        dgvProducts.ContextMenuStrip = gridMenu;
        dgvProducts.CellMouseDown += (_, e) =>
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0) bindingSource.Position = e.RowIndex;
        };

        btnAdd.Click += (_, _) => AddProduct();
        btnDelete.Click += (_, _) => DeleteProduct();
        Shown += (_, _) => ReloadData();
    }

    private Product? Current => bindingSource.Current as Product;

    private void AddProduct()
    {
        int n = products.Count + 1;
        while (products.Any(p => p.ProductId == $"SP{n:000}")) n++;

        var product = new Product { ProductId = $"SP{n:000}", Name = "Sản phẩm mới" };
        products.Add(product);
        bindingSource.Position = products.Count - 1;
        NotifyStatus($"Thêm thành công sản phẩm {product.ProductId}. Tổng: {products.Count}.");
    }

    private void DeleteProduct()
    {
        if (Current is not { } product) return;
        if (MessageBox.Show(this, $"Xóa sản phẩm {product.ProductId} - {product.Name}?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        bindingSource.RemoveCurrent();
        NotifyStatus($"Xóa thành công sản phẩm {product.ProductId}. Còn lại: {products.Count}.");
    }

    private void ShowDetail()
    {
        if (Current is not { } p) return;
        MessageBox.Show(this,
            $"Mã SP: {p.ProductId}\nTên: {p.Name}\nĐơn giá: {p.Price:N0} đ\nSố lượng: {p.Quantity}\nThành tiền: {p.Price * p.Quantity:N0} đ",
            "Chi tiết sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public override void ReloadData()
    {
        products = new BindingList<Product>(DataService.GetProducts());
        bindingSource.DataSource = products;
        NotifyStatus($"Đã làm mới dữ liệu: {products.Count} sản phẩm.");
    }
}
