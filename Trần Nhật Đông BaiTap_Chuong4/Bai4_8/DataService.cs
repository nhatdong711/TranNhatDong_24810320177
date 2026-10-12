namespace QuanLySinhVien;

public static class DataService
{
    private static readonly (string Id, string Name, int Age, string Gender)[] StudentData =
    {
        ("SV001", "Nguyễn Văn An", 20, "Nam"),
        ("SV002", "Trần Thị Bình", 19, "Nữ"),
        ("SV003", "Lê Minh Châu", 21, "Nữ"),
        ("SV004", "Phạm Quốc Dũng", 22, "Nam"),
        ("SV005", "Hoàng Thị Hà", 20, "Nữ"),
        ("SV006", "Huỳnh Gia Khoa", 19, "Nam"),
        ("SV007", "Phan Ngọc Linh", 21, "Nữ"),
        ("SV008", "Vũ Thị Mai", 20, "Nữ"),
        ("SV009", "Võ Thanh Nam", 23, "Nam"),
        ("SV010", "Đặng Hữu Phúc", 22, "Nam"),
    };

    private static readonly (string Id, string Name, decimal Price, int Quantity)[] ProductData =
    {
        ("SP001", "Bút bi", 5000, 200),
        ("SP002", "Vở kẻ ngang", 12000, 150),
        ("SP003", "Balo", 250000, 30),
        ("SP004", "Máy tính Casio", 480000, 25),
        ("SP005", "Thước kẻ", 8000, 120),
        ("SP006", "Chuột không dây", 190000, 40),
        ("SP007", "Bàn phím", 350000, 20),
        ("SP008", "USB 32GB", 120000, 60),
    };

    public static async Task<List<Student>> LoadStudentsAsync(IProgress<int>? progress = null, CancellationToken ct = default)
    {
        return await Task.Run(() =>
        {
            var list = new List<Student>(StudentData.Length);
            for (int i = 0; i < StudentData.Length; i++)
            {
                ct.ThrowIfCancellationRequested();
                Thread.Sleep(250);

                var (id, name, age, gender) = StudentData[i];
                list.Add(new Student { StudentId = id, FullName = name, Age = age, Gender = gender });
                progress?.Report((i + 1) * 100 / StudentData.Length);
            }
            return list;
        }, ct);
    }

    public static async Task<List<Product>> LoadProductsAsync(IProgress<int>? progress = null, CancellationToken ct = default)
    {
        return await Task.Run(async () =>
        {
            var list = new List<Product>(ProductData.Length);
            for (int i = 0; i < ProductData.Length; i++)
            {
                await Task.Delay(250, ct);

                var (id, name, price, quantity) = ProductData[i];
                list.Add(new Product { ProductId = id, Name = name, Price = price, Quantity = quantity });
                progress?.Report((i + 1) * 100 / ProductData.Length);
            }
            return list;
        }, ct);
    }
}
