using Data.Home;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementacoes
{
    public class HomeService : IHomeService
    {

        private readonly HomeDAL _homedal; 
        public HomeService(HomeDAL homedal) {
            _homedal = homedal;
        }

        public async Task Iniciar()
        {
            IniciarCrawler();
        }
        private async Task IniciarCrawler()
        {

            var startTime = DateTime.Now;

            var options = new ChromeOptions();

            using (var driver = new ChromeDriver(options))
            {
                var proxies = new List<Dictionary<string, string>>();
                int totalRows = 0;
                int page = 1;

                while (true)
                {
                    driver.Navigate().GoToUrl($"https://pt.proxyservers.pro/proxy/list/order/updated/order_dir/desc/page/{page}");
                    Thread.Sleep(3000); 

                    var rows = driver.FindElements(By.CssSelector("table tbody tr"));
                    if (rows == null || rows.Count == 0)
                        break;

                    foreach (var row in rows)
                    {
                        var cells = row.FindElements(By.TagName("td"));
                        if (cells.Count < 7) continue;

                        var proxy = new Dictionary<string, string>
                        {
                            ["IP"] = cells[1].Text.Trim(),
                            ["Porta"] = cells[2].Text.Trim(),
                            ["País"] = cells[3].Text.Trim(),
                            ["Protocolo"] = cells[6].Text.Trim(),
                        };

                        proxies.Add(proxy);
                    }

                    Console.WriteLine($"Página {page} processada com {rows.Count} proxies.");
                    totalRows += rows.Count;
                    page++;
                }

                string json = JsonConvert.SerializeObject(proxies, Formatting.Indented);
                var jsonDir = Path.Combine(AppContext.BaseDirectory, "json");
                Directory.CreateDirectory(jsonDir);
                var filePath = Path.Combine(jsonDir, $"proxies_{DateTime.Now:yyyyMMddHHmmss}.json");
                File.WriteAllText(filePath, json);

                var log = new CrawlerLogs
                {
                    StartTime = startTime,
                    EndTime = DateTime.Now,
                    TotalPages = page - 1,
                    TotalRows = totalRows,
                    JsonPath = filePath
                };

                await _homedal.GravarLog(log);

                Console.WriteLine($"Arquivo JSON salvo em: {filePath}");
            }
        }

    }

}
