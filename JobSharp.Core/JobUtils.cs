using OpenQA.Selenium;

namespace JobSharp.Core;

public static class JobUtils
{
    public static void HumanLikeTyping(IWebElement element, string text)
    {
        foreach (char c in text)
        {
            element.SendKeys(c.ToString());
            Thread.Sleep(new Random().Next(50, 150));
        }
    }

    public static void RandomDelay(int minMs, int maxMs)
    {
        var random = new Random();
        Thread.Sleep(random.Next(minMs, maxMs));
    }
}
