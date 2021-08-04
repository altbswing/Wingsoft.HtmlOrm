using ClosedXML.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Wingsoft.HtmlOrm;

namespace Wingsoft.HtmlOrm.Fanza {

    public class Program {

        private const int _StartPage = 1;

        private static readonly DateTime _StartDate = DateTime.Parse("2021-07-01");

        private static readonly DateTime _EndDate = DateTime.Parse("2021-07-31");

        private static void Main() {
            try {
                var t = MainAsync();
                t.Wait();
            } catch (Exception ex) {
                Console.WriteLine(ex.InnerException);
            }
            Console.Read();
        }

        /// <summary>
        /// main
        /// </summary>
        /// <returns></returns>
        private async static Task MainAsync() {
            // リスト取得
            var list = await _AvList();
            _ToExcel(list);
        }

        private static void _ToExcel(List<AdultVideo> list) {
            var report = _CreateReport();
            using (var fs = report.Open(FileMode.OpenOrCreate)) {
                var xBook = new XLWorkbook(fs);
                xBook.CalculateMode = XLCalculateMode.Auto;
                var xSheet = xBook.Worksheets.First();
                // 解析XP
                var xpMap = new Dictionary<string, string>();
                var head = (char)('A' - 1);
                for (var c = 'H'; ; c++) {
                    var xCol = $"{(head < 'A' ? "" : $"{head}")}{c}";
                    var xp = $"{xSheet.Cell($"{xCol}1")?.Value}".Trim();
                    if (string.IsNullOrEmpty(xp)) {
                        break;
                    }
                    xpMap[xp] = xCol;
                    if (c == 'Z') {
                        c = 'A';
                        head++;
                    }
                }
                // 填充数据
                var xRowNo = 2;
                foreach (var av in list) {
                    // 车牌
                    xSheet.Cell($"A{xRowNo}").Value = av.No;
                    // 标题
                    xSheet.Cell($"B{xRowNo}").Value = av.Title;
                    // 日期
                    xSheet.Cell($"C{xRowNo}").Value = $"{av.Date:yyyy-MM-dd}";
                    // 片商
                    xSheet.Cell($"D{xRowNo}").Value = av.Maker;
                    // 导演
                    xSheet.Cell($"E{xRowNo}").Value = av.Supervision;
                    // 女优
                    xSheet.Cell($"F{xRowNo}").Value = string.Join(" | ", av.Actress);
                    // 时长
                    xSheet.Cell($"G{xRowNo}").Value = $"'{av.Duration}";
                    // 设置XP列表
                    foreach (var xp in av.XpList) {
                        if (!xpMap.ContainsKey(xp)) {
                            Console.WriteLine($"XP:{xp} 不存在");
                            continue;
                        }
                        var xAddr = $"{xpMap[xp]}{xRowNo}";
                        xSheet.Cell(xAddr).Value = "●";
                    }
                    xRowNo++;
                }
                var array = JsonConvert.DeserializeObject<JArray>(File.ReadAllText("Tags.json"));
                var zhMap = array.ToDictionary(
                    t => (t["ja-JP"] as JValue).Value,
                    t => (t["zh-CN"] as JValue).Value
                );
                foreach (var item in xpMap) {
                    var xAddr = $"{xpMap[item.Key]}1";
                    var ja = xSheet.Cell(xAddr).Value as string;
                    if (!zhMap.ContainsKey(ja)) {
                        Console.WriteLine($"日本語:「{ja}」 に対応する中国語は存在しません。");
                        continue;
                    }
                    xSheet.Cell(xAddr).Value = zhMap[ja];
                }
                // 保存
                xBook.SaveAs(fs);
            }
        }

        /// <summary>
        /// Excel
        /// </summary>
        private static FileInfo _CreateReport() {
            Console.WriteLine($"Excel作成中...");
            var desk = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var path = Path.Combine(desk, "AV", $"车牌大全_{_StartDate:yyyy年MM月}_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            var report = new FileInfo(path);
            if (!report.Directory.Exists) {
                report.Directory.Create();
            }
            var template = new FileInfo("Template.xlsx");
            template.CopyTo(report.FullName);
            return new FileInfo(report.FullName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async static Task<List<AdultVideo>> _AvList() {
            using var dmm = new EntityHttpClient("http://www.dmm.co.jp");
            // 先确认一次年龄
            await dmm.HttpGetAsync<object>("https://www.dmm.co.jp/age_check/=/declared=yes/?rurl=http%3A%2F%2Fwww.dmm.co.jp%2Fdigital%2Fvideoa%2F-%2Flist%2F%3D%2Fsort%3Ddate%2Fpage%3D1%2F");
            Console.WriteLine("年齢確認成功");
            var avList = new List<AdultVideo>();
            for (int pageNo = _StartPage; ; pageNo++) {
                var url = $"https://www.dmm.co.jp/digital/videoa/-/list/=/sort=date/page={pageNo}/";
                var page = await dmm.HttpGetAsync<ListPage>(url);
                Console.WriteLine($"ページ {pageNo} :");
                var urlList = page.AvUrlList.Where(u => u.StartsWith("https")).ToList();
                var taskList = urlList.Select(u => _ParseAvAsync(dmm, u)).ToList();
                // av信息url
                foreach (var avUrl in urlList) {
                    var task = _ParseAvAsync(dmm, avUrl);
                }
                foreach (var task in taskList) {
                    avList.Add(await task);
                }
                if (avList.Any(av => av != null && av.Date < _StartDate)) {
                    break;
                }
            }
            avList = avList.Where(av => av != null && _StartDate <= av.Date && av.Date <= _EndDate).ToList();
            return avList;
        }

        /// <summary>
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async static Task<AdultVideo> _ParseAvAsync(EntityHttpClient dmm, string avUrl) {
            try {
                var avPage = await dmm.HttpGetAsync<AdultVideoPage>(avUrl);
                // map
                var map = avPage.ItemList
                    .Where(i => !string.IsNullOrWhiteSpace(i.Name))
                    .ToDictionary(i => i.Name.Trim('：'), i => i.Html);

                var av = new AdultVideo();
                // 标题
                av.Title = avPage.Title;
                // 片商 "td[2]/a")]
                if (map.ContainsKey("メーカー")) {
                    av.Maker = map["メーカー"];
                }
                // 导演 "td[2]/a")]
                if (map.ContainsKey("監督")) {
                    av.Supervision = map["監督"];
                }
                // 时长 "td[2]")]
                if (map.ContainsKey("収録時間")) {
                    av.Duration = map["収録時間"];
                }
                // 上线日期 "td[2]")]
                if (map.ContainsKey("配信開始日")) {
                    av.Date = DateTime.Parse(map["配信開始日"]);
                }
                // 女优 "//*[@id='performer']/a")]
                if (map.ContainsKey("出演者")) {
                    av.Actress = map["出演者"].Split(" ".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                // 性癖 "td[2]/a")]
                if (map.ContainsKey("ジャンル")) {
                    av.XpList = map["ジャンル"].Split(" ".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                // 车牌 "td[2]")]
                if (map.ContainsKey("品番")) {
                    var hinban = Regex.Match(map["品番"], @"[a-zA-Z]+[0-9]+").Value;
                    var w = Regex.Match(hinban, @"[a-zA-Z]+").Value;
                    var d = Regex.Match(hinban, @"[0-9]+").Value;
                    av.No = $"{w.ToUpper()}-{d.Trim('0'):000}";
                }
                Console.WriteLine($" |- {av.Date:yyyy-MM-dd} {av.No} {av.Title}");
                return av;
            } catch (Exception ex) {
                Console.WriteLine($"\t{avUrl}\n\t{ex.Message}");
                return null;
            }
        }


    }

    /// <summary>列表页</summary>
    public class ListPage {

        [NodeList(XPath = "//*[@id=\"list\"]/li")]
        [NodeValue(XPath = "div/p[2]/a", Attribute = "href")]
        public List<string> AvUrlList { get; set; }
    }

    /// <summary>AV页</summary>
    public class AdultVideoPage {

        /// <summary>标题</summary>
        [NodeInner(XPath = "//*[@id='title']")]
        public string Title { get; set; }

        /// <summary>数据项</summary>
        [NodeList(XPath = "//*[@id='mu']/div/table/tr/td[1]/table/tr")]
        [NodeObject]
        public List<AdultVideoItem> ItemList { get; set; }
    }

    /// <summary>AV数据项</summary>
    public class AdultVideoItem {

        /// <summary>数据名</summary>
        [NodeInner(XPath = "td[1]")]
        public string Name { get; set; }

        /// <summary>数据html</summary>
        [NodeInner(XPath = "td[2]")]
        public string Html { get; set; }

        public override string ToString() => $"{Name} = {Html}";
    }

}