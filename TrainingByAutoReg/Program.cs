using System.Text;
using _10MinuteMail.net;
using _10MinuteMail.net.Types;
using PuppeteerSharp;
using PuppeteerSharp.Input;

namespace TrainingByAutoReg
{
    internal class Program
    {
        internal static async Task Main(string[] args)
        {
            BrowserFetcher fetcher = Puppeteer.CreateBrowserFetcher(new BrowserFetcherOptions());
            fetcher.DownloadProgressChanged += (_, eventArgs) =>
                Console.WriteLine($"[BrowserFetcher] Downloading progress: {eventArgs.ProgressPercentage}");
            await fetcher.DownloadAsync();

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
            try
            {
                var tempMailService = new TenMinuteMail();
                await tempMailService.GenerateNewEmailAddress();
                string emailAddress = await tempMailService.GetEmailAddress();

                var account = new Account()
                {
                    Login = emailAddress,
                    Password = $"{emailAddress}!",
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    PhoneNumber = new Random().NextInt64(375290000000, 375299999999).ToString(),
                    FirstName = Faker.Name.First(),
                    LastName = Faker.Name.Last(),
                };

                Browser browser = await Puppeteer.LaunchAsync(new LaunchOptions {Headless = false});

                Page page = await browser.NewPageAsync();
                await page.GoToAsync("https://training.by/Auth/Login?redirectUrl=#!");
                Task<Response> navTask1 = page.WaitForNavigationAsync(new NavigationOptions()
                {
                    WaitUntil = new[]
                    {
                        WaitUntilNavigation.Networkidle0
                    }
                });
                await page.ClickAsync("#registerNow");
                await navTask1;

                await page.TypeAsync("#email", $"{account.Login}");
                await page.ClickAsync("#kc-login-step1");

                await page.WaitForSelectorAsync("#kc-login-step2", new WaitForSelectorOptions() {Visible = true});
                await page.TypeAsync("#firstName", $"{account.FirstName}");

                // Removing the value of lastname's field
                ElementHandle lastnameField = await page.QuerySelectorAsync("#lastName");
                await lastnameField.ClickAsync(new ClickOptions {ClickCount = 3});
                await lastnameField.PressAsync("Backspace");

                await page.TypeAsync("#lastName", $"{account.LastName}");
                await page.TypeAsync("#password", $"{account.Password}");
                await page.TypeAsync("#password-confirm", $"{account.Password}");

                await page.ClickAsync("#kc-login-step2");

                await page.WaitForSelectorAsync("#confirmNotice", new WaitForSelectorOptions() {Visible = true});
                await page.EvaluateExpressionAsync("document.querySelector('#confirmNotice').parentElement.click()");

                await page.WaitForSelectorAsync("#alert-error",
                    new WaitForSelectorOptions() {Visible = true, Timeout = 0});

                while ((await tempMailService.GetEmails()).Length < 2)
                {
                    await Task.Delay(2000);
                }

                MailContent[] emails = await tempMailService.GetEmails();
                string activateUrl = emails[0].Urls[0];

                await page.GoToAsync(activateUrl, WaitUntilNavigation.Networkidle0);

                await page.TypeAsync("#phone", account.PhoneNumber);
                await page.SelectAsync("select[name='city']", "number:1");
                await page.SelectAsync("select[name='city0']", "number:1");
                await page.SelectAsync("select[name='university0']", "number:459");
                await page.SelectAsync("select[name='faculty0']", "number:739");
                await page.SelectAsync("select[name='department0']", "number:2915");
                await page.SelectAsync("select[name='educationBusy0']", "number:1");
                await page.SelectAsync("select[name='degree0']", "number:3");
                await page.SelectAsync("select[name='admissionYear0']", "number:1");
                await page.SelectAsync("select[name='graduationYear0']", "number:2026");
                await page.SelectAsync("select[name='englishLevel']", "number:5");

                await page.ClickAsync("button[class='button button--green save ng-binding']");
                await page.WaitForNavigationAsync();

                var accountsFilePath = Path.Combine(AppContext.BaseDirectory, "accounts.txt");
                if (new FileInfo(accountsFilePath).Exists == false)
                {
                    File.Create(accountsFilePath).Close();
                }

                await File.AppendAllTextAsync(accountsFilePath, $"{account.ToString()}\n", Encoding.UTF8);

                await browser.CloseAsync();


                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        private struct Account
        {
            public string Login { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string PhoneNumber { get; set; }
            public DateOnly Date { get; set; }

            public override string ToString()
            {
                return $"{Login}:{Password} | {FirstName} {LastName} | {Date.ToShortDateString()}";
            }
        }
    }
}