namespace QuanLySinhVien;

public static class DataService
{
    public static List<Student> GetStudents() => new()
    {
        new Student { StudentId = "SV001", FullName = "Trần Nhật Đông", Age = 20, Gender = "Nam" },
        new Student { StudentId = "SV002", FullName = "Trần Thị Kim Liên", Age = 19, Gender = "Nữ" },
        new Student { StudentId = "SV003", FullName = "Nguyễn Thị Hồng Linh", Age = 21, Gender = "Nữ" },
        new Student { StudentId = "SV004", FullName = "Trần Thế Hào", Age = 22, Gender = "Nam" },
        new Student { StudentId = "SV005", FullName = "Ngô Lâm Khuê", Age = 20, Gender = "Nữ" },
    };
}
