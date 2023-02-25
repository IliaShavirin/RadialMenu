namespace BaseProj.SharpVectors.SharpVectorCss.Css
{
    internal sealed class CssProperty
    {
        internal CssValue InitialCssValue;
        internal string InitialValue;
        internal bool IsInherited;

        internal CssProperty(bool isInherited, string initialValue)
        {
            IsInherited = isInherited;
            InitialValue = initialValue;
            InitialCssValue = null;
        }
    }
}