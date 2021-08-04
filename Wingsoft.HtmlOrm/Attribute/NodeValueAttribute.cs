using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wingsoft.HtmlOrm {

    /// <summary>
    /// 要素の属性値を取得する
    /// 
    /// by tsubasa
    /// </summary>
    public class NodeValueAttribute : SingleNodeAttribute {

        /// <summary>html要素の属性名</summary>
        public string Attribute { get; set; }
    }
}
