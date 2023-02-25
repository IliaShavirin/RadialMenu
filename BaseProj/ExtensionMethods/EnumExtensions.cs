using System;
using System.ComponentModel;

namespace BaseProj.ExtensionMethods
{
    public static class EnumExtensions
    {
        public static string TryGetDescription(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray.Length == 0) return value.ToString();

            foreach (var attribute in attribArray)
                if (attribute is DescriptionAttribute attrib)
                    return attrib.Description;

            return "";
        }
    }
}