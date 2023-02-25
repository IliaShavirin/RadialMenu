using System;
using System.ComponentModel;

namespace BaseProj.ExtensionMethods
{
    [AttributeUsage(AttributeTargets.All)]
    public class ValueAttribute : DescriptionAttribute
    {
        public ValueAttribute(string val) : base(val)
        {
        }
    }
}