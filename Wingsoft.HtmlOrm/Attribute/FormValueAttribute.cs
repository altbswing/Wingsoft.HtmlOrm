using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wingsoft.HtmlOrm {

    /// <summary>
    /// フォームの値を示す
    /// 
    /// by tsubasa
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class FormValueAttribute : Attribute {

        /// <summary>フォームのName属性</summary>
        public string Name { get; set; }
    }

}
