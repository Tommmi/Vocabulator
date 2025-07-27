using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vocabulator.Common
{
    public static class EnumerableExtension
    {
        public static string StringJoin(this IEnumerable<string> strings, string delimiter)
        {
            var result = new StringBuilder();
            bool firstTime = true;
            foreach (var item in strings)
            {
                if(firstTime)
                {
                    firstTime = false;
                }
                else
                {
                    result.Append(delimiter);
                }

                result.Append(item);
            }
            return result.ToString();
        }
    }
}
