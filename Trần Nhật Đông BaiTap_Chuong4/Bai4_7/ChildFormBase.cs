namespace QuanLySinhVien;

public class ChildFormBase : Form
{
    public event Action<string>? StatusMessage;

    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public string CurrentUser { get; set; } = "";

    protected void NotifyStatus(string message) => StatusMessage?.Invoke(message);

    public virtual void ReloadData()
    {
    }
}
