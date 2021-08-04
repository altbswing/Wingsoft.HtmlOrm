using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wingsoft.Mfa.Gov.Passport {

    /// <summary>
    /// 预约数据
    /// </summary>
    public class ReservationData {

        /// <summary>date</summary>
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        /// <summary>orgName</summary>
        [JsonProperty("orgName")]
        public string OrgName { get; set; }

        /// <summary>beSpeakNumber</summary>
        [JsonProperty("beSpeakNumber")]
        public string BeSpeakNumber { get; set; }

        /// <summary>PeriodOfTimeList</summary>
        [JsonProperty("PeriodOfTimeList")]
        public List<Period> PeriodOfTimeList { get; set; }
    }

    /// <summary>
    /// 时间段信息
    /// </summary>
    public class Period {

        /// <summary>日期</summary>
        [JsonIgnore]
        public ReservationData Reservation { get; set; }

        /// <summary>日期</summary>
        [JsonIgnore]
        public DateTime Date => Reservation.Date;

        /// <summary>periodid</summary>
        [JsonProperty("periodid")]
        public string Periodid { get; set; }

        /// <summary>startTime</summary>
        [JsonProperty("startTime")]
        public TimeSpan StartTime { get; set; }

        /// <summary>endTime</summary>
        [JsonProperty("endTime")]
        public TimeSpan EndTime { get; set; }

        /// <summary>peopleNumber</summary>
        [JsonProperty("peopleNumber")]
        public int PeopleNumber { get; set; }

        /// <summary>userNumber</summary>
        [JsonProperty("userNumber")]
        public int UserNumber { get; set; }
    }
}
