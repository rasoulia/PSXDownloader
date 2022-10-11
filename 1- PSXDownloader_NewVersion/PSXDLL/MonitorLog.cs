using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace PSXDLL
{
    public class MonitorLog
    {

        /// <summary>
        /// matching link
        /// </summary>
        /// <returns></returns>
        public static bool RegexUrl(string urls)
        {
            string[]? rules = AppConfig.Instance().Rule!.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (rules.Length <= 0 || string.IsNullOrEmpty(urls))
            {
                return false;
            }

            return
                rules.Select(rule => new Regex(rule.ToLower().Replace(".", @"\.").Replace("*", ".*?")))
                     .Any(regex => regex.Match(urls.ToLower()).Success);
        }

    }
}
