namespace QuanLySinhVien;

public static class DataService
{
    public static List<Student> GetStudents() => new()
    {
        new Student { StudentId = "SV001", FullName = "Nguyễn Văn An", Age = 20, Gender = "Nam" },
        new Student { StudentId = "SV002", FullName = "Trần Thị Bình", Age = 19, Gender = "Nữ" },
        new Student { StudentId = "SV003", FullName = "Lê Minh Châu", Age = 21, Gender = "Nữ" },
        new Student { StudentId = "SV004", FullName = "Phạm Quốc Dũng", Age = 22, Gender = "Nam" },
        new Student { StudentId = "SV005", FullName = "Hoàng Thị Hà", Age = 20, Gender = "Nữ" },
        new Student { StudentId = "SV006", FullName = "Huỳnh Gia Khoa", Age = 19, Gender = "Nam" },
        new Student { StudentId = "SV007", FullName = "Phan Ngọc Linh", Age = 21, Gender = "Nữ" },
        new Student { StudentId = "SV008", FullName = "Vũ Thị Mai", Age = 20, Gender = "Nữ" },
        new Student { StudentId = "SV009", FullName = "Võ Thanh Nam", Age = 23, Gender = "Nam" },
        new Student { StudentId = "SV010", FullName = "Đặng Hữu Phúc", Age = 22, Gender = "Nam" },
    };

    public static List<Product> GetProducts() => new()
    {
        new Product { ProductId = "SP001", Name = "Bút bi", Price = 5000, Quantity = 200 },
        new Product { ProductId = "SP002", Name = "Vở kẻ ngang", Price = 12000, Quantity = 150 },
        new Product { ProductId = "SP003", Name = "Balo", Price = 250000, Quantity = 30 },
        new Product { ProductId = "SP004", Name = "Máy tính Casio", Price = 480000, Quantity = 25 },
        new Product { ProductId = "SP005", Name = "Thước kẻ", Price = 8000, Quantity = 120 },
        new Product { ProductId = "SP006", Name = "Chuột không dây", Price = 190000, Quantity = 40 },
        new Product { ProductId = "SP007", Name = "Bàn phím", Price = 350000, Quantity = 20 },
        new Product { ProductId = "SP008", Name = "USB 32GB", Price = 120000, Quantity = 60 },
    };
}
