using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace BaseProj.SharpVectors.SharpVectorRuntime
{
    public sealed class EmbeddedBitmapDataConverter : TypeConverter
    {
        #region Constructors and Destructor

        #endregion

        #region Public Methods

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string)) return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var toDecode = Convert.FromBase64String((string)value);

            var memoryStream = new MemoryStream(toDecode);
            return new EmbeddedBitmapData(memoryStream);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var bitmapInfo = (EmbeddedBitmapData)value;
                var memoryStream = bitmapInfo.Stream;

                return Convert.ToBase64String(memoryStream.ToArray());
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion Methods
    }
}