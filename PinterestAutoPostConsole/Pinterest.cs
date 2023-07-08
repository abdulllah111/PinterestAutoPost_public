using System;
using System.IO;
using ClosedXML;
using ClosedXML.Excel;
using ClosedXML.Graphics;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace PinterestAutoPostConsole{
    
    public class PinterestAutoPoster
    {
        private FirefoxDriver _driver;

        public PinterestAutoPoster(FirefoxDriver driver)
        {
            _driver = driver;
        }

        public void PostPins()
        {
            // System.Console.WriteLine("Введите путь до excel файла");
            // string _excelFilePath = Console.ReadLine();

            string _excelFilePath = "/run/media/abdul/App/MyProjects/NET/PinterestAutoPost/PinterestAutoPostConsole/jkt3.xlsx";
            // Load the Excel document using ClosedXML
            LoadOptions.DefaultGraphicEngine = new DefaultGraphicEngine("Times New Roman Cyr");

            var workbook = new XLWorkbook(_excelFilePath);
            var worksheet = workbook.Worksheet(1);

            // Loop through the rows in the Excel document and post each pin on Pindoterest
            foreach (var row in worksheet.RowsUsed())
            {   
                var pinTitle = row.Cell(1).Value.ToString();
                var pinDescription = row.Cell(2).Value.ToString();
                var pinImageUrl = row.Cell(3).Value.ToString();
                var pinLink = row.Cell(4).Value.ToString();

                _driver.Navigate().GoToUrl("https://www.pinterest.com/pin-builder/");
                Thread.Sleep(7500);
                _driver.FindElement(By.CssSelector("textarea[placeholder='Добавьте название']")).SendKeys(pinTitle);
                _driver.FindElement(By.ClassName("notranslate")).SendKeys(pinDescription);
                _driver.FindElement(By.CssSelector("textarea[placeholder='Добавьте целевую ссылку']")).SendKeys(pinLink);
                _driver.FindElement(By.CssSelector("input[aria-label='Загрузка файла']")).SendKeys(pinImageUrl);
                Thread.Sleep(2000);
                _driver.FindElement(By.CssSelector("button[data-test-id='board-dropdown-select-button']")).Click();
                Thread.Sleep(2000);
                _driver.FindElement(By.CssSelector("div[data-test-id='board-row-Здоровый ЖКТ']")).Click();
                _driver.FindElement(By.CssSelector("button[data-test-id='board-dropdown-save-button']")).Click();
                Thread.Sleep(60000);
            }
            
            _driver.Navigate().GoToUrl("https://ru.pinterest.com/Pohudeniye/");
        }
    }
}
