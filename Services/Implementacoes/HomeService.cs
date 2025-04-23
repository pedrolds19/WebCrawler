using Data.Home;
using Models;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Services.Interfaces;
using System.Threading;

namespace Services.Implementacoes
{
    public class HomeService : IHomeService
    {
        private readonly HomeDAL _homedal;
        private readonly string _outputDir;
        private readonly int _maxThreads = 3;

        public HomeService(HomeDAL homedal)
        {
            _homedal = homedal;
            _outputDir = Path.Combine(AppContext.BaseDirectory, "output");
        }

        public async Task<CrawlerLogs> Iniciar()
        {
            var startTime = DateTime.Now;

            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--disable-gpu");

            Console.WriteLine($"Iniciando serviço...");

            var proxies = new List<Dictionary<string, string>>();
            int totalRows = 0;
            int page = 1;
            bool hasMorePages = true;
            SemaphoreSlim semaphore = new(_maxThreads);

            using (var driver = new ChromeDriver(options))
            {
                while (hasMorePages)
                {
                    await semaphore.WaitAsync();
                    driver.Navigate().GoToUrl($"https://pt.proxyservers.pro/proxy/list/order/updated/order_dir/desc/page/{page}");

                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    var table = wait.Until(driver => driver.FindElement(By.CssSelector("table tbody")));

                    await Task.Delay(2000); 

                    var rows = driver.FindElements(By.CssSelector("table tbody tr"));
                    if (rows == null || rows.Count == 0)
                    {
                        Console.WriteLine($"Não há mais proxies para processar na página {page}. Finalizando.");
                        hasMorePages = false;
                        break;
                    }

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

                    var pageSource = driver.PageSource;
                    var htmlDir = Path.Combine(_outputDir, "html");
                    Directory.CreateDirectory(htmlDir); 
                    var htmlFilePath = Path.Combine(htmlDir, $"pagina_{page}.html");

                    try
                    {
                        await File.WriteAllTextAsync(htmlFilePath, pageSource);
                        Console.WriteLine($"Página {page} salva como HTML em: {htmlFilePath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao salvar HTML da página {page}: {ex.Message}");
                    }

                    page++; 
                }

                string json = JsonConvert.SerializeObject(proxies, Formatting.Indented);
                var jsonDir = Path.Combine(_outputDir, "json");
                Directory.CreateDirectory(jsonDir); 
                var filePath = Path.Combine(jsonDir, $"proxies_{DateTime.Now:yyyyMMddHHmmss}.json");
                await File.WriteAllTextAsync(filePath, json);

                Console.WriteLine($"Arquivo JSON salvo em: {filePath}");

                var log = new CrawlerLogs
                {
                    StartTime = startTime,
                    EndTime = DateTime.Now,
                    TotalPages = page - 1,
                    TotalRows = totalRows,
                    JsonPath = filePath
                };

                return log;
            }
        }
    }
}
