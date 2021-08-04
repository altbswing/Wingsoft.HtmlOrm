using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wingsoft.Mfa.Gov.Passport {

    /// <summary>
    /// 配置文件
    /// </summary>
    public class LoginConfig {

        /// <summary>档案号</summary>
        [JsonProperty("recordNumber")]
        public string RecordNumber { get; set; }

        /// <summary>验证问题</summary>
        [JsonProperty("questionID")]
        public string QuestionId { get; set; }

        /// <summary>答案</summary>
        [JsonProperty("answer")]
        public string Answer { get; set; }
    }
}
