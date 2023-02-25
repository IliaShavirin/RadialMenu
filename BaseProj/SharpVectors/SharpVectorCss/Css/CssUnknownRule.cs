// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System.Collections.Generic;
using System.Text.RegularExpressions;
using BaseProj.SharpVectors.SharpVectorCore.Css;

namespace BaseProj.SharpVectors.SharpVectorCss.Css
{
    /// <summary>
    ///     The CSSUnknownRule interface represents an at-rule not supported by this user agent.
    /// </summary>
    public sealed class CssUnknownRule : CssRule, ICssUnknownRule
    {
        #region Constructors

        /// <summary>
        ///     The constructor for CssUnknownRule
        /// </summary>
        internal CssUnknownRule(object parent, bool readOnly,
            IList<string> replacedStrings, CssStyleSheetType origin)
            : base(parent, readOnly, replacedStrings, origin)
        {
        }

        #endregion

        #region Implementation of ICssRule

        /// <summary>
        ///     The type of the rule. The expectation is that binding-specific casting methods can be used to cast down from an
        ///     instance of the CSSRule interface to the specific derived interface implied by the type.
        /// </summary>
        public override CssRuleType Type => CssRuleType.UnknownRule;

        #endregion

        #region Static members

        // TODO: should also find blocks
        private static readonly Regex regex = new Regex(@"^@[^;]+;");

        internal static CssRule Parse(ref string css, object parent, bool readOnly,
            IList<string> replacedStrings, CssStyleSheetType origin)
        {
            var match = regex.Match(css);
            if (match.Success)
            {
                var rule = new CssUnknownRule(parent, readOnly, replacedStrings, origin);
                css = css.Substring(match.Length);
                return rule;
            }

            // didn't match => do nothing
            return null;
        }

        #endregion
    }
}