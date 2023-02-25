namespace BaseProj.SharpVectors.SharpVectorCore.Css
{
    /// <summary>
    ///     The CSSFontFaceRule interface represents a @font-face rule
    ///     in a CSS style sheet. The @font-face rule is used to hold a
    ///     set of font descriptions.
    /// </summary>
    /// <developer>niklas@protocol7.com</developer>
    /// <completed>20</completed>
    public interface ICssFontFaceRule : ICssRule
    {
        /// <summary>
        ///     The declaration-block of this rule.
        /// </summary>
        ICssStyleDeclaration Style { get; }
    }
}