using FMS.Client.Models;
using FMS.Client.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;

namespace FMS.Client.ViewModels
{
    public class RevenueItem : INotifyPropertyChanged
    {
        public string Doctor { get; set; }

        private string category;
        public string Category
        {
            get => category;
            set { category = value; OnPropertyChanged(); }
        }

        public decimal Amount { get; set; }

        public List<string> AllDoctors => UploadViewModel.GlobalDoctors;

        public List<string> Categories =>
            UploadViewModel.Instance?.DoctorCategoryMap.TryGetValue(Doctor, out var list) == true
            ? list
            : new List<string>();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class UploadViewModel : INotifyPropertyChanged
    {
        public static List<string> GlobalDoctors { get; private set; }
        public static UploadViewModel? Instance { get; private set; }

        public DateTime Date { get; set; } = DateTime.Today;
        public string Remark { get; set; } = string.Empty;
        public ObservableCollection<RevenueItem> Items { get; set; } = new();

        public Dictionary<string, List<string>> DoctorCategoryMap { get; private set; } = new();
        public ICommand UploadCommand { get; }

        private readonly ApiService _api = new();

        public UploadViewModel()
        {
            Instance = this;
            LoadConfig();
            UploadCommand = new RelayCommand(async () =>
            {
                var data = new DailyRevenueUploadRequest
                {
                    Date = this.Date,
                    Remark = this.Remark,
                    Items = Items.Select(i => new Models.RevenueItem
                    {
                        Doctor = i.Doctor,
                        Category = i.Category,
                        Amount = i.Amount
                    }).ToList()
                };

                var result = await _api.UploadDailyDataAsync(data);
                MessageBox.Show(result ? "上传成功" : "上传失败");
            });
        }

        private void LoadConfig()
        {
            var json = File.ReadAllText("Config/UploadConfig.json");
            var config = JsonSerializer.Deserialize<UploadConfig>(json);
            DoctorCategoryMap = config.医生分类;
            GlobalDoctors = DoctorCategoryMap.Keys.ToList();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}