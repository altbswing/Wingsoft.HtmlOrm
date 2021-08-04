using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wingsoft.HtmlOrm;

namespace Wingsoft.HtmlOrm.VanillaRock {

    public class SearchPage {

        [NodeInner(XPath = "//*[@id='content']/div[1]/h1", ThrowNotFind = false)]
        public string Title { get; set; }

        [NodeValue(XPath = "div[2]/a", Attribute = "href")]
        [NodeList(XPath = "//*[@id='content']/div[@class='post']", ThrowNotFind = false)]
        /// <summary></summary>
        public List<string> GroupUrlList { get; set; }
    }

    public class GroupPage {

        [NodeValue(XPath = "a", Attribute = "href")]
        [NodeList(XPath = "//*[@class='main-img']", Skip = 1)]
        /// <summary></summary>
        public List<string> UrlList { get; set; }

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
            using (var dmhy = new EntityHttpClient("https://vanilla-rock.com/")) {
                var keyword = "レズ";
                var paged = 1;
                var imageUrlList = new List<string>();
                while (true) {
                    var pageUrl = $"https://vanilla-rock.com/?s={keyword}&paged={paged++}";
                    var page = await dmhy.HttpGetAsync<SearchPage>(pageUrl);
                    if (page.GroupUrlList == null || !page.GroupUrlList.Any()) {
                        break;
                    }
                    Console.WriteLine($"标题 = {page.Title}");
                    Console.WriteLine($"要素 = {page.GroupUrlList.Count}");
                    foreach (var url in page.GroupUrlList.AsParallel()) {
                        Console.WriteLine($"{url}");
                        var groupPage = await dmhy.HttpGetAsync<GroupPage>(url);
                        foreach (var imgUrl in groupPage.UrlList) {
                            Console.WriteLine($"\t{imgUrl}");
                            imageUrlList.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(imgUrl)));
                        }
                    }
                }
                await File.AppendAllLinesAsync(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{keyword}.txt"),
                    imageUrlList, Encoding.UTF8);
            }
        }
    }
}
