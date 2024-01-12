using Demo.WebApi.Application.Common.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Demo.WebApi.Shared.Services.Common.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum enumValue)
    {
        object[] attr = enumValue.GetType().GetField(enumValue.ToString())!
            .GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attr.Length > 0)
            return ((DescriptionAttribute)attr[0]).Description;
        string result = enumValue.ToString();
        result = Regex.Replace(result, "([a-z])([A-Z])", "$1 $2");
        result = Regex.Replace(result, "([A-Za-z])([0-9])", "$1 $2");
        result = Regex.Replace(result, "([0-9])([A-Za-z])", "$1 $2");
        result = Regex.Replace(result, "(?<!^)(?<! )([A-Z][a-z])", " $1");
        return result;
    }

    public static List<string> GetDescriptionList(this Enum enumValue)
    {
        string result = enumValue.GetDescription();
        return result.Split(',').ToList();
    }

    public static List<string> GetEnumList<T>()
        where T : Enum
    {
       return Enum.GetValues(typeof(T))
               .Cast<T>()
               .Select(v => v.ToString())
               .ToList();
    }

    public static string? GetObject<TEnum>(this TEnum value)
       where TEnum : Enum?
    {
        if (value == null) return null;
        return JsonConvert.SerializeObject(
           new LookupResponse
           {
               Id = Convert.ToInt32(value),
               Name = value.ToString()
           },
           new JsonSerializerSettings
           {
               ContractResolver = new CamelCasePropertyNamesContractResolver()
           });
    }

    public static object ToObject<TEnum>(this TEnum value)
       where TEnum : Enum
       => new LookupResponse
       {
           Id = Convert.ToInt32(value),
           Name = value.ToString()//Enum.GetName(typeof(TEnum), value)!
       };

    public static TEnum GetEnumValue<TEnum>(this int value)
        where TEnum : Enum
        => (TEnum)Enum.Parse(typeof(TEnum), value.ToString(), true);

}