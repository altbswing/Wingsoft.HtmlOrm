using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wingsoft.Mfa.Gov.Passport {

    /// <summary>
    /// 大使馆网站客户端
    /// </summary>
    public class MfaClient : IDisposable {

        private static ILog _Log = LogManager.GetLogger(typeof(MfaClient));

        /// <summary>Http通信客户端</summary>
        private readonly HttpClient _httpClient;

        /// <summary>用户信息</summary>
        private readonly LoginConfig _loginConfig;

        /// <summary>登录信息</summary>
        private LoginData _loginData;

        /// <summary>执行</summary>
        public bool IsRun { get; private set; } = false;

        /// <summary>刷站间隔（默认5秒）</summary>
        public int WaitMs { get; set; } = 10000;

        /// <summary>
        /// 初始化客户端
        /// </summary>
        public MfaClient(LoginConfig loginConfig) {
            _httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });
            _httpClient.DefaultRequestHeaders.Add("Referrer-Policy", "strict-origin-when-cross-origin");
            _httpClient.BaseAddress = new Uri("https://ppt.mfa.gov.cn");
            // 用户信息
            _loginConfig = loginConfig;
        }

        /// <summary>
        /// 开始
        /// </summary>
        /// <returns></returns>
        public async Task Start() {
            if (_loginData == null) {
                await LoginAsync();
            }
            IsRun = true;
            while (IsRun) {
                var list = await GetReservationData();
                // DEBUG
                //list[0].PeriodOfTimeList[1].PeopleNumber = 101;
                //list[1].PeriodOfTimeList[1].PeopleNumber = 101;
                //list[2].PeriodOfTimeList[1].PeopleNumber = 101;
                var okList = new List<Period>();
                // 可预约的日期
                foreach (var day in list) {
                    _Log.Info($"{day.OrgName} - {day.Date:yyyy-MM-dd}");
                    // 可预约的时间段
                    foreach (var period in day.PeriodOfTimeList) {
                        var ok = period.PeopleNumber != period.UserNumber;
                        _Log.Info($"\t|- {period.StartTime:hh\\:mm}～{period.EndTime:hh\\:mm} " +
                            $"预约人数： {period.UserNumber}/{period.PeopleNumber}" +
                            $"{(ok ? "(可抢)" : "")}");
                        if (ok) {
                            period.Reservation = day;
                            okList.Add(period);
                        }
                    }
                }
                if (okList.Any()) {
                    MfaToast.Hit(okList);
                } else {
                    _Log.Info($"真遗憾，没抢到...");
                }
                _Log.Info($"下一次抢号： {DateTime.Now.AddMilliseconds(WaitMs): HH:mm:ss}");
                _Log.Info($"");
                await Task.Delay(WaitMs);
            }
            MfaToast.End();
        }

        /// <summary>
        /// 结束刷刷
        /// </summary>
        public void End() => IsRun = false;

        /// <summary>Login</summary>
        private async Task LoginAsync() {
            _Log.Info("正在登录...");
            // 生成form数据
            var query = from p in typeof(LoginConfig).GetProperties()
                        let attr = p.GetCustomAttribute<JsonPropertyAttribute>()
                        where attr != null
                        select (Key: attr.PropertyName, Value: $"{p.GetValue(_loginConfig)}");
            var form = query.ToDictionary(i => i.Key, i => i.Value);
            foreach (var item in form) {
                _Log.Info($"{item.Key}: {item.Value}");
            }
            var url = "https://ppt.mfa.gov.cn/appo/service/reservation/data/getLastReservationInfo.json";
            // 提交通信
            var json = await _PostAsync(url, form);
            // entity
            var mfaResponse = JsonConvert.DeserializeObject<MfaResponse<LoginData>>(json);
            // 调用异常
            if (mfaResponse.Status != 0) {
                throw new Exception($"登录失败： 状态码 = {mfaResponse.Status}");
            }
            _Log.Info("登录成功！");
            _Log.Info("");
            _loginData = mfaResponse.Data;
        }

        /// <summary>
        /// 预约数据
        /// </summary>
        /// <returns></returns>
        private async Task<List<ReservationData>> GetReservationData() {
            // 开始刷数据
            _Log.Info($"正在获取预约数据...");
            var rid = new Random(Guid.NewGuid().GetHashCode()).NextDouble();
            var url = $"https://ppt.mfa.gov.cn/appo/service/reservation/data/getReservationDateBean.json?rid={rid}";
            var formData = new Dictionary<string, string> { ["addressName"] = "" };

            var json = await _PostAsync(url, formData);
            // entity
            var mfaResponse = JsonConvert.DeserializeObject<MfaResponse<List<ReservationData>>>(json);
            // 调用异常
            if (mfaResponse.Status != 0) {
                throw new Exception($"获取信息失败： 状态码 = {mfaResponse.Status}");
            }
            return mfaResponse.Data;
        }

        /// <summary>
        /// 提交Post获取字符串返回值
        /// </summary>
        /// <param name="url"></param>
        /// <param name="formData"></param>
        /// <returns></returns>
        private async Task<string> _PostAsync(string url, Dictionary<string, string> formData) {
            // 通信form
            var form = new FormUrlEncodedContent(formData);
            var response = await _httpClient.PostAsync(url, form);
            // http通信失败
            if (response.StatusCode != HttpStatusCode.OK) {
                throw new Exception($"http通信失败： 状态码 = {response.StatusCode}");
            }
            if (response.Content.Headers.ContentType != null) {
                response.Content.Headers.ContentType.CharSet = Encoding.UTF8.WebName;
            }
            response.EnsureSuccessStatusCode();
            // json
            var json = await response.Content.ReadAsStringAsync();
            return json;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Dispose() {
            _httpClient.Dispose();
            IsRun = false;
        }
    }
}
