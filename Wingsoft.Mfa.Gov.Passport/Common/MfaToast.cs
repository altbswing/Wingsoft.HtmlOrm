using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;

namespace Wingsoft.Mfa.Gov.Passport {

    /// <summary>
    /// 通知管理
    /// </summary>
    public static class MfaToast {

        static MfaToast() => ToastNotificationManagerCompat.OnActivated += Activated;

        /// <summary>
        /// 点击通知
        /// </summary>
        /// <param name="e"></param>
        private static void Activated(ToastNotificationActivatedEventArgsCompat toastArgs) {
            Process.Start("explorer", "https://ppt.mfa.gov.cn/appo/page/reservation.html");
        }

        /// <summary>停止抢号通知</summary>
        public static void End() {
            new ToastContentBuilder()
                .AddText("已停止抢号！")
                .Show();
        }

        /// <summary>
        /// 抢号通知
        /// </summary>
        /// <param name="okList"></param>
        public static void Hit(List<Period> okList) {
            var dateList = okList.Select(p => p.Date).ToHashSet()
                .OrderBy(d => d)
                .Select(d => $"{d: MM月dd日}")
                .ToList();
            new ToastContentBuilder()
                .AddText("赶紧抢号！")
                .AddText(string.Join(", ", dateList))
                .Show();
        }

    }
}
