using System.ComponentModel;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;

public class UploadViewModel : INotifyPropertyChanged
{
    private RevenueRecord _record = new RevenueRecord();
    public RevenueRecord Record
    {
        get => _record;
        set
        {
            _record = value;
            OnPropertyChanged(nameof(Record));
        }
    }

    public ICommand UploadCommand { get; }

    public UploadViewModel()
    {
        UploadCommand = new RelayCommand(async () => await UploadAsync());
    }
    private readonly ApiService _api = new();

    private async Task UploadAsync()
    {
        bool result = await _api.UploadRevenueAsync(Record);
        MessageBox.Show(result ? "上传成功" : "上传失败");
    }


    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
