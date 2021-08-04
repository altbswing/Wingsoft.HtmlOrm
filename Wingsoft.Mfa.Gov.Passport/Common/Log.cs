using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wingsoft.Mfa.Gov.Passport {

    public static class Log {

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ILog GetLogger() {
            var repository = LogManager.CreateRepository("NETCoreRepository");
            XmlConfigurator.Configure(repository, new FileInfo("log4net.xml"));
            var logger = LogManager.GetLogger(repository.Name, "NETCorelog4net");
            return logger;
        }
    }
}
