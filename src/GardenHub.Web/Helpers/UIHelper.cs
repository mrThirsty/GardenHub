using System.ComponentModel;
using System.Reflection;

namespace GardenHub.Web.Helpers;

public static class UIHelper
{
    public static string GetFriendlyEnum<T>(this T enumValue) where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
            return null;

        string description = enumValue.ToString();
        
        var fieldInfo = typeof(T).GetField(enumValue.ToString());

        if (fieldInfo != null)
        {
            var attrs = fieldInfo.GetCustomAttributes<DescriptionAttribute>(false);
            
            if (attrs != null && attrs.Any())
            {
                description = attrs.First().Description;
            }
        }

        return description.StartsWith("PropertyDescription") ? description.Replace("PropertyDescription", "") : description;
    }
}