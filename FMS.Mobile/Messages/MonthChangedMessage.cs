using CommunityToolkit.Mvvm.Messaging.Messages;
using System;

namespace FMS.Mobile.Messages
{
    /// <summary>
    /// 发布：当前选中月份已变化（Year + Month）
    /// 用于在VM之间传递月份变更消息
    /// </summary>
    public class MonthChangedMessage : ValueChangedMessage<DateTime>
    {
        public MonthChangedMessage(DateTime monthFirstDay) : base(monthFirstDay) { }
    }
}
