using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Wingsoft.HtmlOrm;

namespace Wingsoft.HtmlOrm.Fanza {

    /// <summary>
    /// 一张AV的基本信息
    /// </summary>
    public class AdultVideo {

        /// <summary>标题</summary>
        public string Title { get; set; }

        /// <summary>车牌</summary>
        public string No { get; set; }

        /// <summary>片商</summary>
        public string Maker { get; set; }

        /// <summary>导演</summary>
        public string Supervision { get; set; }

        /// <summary>时长</summary>
        public string Duration { get; set; }

        /// <summary>女优</summary>
        public List<string> Actress { get; set; }

        /// <summary>上线日期</summary>
        public DateTime? Date { get; set; }

        /// <summary>性癖</summary>
        public List<string> XpList { get; set; }
    }
}
