using Newtonsoft.Json;

namespace INFT3500.Helpers;

public static class SessionHelper
{
    public static void SerializeSession(this ISession session, string key, object value)
    {
        session.SetString(key, JsonConvert.SerializeObject(value));
    }

    public static T? DeserializeSession<T>(this ISession session, string key)
    {
        var sessionString = session.GetString(key);
        return sessionString == null ? default! : JsonConvert.DeserializeObject<T>(sessionString);
    }
}