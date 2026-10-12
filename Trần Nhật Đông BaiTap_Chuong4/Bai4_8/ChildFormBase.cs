namespace QuanLySinhVien;

public class ChildFormBase : Form
{
    public event Action<string>? StatusMessage;

    public event Action<int>? ProgressReported;

    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public string CurrentUser { get; set; } = "";

    private readonly CancellationTokenSource _cts = new();

    protected void NotifyStatus(string message) => StatusMessage?.Invoke(message);

    public virtual Task ReloadDataAsync() => Task.CompletedTask;

    protected async Task LoadWithProgressAsync<T>(
        Func<IProgress<int>, CancellationToken, Task<List<T>>> loader,
        Action<List<T>> apply,
        Button button, ProgressBar bar, Label percentLabel, string what)
    {
        if (!button.Enabled) return;
        button.Enabled = false;

        var progress = new Progress<int>(p =>
        {
            if (IsDisposed) return;
            bar.Value = p;
            percentLabel.Text = $"{p}%";
            ProgressReported?.Invoke(p);
        });

        try
        {
            NotifyStatus($"Đang nạp danh sách {what}...");
            List<T> data = await loader(progress, _cts.Token);
            apply(data);
            NotifyStatus($"Đã nạp xong {data.Count} {what}.");
            MessageBox.Show(this, $"Nạp dữ liệu hoàn tất: {data.Count} {what}.", "Hoàn tất",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            NotifyStatus($"Lỗi nạp dữ liệu: {ex.Message}");
            MessageBox.Show(this, ex.Message, "Lỗi nạp dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!IsDisposed) button.Enabled = true;
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _cts.Cancel();
        base.OnFormClosed(e);
    }
}
