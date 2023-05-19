using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkFlowEngine.Web.Models.Extensions
{
    /// <summary>
    /// Extension methods to set and get serializable objects
    /// </summary>
    public static class SessionExtensions
    {
        /// <summary>
        /// Set the object by session key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        /// <summary>
        /// Get the serialized object by session key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) :
                JsonConvert.DeserializeObject<T>(value);
        }
    }
    /******************* USAGE OF ABOVE EXTENSION METHOD *********************/
    // Requires you add the Set and Get extension method mentioned in the topic.
    //if (HttpContext.Session.Get<DateTime>(SessionKeyTime) == default(DateTime))
    //{
    //    HttpContext.Session.Set<DateTime>(SessionKeyTime, currentTime);
    //}

    public static class StringExtensions
    {
        public static bool HasSpecialChar(this String input)
        {
            string specialChar = @"\|!#$%&/()=?»«@£§€{}.-;'<>_,";
            foreach (var item in specialChar)
            {
                if (input.Contains(item)) return true;
            }

            return false;
        }
    }
}
