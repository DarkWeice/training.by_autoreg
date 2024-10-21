using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using PuppeteerSharp;
using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.ExtraStealth;

namespace TrainingByAutoReg
{
    internal class TestProgram
    {

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
            Browser? browser = null;
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

                var launchOptions = new LaunchOptions   
                {
                    Headless = false, // =false for test
                };

                using (var browsera = await Puppeteer.LaunchAsync(launchOptions))
                using (var page = await browsera.NewPageAsync())
                {
                    //await page.GoToAsync("https://www.sandbox.game/en/sign");
                    //await page.WaitForNavigationAsync();

                    await page.GoToAsync("https://www.sandbox.game/en/sign/");

                    var cap1 = await page.QuerySelectorAsync("switch-method__button");
                    await cap1.ClickAsync();
                    await page.WaitForNavigationAsync();

                    //var input_mail = await page.QuerySelectorAsync("CustomInput__input");
                    await page.TypeAsync("CustomInput__input", $"{account.Email}");

                    var signup_mail = await page.QuerySelectorAsync("sign-up-email-btn");
                    await signup_mail.ClickAsync();
                }
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
