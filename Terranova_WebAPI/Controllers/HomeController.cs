using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Terranova_APIClient;
using Terranova_WebAPI.Models;
using Microsoft.Extensions.Logging.AzureAppServices;
using System.Reflection;

namespace Terranova_WebAPI.Controllers
{
    public class HomeController : Controller
    {
        private ILogger Logger { get; }
        private IAPIClient _apiClient;
        private ILoggerFactory LoggerFactory { get; }

        public HomeController(ILoggerFactory loggerFactory, IAPIClient apiClient)
        {
            _apiClient = apiClient;
            LoggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger("HomeController");
        }

        public IActionResult Index()
        {
            return View("Index", GetToken());
        }

        [Route("ClearCache")]
        public IActionResult ClearCache()
        {
            this.Logger.LogInformation("ClearCache");

            EnvCache.ClearCache();
            ViewBag.CacheMessage = "Cache Cleared!";
            return View("Index", GetToken());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task Test(string envUid = "bdfae62f-902c-4002-8f56-ab1f233a32c4")
        {
            Logger.LogError("Test");

            var o = new Terranova_APIClient.TerranovaAPIClient(_apiClient.ReportApiInfos, LoggerFactory);

            var infos = Terranova_APIClient.EnvCache.GetReportApiInfo(_apiClient.ReportApiInfos, envUid, Logger);

            var a = await o.GetConfigurationAsync(envUid, "en");
            var b = await o.ReportAsync(new Terranova_APIClient.Models.PhishingEmailInfo
            {
                TwcId = "15504009",
                EnvUID = envUid,
                EmailBody = "bob",
                EmailReportedBy = "stephane.brouillard@terranovacorporation.com",
                IsRealPhishing = false,
                ReportedDate = DateTime.Now.ToString(),
                Comments = "idk"
            });
        }

        private static string appToken { get; set; } = null;

        public static string GetToken() =>
            appToken ?? (appToken = System.IO.File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location).ToString("yMdHmss"));
    }
}
