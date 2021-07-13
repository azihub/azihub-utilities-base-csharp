using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Azihub.Utilities.Base.Extensions.Object
{
    public static class QueryString
    {
        /// <summary>
        /// Generate a Web QueryString using reflection from any object with public properties that has
        /// value other than null
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetQueryString(this object obj)
        {
            IEnumerable<string> properties = obj.GetType().GetRuntimeProperties()
                .Where(p => p.GetValue(obj, null) != null)
                .Select(p => p.Name + "=" + WebUtility.UrlEncode(p.GetValue(obj, null).ToString()));

            return string.Join("&", properties.ToArray());
        }
    }
}