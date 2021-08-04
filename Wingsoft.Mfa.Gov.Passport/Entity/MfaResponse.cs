using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wingsoft.Mfa.Gov {

    /// <summary>
    /// 返回消息
    /// </summary>
    public class MfaResponse<TData> {

        /// <summary>状态码</summary>
        [JsonProperty("status")]
        public int Status { get; set; }

        /// <summary>数据</summary>
        [JsonProperty("data")]
        public TData Data { get; set; }
    }
}
