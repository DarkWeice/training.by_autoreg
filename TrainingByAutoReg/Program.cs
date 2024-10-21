//using System.Net.Mail;
//using System.Text;
//using _10MinuteMail.net;
//using _10MinuteMail.net.Types;
//using PuppeteerSharp;
//using PuppeteerSharp.Input;
//using Aspose.Email;

//namespace TrainingByAutoReg
//{
//    public class Program
//    {
//        internal static async Task dMain(string[] args)
//        {
//            var fetcher = new BrowserFetcher(new BrowserFetcherOptions());
//            fetcher.DownloadProgressChanged += (_, eventArgs) =>
//                Console.WriteLine($"[BrowserFetcher] Downloading progress: {eventArgs.ProgressPercentage}");
//            await fetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);

//            bool result = false;
//            if (args.Length > 0)
//            {
//                int number = int.Parse(args[0]);
//                for (var i = 0; i < number; i++)
//                {
//                    result = await RunAsync();
//                    if (result == false)
//                    {
//                        break;
//                    }
//                }
//            }
//            else
//            {
//                result = await RunAsync();
//            }

//            if (result == false)
//            {
//                Console.WriteLine("Fail.");
//            }
//            else
//            {
//                Console.WriteLine("Success. See the accounts.txt file.");
//            }

//        }

//        private static async Task<bool> RunAsync()
//        {
//            GenerateNewEmailAddress generateNewEmailAddress = new GenerateNewEmailAddress();
//            Browser? browser = null;
//            try
//            {
//                //var tempMailService = new TenMinuteMail();
//                //await tempMailService.GenerateNewEmailAddress();
//                string emailAddress = generateNewEmailAddress.GenerateNewEmailAddresss();
                
//                var account = new Account()
//                {
//                    Login = emailAddress, //Исправить как было!!
//                    Email = emailAddress,
//                    Password = $"2{emailAddress}3",
//                };

//                browser = await Puppeteer.LaunchAsync(new LaunchOptions {Headless = false});

//                Page page = await browser.NewPageAsync();
//                await page.GoToAsync("https://www.sandbox.game/en/sign/?redirectTo=%2Fevents%2Falpha-season-4%2F&createAccount=true&mktTarget=undefined&showOnboarding=undefined");
//                Task<Response> navTask1 = page.WaitForNavigationAsync(new NavigationOptions()
//                {
//                    WaitUntil = new[]
//                    {
//                        WaitUntilNavigation.Networkidle0
//                    }
//                });

//                await navTask1;

//                await page.TypeAsync("#input", $"{account.Email}");
//                await page.ClickAsync("#sign-up-email-btn");

//                await page.WaitForSelectorAsync("#signup-button", new WaitForSelectorOptions() {Visible = true});
//                await page.TypeAsync("#input", $"{account.Login}");

//                await page.TypeAsync("#password-input", $"{account.Password}");

//                await page.ClickAsync("#checkbox");
//                await page.ClickAsync("#signup-button");

//                //await page.WaitForSelectorAsync("#confirmNotice", new WaitForSelectorOptions() { Visible = true });
//                //await page.EvaluateExpressionAsync("document.querySelector('#confirmNotice').parentElement.click()");

//                //await page.WaitForSelectorAsync("#alert-error",
//                //    new WaitForSelectorOptions() { Visible = true, Timeout = 0 });

//                //while ((await tempMailService.GetEmails()).Length < 2)
//                //{
//                //    await Task.Delay(2000);
//                //}

//                //MailContent[] emails = await tempMailService.GetEmails();
//                //string activateUrl = emails[0].Urls[0];

//                //await page.GoToAsync(activateUrl, WaitUntilNavigation.Networkidle0);

//                Task<Response> navTask2 = page.WaitForNavigationAsync(new NavigationOptions()
//                {
//                    WaitUntil = new[]
//                    {
//                        WaitUntilNavigation.Load,
//                        WaitUntilNavigation.Networkidle2,
//                    }
//                });
//                await page.EvaluateExpressionAsync("document.getElementById('saveProfile').click()");
//                await navTask2;

//                var accountsFilePath = Path.Combine(AppContext.BaseDirectory, "accounts.txt");
//                if (new FileInfo(accountsFilePath).Exists == false)
//                {
//                    File.Create(accountsFilePath).Close();
//                }

//                await File.AppendAllTextAsync(accountsFilePath, $"{account.ToString()}\n", Encoding.UTF8);

//                return true;
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e);
//                return false;
//            }
//            finally
//            {
//                if (browser != null)
//                {
//                    await browser.CloseAsync();
//                }
//            }
//        }

//        private struct Account
//        {
//            public string Email { get; set; }
//            public string Login { get; set; }
//            public string Password { get; set; }

//            public override string ToString()
//            {
//                return $"{Email}:{Password} | {Login}";
//            }
//        }
//    }
//}