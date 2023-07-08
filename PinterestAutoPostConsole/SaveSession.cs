using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using Cookie = OpenQA.Selenium.Cookie;

namespace PinterestAutoPostConsole
{
    

    class SaveSession
    {
        private string _MAIL;
        private string _PASS;

        private FirefoxDriver SaveCookies(){
            
            // Read mail:pass

            System.Console.WriteLine("Введите почту: ");
            _MAIL = Console.ReadLine()!;
            System.Console.WriteLine("Введите пароль: ");
            _PASS = Console.ReadLine()!;

            // Create a new FirefoxDriver

            var options = new FirefoxOptions();
            options.PageLoadStrategy = PageLoadStrategy.None;
            var driver = new FirefoxDriver(options);
            

            // Perform authentication and navigate to the page you want to save the session for

            driver.Navigate().GoToUrl("https://www.pinterest.com/login/");
            Thread.Sleep(6000);
            driver.FindElement(By.Name("id")).SendKeys(_MAIL);
            driver.FindElement(By.Name("password")).SendKeys(_PASS);
            driver.FindElement(By.ClassName("SignupButton")).Click();
            Thread.Sleep(6000);

            // Get the cookies from the browser
            var cookies = driver.Manage().Cookies.AllCookies;

            string jsonString = JsonConvert.SerializeObject(cookies);

            // Write the JSON string to a file
            string filename = "cookies.json";

            using (StreamWriter writer = File.CreateText(filename))
            {
                writer.Write(jsonString);
            }
            return driver;
        }
        public FirefoxDriver SaveAndGetSession(){
            
            FirefoxDriver driver;

            string filename = "cookies.json";
            if(!File.Exists(filename)){
                driver = SaveCookies();
            }
            else{
                var options = new FirefoxOptions();
                options.PageLoadStrategy = PageLoadStrategy.None;
                driver = new FirefoxDriver(options);
                Thread.Sleep(2000);
            }
            
            // Read the JSON string from the file
            string jsonString = File.ReadAllText(filename);

            // Deserialize the JSON string to a list of dictionary objects
            var cookiesList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonString);

            // Convert the dictionary objects to Cookie objects
            var cookies = new List<OpenQA.Selenium.Cookie>();
            foreach (var cookieDict in cookiesList)
            {
                DateTime? expiry = null;
                if (!string.IsNullOrEmpty(cookieDict["expiry"]))
                {
                    TimeSpan ts = TimeSpan.FromSeconds(int.Parse(cookieDict["expiry"])); //с ним все в порядке
                    DateTime dt = new DateTime(1970, 1, 1); 
                    expiry = dt+ts;
                }

                OpenQA.Selenium.Cookie cookie = new OpenQA.Selenium.Cookie(
                    cookieDict["name"],
                    cookieDict["value"],
                    ".pinterest.com",
                    cookieDict["path"],
                    expiry,
                    bool.Parse(cookieDict["secure"]),
                    bool.Parse(cookieDict["httpOnly"]),
                    cookieDict["sameSite"]

                );

                cookies.Add(cookie);
            }

            // Create a Collection<OpenQA.Selenium.Cookie> object from the list of cookies
            var collection = new Collection<OpenQA.Selenium.Cookie>(cookies);

            // driver.ExecuteScript("window.open();");
            // // Switch to the new window or tab
            // driver.SwitchTo().Window(driver.WindowHandles.Last());
            // // Navigate to the URL
            driver.Navigate().GoToUrl("https://www.pinterest.com/");
            Thread.Sleep(5000);
            // Set the cookies in the new window or tab
            foreach (var cookie in collection)
            {
                driver.Manage().Cookies.AddCookie(cookie);
            }
            driver.Navigate().GoToUrl("https://www.pinterest.com/");

            // Now when you navigate to the site, you should already be authenticated
            Thread.Sleep(4000);
            return driver;
        }
    }
}