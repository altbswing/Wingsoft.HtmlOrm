﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wingsoft.HtmlOrm {

    /// <summary>
    /// ノード共通のXPathプロパティを定義する属性
    /// 
    /// by tsubasa</summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public abstract class HtmlNodeAttribute : Attribute {

        /// <summary>該当要素のXPath</summary>
        public string XPath { get; set; } = ".";

        /// <summary>スローするか</summary>
        public bool ThrowNotFind { get; set; } = true;
    }
}
