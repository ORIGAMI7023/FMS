using FMS.Client.Models;
using FMS.Client.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;

namespace FMS.Client.ViewModels
{
    public class RevenueItem : INotifyPropertyChanged
    {
        public string Doctor { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public bool IsFirstOfDoctor { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class UploadViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<RevenueItem> Items { get; set; } = new();
        public Dictionary<string, List<string>> DoctorCategoryMap { get; private set; } = new();
        public DateTime Date { get; set; } = DateTime.Today;
        public string Remark { get; set; } = "";

        public ICommand UploadCommand { get; }

        private readonly ApiService _api = new();

        public UploadViewModel()
        {
            LoadConfig();
            GenerateAllRows();

            UploadCommand = new RelayCommand(async () =>
            {
                var data = new DailyRevenueUploadRequest
                {
                    Date = this.Date,
                    Remark = this.Remark,
                    Items = Items
                        .Where(i => i.Amount != 0)
                        .Select(i => new Models.RevenueItem
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
        }

        private void GenerateAllRows()
        {
            foreach (var doctor in DoctorCategoryMap)
            {
                bool first = true;
                foreach (var category in doctor.Value)
                {
                    Items.Add(new RevenueItem
                    {
                        Doctor = doctor.Key,
                        Category = category,
                        Amount = 0,
                        IsFirstOfDoctor = first
                    });
                    first = false;
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}