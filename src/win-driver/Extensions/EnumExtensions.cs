using System;
using System.ComponentModel;

namespace WinDriver.Extensions
{
    public static class EnumExtensions
    {
        public static string Description<T>(this object enumValue) where T : struct
        {
            var type = enumValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("enumValue must be an enum type", "enumValue");
            }

            var member = type.GetMember(enumValue.ToString());
            if (member.Length > 0)
            {
                var attributes = member[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length > 0)
                {
                    return ((DescriptionAttribute)attributes[0]).Description;
                }
            }

            return enumValue.ToString();
        }
    }
}