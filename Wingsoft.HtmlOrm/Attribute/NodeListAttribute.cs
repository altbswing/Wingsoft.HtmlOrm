using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wingsoft.HtmlOrm {

    /// <summary>
    /// 要素群のXPathを設定する
    /// 
    /// by tsubasa
    /// </summary>
    public class NodeListAttribute : HtmlNodeAttribute {

        public int Skip { get; set; }
    }
}
