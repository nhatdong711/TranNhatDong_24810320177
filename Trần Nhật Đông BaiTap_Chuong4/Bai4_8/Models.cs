using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QuanLySinhVien;

public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

public class Student : ObservableObject
{
    private string _studentId = "";
    private string _fullName = "";
    private int _age = 18;
    private string _gender = "Nam";
    private string _avatarPath = "";

    public string StudentId { get => _studentId; set => Set(ref _studentId, value); }
    public string FullName { get => _fullName; set => Set(ref _fullName, value); }
    public int Age { get => _age; set => Set(ref _age, value); }
    public string Gender { get => _gender; set => Set(ref _gender, value); }
    public string AvatarPath { get => _avatarPath; set => Set(ref _avatarPath, value); }
}

public class Product : ObservableObject
{
    private string _productId = "";
    private string _name = "";
    private decimal _price;
    private int _quantity;

    public string ProductId { get => _productId; set => Set(ref _productId, value); }
    public string Name { get => _name; set => Set(ref _name, value); }
    public decimal Price { get => _price; set => Set(ref _price, value); }
    public int Quantity { get => _quantity; set => Set(ref _quantity, value); }
}
