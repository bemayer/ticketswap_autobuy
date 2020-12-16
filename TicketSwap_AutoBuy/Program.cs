using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace TicketSwapAutoBuy
{
    class Program
    {
        static void Main(string[] args)
        {

            //param
            string fbId = ""; //Facebook Id
            string fbPw = ""; //Facebook Password
            string eUrl = ""; //Event Url

            //log to Facebook
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://www.facebook.com");
            IWebElement SearchText = driver.FindElement(By.Name("email"));
            SearchText.SendKeys(fbId);
            IWebElement SearchTextp = driver.FindElement(By.Name("pass"));
            SearchTextp.SendKeys(fbPw);
            SearchText.Submit();

            //log to TicketSwap
            driver.Navigate().GoToUrl("https://www.ticketswap.com/");
            try { driver.FindElement(By.LinkText("Login")).Click(); }
            catch { driver.FindElement(By.LinkText("Se connecter")).Click(); }

            //get new URLs
            HtmlWeb web = new HtmlWeb();
            List<string> oldurlList = new List<string>();
            List<string> newurlList = new List<string>();
            List<string> urlList = new List<string>();
            int tryNb = 0;
            while (newurlList.Count.Equals(0))
            {
                System.Threading.Thread.Sleep(1000);
                HtmlDocument doc = web.Load(eUrl);
                var links = doc.DocumentNode.Descendants("a").Where(d => d.OuterHtml.Contains("offerurl"));
                if (oldurlList.Count.Equals(0))
                    foreach (var link in links) {oldurlList.Add(link.GetAttributeValue("href", "no address"));}
                urlList.Clear();
                foreach (var link in links) {urlList.Add(link.GetAttributeValue("href", "no address"));}
                newurlList = urlList.Except(oldurlList).ToList();
            }

            //add tickets to basket
            foreach (string url in newurlList)
            {
                driver.Navigate().GoToUrl("https://www.ticketswap.com" + url);
                driver.FindElement(By.Id("listing-reserve-form")).Submit();
            }
        }
    }
}

