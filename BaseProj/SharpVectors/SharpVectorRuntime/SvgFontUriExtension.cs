using System;
using System.Windows.Markup;

namespace BaseProj.SharpVectors.SharpVectorRuntime
{
    public sealed class SvgFontUriExtension : MarkupExtension
    {
        private readonly string _inputUri;

        public SvgFontUriExtension(string inputUri)
        {
            _inputUri = Environment.ExpandEnvironmentVariables(inputUri);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new Uri(_inputUri);
        }
    }
}