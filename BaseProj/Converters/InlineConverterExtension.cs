using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public class InlineConverterExtension : MarkupExtension
    {
        private static readonly Dictionary<string, WeakReference> s_WeakReferenceLookup;

        static InlineConverterExtension()
        {
            s_WeakReferenceLookup = new Dictionary<string, WeakReference>();
        }

        public InlineConverterExtension()
        {
        }

        public InlineConverterExtension(Type converterType)
        {
            ConverterType = converterType;
        }

        /// <summary>
        ///     The type of the converter to create
        /// </summary>
        /// <value>The type of the converter.</value>
        public Type ConverterType { get; set; }

        /// <summary>
        ///     The optional arguments for the converter's constructor.
        /// </summary>
        /// <value>The argumments.</value>
        public object[] Arguments { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var target = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));

            var propertyInfo = target.TargetProperty as PropertyInfo;

            if (!propertyInfo.PropertyType.IsAssignableFrom(typeof(IValueConverter)))
                throw new NotSupportedException("Property '" + propertyInfo.Name +
                                                "' is not assignable from IValueConverter.");

            Debug.Assert(ConverterType != null,
                "ConverterType is has not been set, ConverterType{x:Type converterType}");

            var key = ConverterType.ToString();

            if (Arguments != null)
            {
                var args = new List<string>();
                foreach (var obj in Arguments)
                    args.Add(obj.ToString());

                key = string.Concat(key, "_", string.Join("|", args.ToArray()));
            }

            WeakReference wr = null;
            if (s_WeakReferenceLookup.TryGetValue(key, out wr))
            {
                if (wr.IsAlive)
                    return wr.Target;
                s_WeakReferenceLookup.Remove(key);
            }

            var converter = Arguments == null
                ? Activator.CreateInstance(ConverterType)
                : Activator.CreateInstance(ConverterType, Arguments);
            s_WeakReferenceLookup.Add(key, new WeakReference(converter));

            return converter;
        }
    }
}