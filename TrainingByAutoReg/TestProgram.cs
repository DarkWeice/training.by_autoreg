using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using PuppeteerSharp;
using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using Newtonsoft.Json;
using System.Text;
using PuppeteerSharp.BrowserData;

namespace TrainingByAutoReg
{
    internal class TestProgram
    {
        // test
        private struct Account
        {
            public string Email { get; set; }
            public string Login { get; set; }
            public string Password { get; set; }

            public override string ToString()
            {
                return $"{Email}:{Password} | {Login}";
            }
        }

        internal static async Task Main(string[] args)
        {
            var fetcher = new BrowserFetcher(new BrowserFetcherOptions());

            bool result = false;
            if (args.Length > 0)
            {
                int number = int.Parse(args[0]);
                for (var i = 0; i < number; i++)
                {
                    result = await RunAsync();
                    if (result == false)
                    {
                        break;
                    }
                }
            }
            else
            {
                result = await RunAsync();
            }

            if (result == false)
            {
                Console.WriteLine("Fail.");
            }
            else
            {
                Console.WriteLine("Success. See the accounts.txt file.");
            }

        }

        private static async Task<bool> RunAsync()
        {
            IBrowser? browser = null;
            try
            {
                GenerateNewEmailAddress generateNewEmailAddress = new GenerateNewEmailAddress();
                string emailAddress = generateNewEmailAddress.GenerateNewEmailAddresss();


                var account = new Account()
                {
                    Login = emailAddress, //Исправить как было!!
                    Email = emailAddress,
                    Password = $"2{emailAddress}3",
                };

                await new BrowserFetcher().DownloadAsync();
                await Task.Delay(5000);
                var profilePath = @"C:\Users\pakapaka\AppData\Local\Google\Chrome\User Data\Profile 1";

                
                var launchOptions = new LaunchOptions
                {
                    Headless = false, // =false for test
                    UserDataDir = profilePath,
                    DefaultViewport = new ViewPortOptions { Width = 800, Height = 800 }
                };

                browser = await Puppeteer.LaunchAsync(launchOptions);
                var page = await browser.NewPageAsync();

                await page.SetUserAgentAsync("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

                await page.GoToAsync("https://www.sandbox.game/en/sign/?redirectTo=%2Fevents%2Falpha-season-4%2F&createAccount=true&mktTarget=undefined&showOnboarding=undefined");
                var cookies = new List<CookieParam>
                {
                    new CookieParam
                    {
                        Name = "my_cookie",
                        Value = "cookie_value",
                        Domain = "example.com", // Замените на ваш домен Path = "/",
                        HttpOnly = false,
                        Secure = false,
                        SameSite = SameSite.Lax // Или CookieSameSite.Strict, в зависимости от ваших требований
                    }
                };

                await page.SetCookieAsync(cookies.ToArray());
                Console.WriteLine("Cookies установлены:");

                foreach (var cookie in cookies)
                {
                    Console.WriteLine($"Name: {cookie.Name}, Value: {cookie.Value}");
                }

                var currentCookies = await page.GetCookiesAsync();
                Console.WriteLine("Текущие куки:");

                await page.TypeAsync("#input.textAlignLeft", $"{account.Email}");
                await Task.Delay(500);
                await page.WaitForSelectorAsync(".custom-button");
                await page.ClickAsync(".custom-button");

                await page.WaitForNetworkIdleAsync();
                await Task.Delay(5000);


                var accountsFilePath = Path.Combine(AppContext.BaseDirectory, "accounts.txt");
                if (new FileInfo(accountsFilePath).Exists == false)
                {
                    File.Create(accountsFilePath).Close();
                }

                await File.AppendAllTextAsync(accountsFilePath, $"{account.ToString()}\n", Encoding.UTF8);


                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            finally
            {
                if (browser != null)
                {
                    await browser.CloseAsync();
                }
            }
        }
    }
}
