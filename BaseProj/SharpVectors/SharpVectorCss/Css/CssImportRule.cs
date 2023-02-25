// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Css;
using BaseProj.SharpVectors.SharpVectorCore.Stylesheets;
using BaseProj.SharpVectors.SharpVectorCss.Stylesheets;

namespace BaseProj.SharpVectors.SharpVectorCss.Css
{
    /// <summary>
    ///     The CSSImportRule interface represents a @import rule within a CSS style sheet. The @import rule is used to import
    ///     style rules from other style sheets.
    /// </summary>
    public class CssImportRule : CssRule, ICssImportRule
    {
        #region Constructors

        /// <summary>
        ///     The constructor for CssImportRule
        /// </summary>
        /// <param name="match">The Regex match that found the charset rule</param>
        /// <param name="parent">The parent rule or parent stylesheet</param>
        /// <param name="readOnly">True if this instance is readonly</param>
        /// <param name="replacedStrings">
        ///     An array of strings that have been replaced in the string used for matching. These needs
        ///     to be put back use the DereplaceStrings method
        /// </param>
        /// <param name="origin">The type of CssStyleSheet</param>
        internal CssImportRule(Match match, object parent, bool readOnly,
            IList<string> replacedStrings, CssStyleSheetType origin)
            : base(parent, readOnly, replacedStrings, origin)
        {
            media = new MediaList(match.Groups["importmedia"].Value);
            Href = DeReplaceStrings(match.Groups["importhref"].Value);

            styleSheet = new CssStyleSheet(ResolveOwnerNode(), Href, null,
                match.Groups["importmedia"].Value, this, origin);
        }

        #endregion

        #region Implementation of ICssRule

        /// <summary>
        ///     The type of the rule. The expectation is that binding-specific casting methods can be used to cast down from an
        ///     instance of the CSSRule interface to the specific derived interface implied by the type.
        /// </summary>
        public override CssRuleType Type => CssRuleType.ImportRule;

        #endregion

        #region Internal methods

        /// <summary>
        ///     Used to find matching style rules in the cascading order
        /// </summary>
        /// <param name="elt">The element to find styles for</param>
        /// <param name="pseudoElt">The pseudo-element to find styles for</param>
        /// <param name="ml">The medialist that the document is using</param>
        /// <param name="csd">A CssStyleDeclaration that holds the collected styles</param>
        protected internal override void GetStylesForElement(XmlElement elt, string pseudoElt, MediaList ml,
            CssCollectedStyleDeclaration csd)
        {
            if (media.Matches(ml)) ((CssStyleSheet)StyleSheet).GetStylesForElement(elt, pseudoElt, ml, csd);
        }

        #endregion

        #region Static members

        private static readonly Regex regex =
            new Regex(@"^@import\s(url\()?""(?<importhref>[^""]+)""\)?(\s(?<importmedia>([a-z]+)(\s*,\s*)?)+)?;");

        internal static CssRule Parse(ref string css, object parent, bool readOnly, IList<string> replacedStrings,
            CssStyleSheetType origin)
        {
            var match = regex.Match(css);
            if (match.Success)
            {
                var rule = new CssImportRule(match, parent, readOnly, replacedStrings, origin);
                css = css.Substring(match.Length);
                return rule;
            }

            // didn't match => do nothing
            return null;
        }

        #endregion

        #region Implementation of ICssImportRule

        private readonly CssStyleSheet styleSheet;

        /// <summary>
        ///     The style sheet referred to by this rule, if it has been loaded. The value of this attribute is null if the style
        ///     sheet has not yet been loaded or if it will not be loaded (e.g. if the style sheet is for a media type not
        ///     supported by the user agent).
        /// </summary>
        public ICssStyleSheet StyleSheet => styleSheet;

        private readonly MediaList media;

        /// <summary>
        ///     A list of media types for which this style sheet may be used.
        /// </summary>
        public IMediaList Media => media;

        /// <summary>
        ///     The location of the style sheet to be imported. The attribute will not contain the "url(...)" specifier around the
        ///     URI
        /// </summary>
        public string Href { get; }

        #endregion
    }
}