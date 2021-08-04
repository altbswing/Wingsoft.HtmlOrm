using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using log4net;
using log4net.Config;
using System.Xml;
using Microsoft.Toolkit.Uwp.Notifications;

[assembly: XmlConfigurator(ConfigFile = "Config/LogConfig.xml", Watch = true)]

namespace Wingsoft.Mfa.Gov.Passport {

    public class Program {

        private static ILog _Log = LogManager.GetLogger(typeof(Program));

        public static void Main() {
            try {
                var task = MainAsync();
                task.Wait();
            } catch (Exception ex) {
                _Log.Info(ex);
            }
        }

        /// <summary>
        /// 异步处理
        /// </summary>
        /// <returns></returns>
        private static async Task MainAsync() {
            // 設定を読み込む
            var config = await _LoadConfigAsync("altbswing");
            using (var mfaGov = new MfaClient(config)) {
                await mfaGov.Start();
            }
        }

        /// <summary>
        /// 读取设置
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private static async Task<LoginConfig> _LoadConfigAsync(string user) {
            var json = await File.ReadAllTextAsync(Path.Combine("Config", $"{user}.json"));
            var config = JsonConvert.DeserializeObject<LoginConfig>(json);
            return config;
        }
    }
}
