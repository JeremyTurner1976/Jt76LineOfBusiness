using System;
using System.Collections.Generic;

namespace JT76.Common.ObjectExtensions
{
    public static class StringExtensions
    {
        public static IEnumerable<string> SplitOnNewLines(this string strSource)
        {
            return strSource.Split(new[] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
        }

        public static IEnumerable<string> SplitOnBreaks(this string strSource)
        {
            return strSource.Split(new[] {"<br/>"}, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}