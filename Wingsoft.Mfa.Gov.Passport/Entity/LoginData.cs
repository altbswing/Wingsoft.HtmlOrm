using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wingsoft.Mfa.Gov.Passport {

    /// <summary>
    /// 登录数据
    /// </summary>
    public class LoginData {

        /// <summary>cancelReservation</summary>
        [JsonProperty("cancelReservation")]
        public int CancelReservation { get; set; }

        /// <summary>isallow</summary>
        [JsonProperty("isallow")]
        public int Isallow { get; set; }

        /// <summary>modificationTimes</summary>
        [JsonProperty("modificationTimes")]
        public int ModificationTimes { get; set; }

        /// <summary>istemporary</summary>
        [JsonProperty("istemporary")]
        public int Istemporary { get; set; }

        /// <summary>orgID</summary>
        [JsonProperty("orgID")]
        public string OrgID { get; set; }

        /// <summary>scheduleResType</summary>
        [JsonProperty("scheduleResType")]
        public string ScheduleResType { get; set; }
    }
}
