using Microsoft.AspNetCore.Http;
using System.Text.Json;


/*
 Usage:
directive: 
    using onlineexamproject.Extensions;

call:        
    User? sessionUser = HttpContext.Session.GetObjectFromJson<User>("string");
    
    here the "string" is a unique identifier for that specific object. it is a key!
 */

namespace onlineexamproject.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T? GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
