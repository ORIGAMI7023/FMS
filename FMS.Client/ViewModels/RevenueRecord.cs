using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FMS.Client.ViewModels
{
    public class RevenueRecord : INotifyPropertyChanged
    {
        public string Doctor { get; set; }
        public string Category { get; set; }

        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    OnPropertyChanged();
                }
            }
        }

        public string DisplayDoctorName { get; set; } // 用于首行医生名展示

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class MainViewModel
    {
        public ObservableCollection<RevenueRecord> Records { get; set; }

        public MainViewModel()
        {
            // 示例数据
            var raw = new[]
            {
                new RevenueRecord { Doctor = "陈霞", Category = "现金" },
                new RevenueRecord { Doctor = "陈霞", Category = "医保" },
                new RevenueRecord { Doctor = "陈霞", Category = "退费" },
                new RevenueRecord { Doctor = "王吉龙", Category = "贴款35" },
                new RevenueRecord { Doctor = "王吉龙", Category = "治疗" },
                new RevenueRecord { Doctor = "王吉龙", Category = "退费" }
            };

            Records = new ObservableCollection<RevenueRecord>();
            string lastDoctor = null;
            foreach (var record in raw)
            {
                if (record.Doctor != lastDoctor)
                {
                    record.DisplayDoctorName = record.Doctor;
                    lastDoctor = record.Doctor;
                }
                else
                {
                    record.DisplayDoctorName = string.Empty;
                }
                Records.Add(record);
            }
        }
    }
}
