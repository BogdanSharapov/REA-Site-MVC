using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace REASite.Models
{
    public static class EnumExtension
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            .Name;
        }
    }
}
