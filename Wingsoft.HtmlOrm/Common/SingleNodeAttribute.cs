using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wingsoft.HtmlOrm {

    /// <summary>
    /// 要素のXPathを設定する
    /// 
    /// by tsubasa
    /// </summary>
    public abstract class SingleNodeAttribute : HtmlNodeAttribute {

        /// <summary>优先顺序</summary>
        public int Sequence { get; set; }

        /// <summary>默认值</summary>
        public string Default { get; set; }
    }

}
