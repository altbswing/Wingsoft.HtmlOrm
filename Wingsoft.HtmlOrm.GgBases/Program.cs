using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Wingsoft.HtmlOrm;

namespace Wingsoft.HtmlOrm.Sample.GgBases {
    public class SearchPage {

        [NodeValue(XPath = "td[2]/a", Attribute = "href", ThrowNotFind = false)]
        [NodeList(XPath = "//tr[@class='dtr']", ThrowNotFind = false)]
        /// <summary></summary>
        public List<string> UrlList { get; set; }

    }

    public class InfoPage {
        
        [NodeInner(XPath = "//*[@id='atitle']")]
        // https://www.ggbases.com/magnet.so?id=105675
        public string Title { get; set; }

    }

    public class Program {

        public static void Main() {
            try {
                var task = RunAsync();
                task.Wait();
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }

        private static async Task RunAsync() {
            using (var ggbases = new EntityHttpClient("https://www.ggbases.com")) {
                var pageNo = 1222;
                var imageUrlList = new List<string>();
                while (true) {
                    var pageUrl = $"https://www.ggbases.com/search.so?p={pageNo}";
                    var page = await ggbases.HttpGetAsync<SearchPage>(pageUrl);
                    var query = from u in page.UrlList
                                let match = Regex.Match(u ?? "", @"\d+$")
                                where match.Success
                                select $"https://www.ggbases.com/magnet.so?id={match.Value}";
                    var uList = query.ToList();
                    foreach (var url in uList) {
                        Console.WriteLine($"{url}");
                        //var page = await dmhy.HttpGetAsync<SearchPage>(url);
                        //var groupPage = await dmhy.HttpGetAsync<GroupPage>(url);
                        //foreach (var imgUrl in groupPage.UrlList) {
                        //    Console.WriteLine($"\t{imgUrl}");
                        //    imageUrlList.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(imgUrl)));
                        //}
                    }
                    break;
                }
                //await File.AppendAllLinesAsync(
                //    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{keyword}.txt"),
                //    imageUrlList, Encoding.UTF8);
            }
        }
    }
}