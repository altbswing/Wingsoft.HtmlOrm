using System;
using HtmlAgilityPack;
using Wingsoft.HtmlOrm.Resources;

namespace Wingsoft.HtmlOrm {

    /// <summary>
    /// XPath无法找到节点时的异常
    /// </summary>
    public class NotFindNodeException : Exception {

        /// <summary>出错父节点</summary>
        public HtmlNode ParentNode { get; private set; }

        /// <summary>XPath</summary>
        public string XPath { get; private set; }

        /// <summary>错误信息</summary>
        public override string Message => $"{string.Format(Messages.E001, XPath)}\r\n{ParentNode}";

        /// <summary>使用出错父节点初始化异常</summary>
        public NotFindNodeException(HtmlNode parentNode, string xPath) {
            ParentNode = parentNode;
            XPath = xPath;
        }
    }
}