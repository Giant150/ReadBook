using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReadBook.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Newtonsoft;
using Newtonsoft.Json;
using System.Text;

namespace ReadBook.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index(bool? next)
        {
            var hostEnv = this.HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
            var configPath = Path.Combine(hostEnv.ContentRootPath, "Setting\\Index.json");
            var configJson = await System.IO.File.ReadAllTextAsync(configPath, System.Text.Encoding.UTF8);
            var config = JsonConvert.DeserializeObject<Config>(configJson);

            var loadUrl = config.Current;
            if (next.HasValue)
            {
                if (next.Value)
                    loadUrl = config.Next;
                else
                    loadUrl = config.Previous;
            }

            if (!loadUrl.EndsWith(".html"))
                return Redirect($"{config.Host}{loadUrl}");

            HtmlWeb web = new HtmlWeb();
            var htmlDoc = await web.LoadFromWebAsync($"{config.Host}{loadUrl}");
            var title = htmlDoc.DocumentNode.SelectSingleNode("/html/head/title");

            ViewData["Title"] = title.InnerText.Trim();

            var contents = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div/div[3]/div/p");
            var contentString = contents.InnerHtml.ToString();
            var resultModel = contentString.Split(new string[2] { "<br />", "<br>" }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Replace("&nbsp;", "")).ToList();

            var nextNodes = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div/div[3]/ul/li[1]/p/a");
            var nextUrl = nextNodes.Attributes["href"].Value;
            var PrevNodes = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div/div[3]/ul/li[4]/p/a");
            var PrevUrl = PrevNodes.Attributes["href"].Value;

            var newConfig = new Config()
            {
                Host = config.Host,
                Current = loadUrl,
                Next = nextUrl,
                Previous = PrevUrl
            };

            configJson = JsonConvert.SerializeObject(newConfig);
            await System.IO.File.WriteAllTextAsync(configPath, configJson, System.Text.Encoding.UTF8);

            return View(resultModel);
        }

        public async Task<IActionResult> Index2(bool? next)
        {
            var hostEnv = this.HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
            var configPath = Path.Combine(hostEnv.ContentRootPath, "Setting\\Index2.json");
            var configJson = await System.IO.File.ReadAllTextAsync(configPath, System.Text.Encoding.UTF8);
            var config = JsonConvert.DeserializeObject<Config>(configJson);

            var loadUrl = config.Current;
            if (next.HasValue)
            {
                if (next.Value)
                    loadUrl = config.Next;
                else
                    loadUrl = config.Previous;
            }

            //if (!loadUrl.EndsWith(".html"))
            //    return Redirect($"{config.Host}{loadUrl}");

            HtmlWeb web = new HtmlWeb();
            var htmlDoc = await web.LoadFromWebAsync($"{config.Host}{loadUrl}");
            var title = htmlDoc.DocumentNode.SelectSingleNode("/html/head/title");

            ViewData["Title"] = title.InnerText.Trim();

            var contents = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div/div[3]/div/p");
            var contentString = contents.InnerHtml.ToString();
            var resultModel = contentString.Split(new string[2] { "<br />", "<br>" }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Replace("&nbsp;", "")).ToList();

            var nextNodes = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div/div[3]/ul/li[1]/p/a");
            var nextUrl = nextNodes.Attributes["href"].Value;
            var PrevNodes = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div/div[3]/ul/li[4]/p/a");
            var PrevUrl = PrevNodes.Attributes["href"].Value;

            var newConfig = new Config()
            {
                Host = config.Host,
                Current = loadUrl,
                Next = nextUrl,
                Previous = PrevUrl
            };

            configJson = JsonConvert.SerializeObject(newConfig);
            await System.IO.File.WriteAllTextAsync(configPath, configJson, System.Text.Encoding.UTF8);

            return View(resultModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
