using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace BaseProj.SharpVectors.SharpVectorCss.Css
{
    #region Public enums

    #endregion

    public sealed class CssXPathSelector
    {
        #region Static Fields

        internal static Regex reSelector = new Regex(CssStyleRule.sSelector);

        #endregion

        #region Internal Fields

        internal XPathSelectorStatus Status = XPathSelectorStatus.Start;
        internal string CssSelector;

        #endregion

        #region Private Fields

        private int _specificity;
        private string sXpath;
        private XPathExpression xpath;
        private readonly IDictionary<string, string> _nsTable;

        #endregion

        #region Constructors and Destructor

        public CssXPathSelector(string selector)
            : this(selector, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase))
        {
        }

        public CssXPathSelector(string selector, IDictionary<string, string> namespaceTable)
        {
            CssSelector = selector.Trim();
            _nsTable = namespaceTable;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Only used for testing!
        /// </summary>
        public string XPath
        {
            get
            {
                if (Status == XPathSelectorStatus.Start) GetXPath(null);
                return sXpath;
            }
        }

        public int Specificity
        {
            get
            {
                if (Status == XPathSelectorStatus.Start) GetXPath(null);
                if (Status != XPathSelectorStatus.Error)
                    return _specificity;
                return 0;
            }
        }

        #endregion

        #region Private Methods

        private void AddSpecificity(int a, int b, int c)
        {
            _specificity += a * 100 + b * 10 + c;
        }

        private string NsToXPath(Match match)
        {
            var r = string.Empty;
            var g = match.Groups["ns"];

            if (g != null && g.Success)
            {
                var prefix = g.Value.TrimEnd('|');

                if (prefix.Length == 0)
                {
                    // a element in no namespace
                    r += "[namespace-uri()='']";
                }
                else if (prefix == "*")
                {
                    // do nothing, any or no namespace is okey
                }
                else if (_nsTable.ContainsKey(prefix))
                {
                    r += "[namespace-uri()='" + _nsTable[prefix] + "']";
                }
                else
                {
                    //undeclared namespace => invalid CSS selector
                    r += "[false]";
                }
            }
            else if (_nsTable.ContainsKey(string.Empty))
            {
                //if no default namespace has been specified, this is equivalent to *|E. Otherwise it is equivalent to ns|E where ns is the default namespace.

                r += "[namespace-uri()='" + _nsTable[string.Empty] + "']";
            }

            return r;
        }

        private string TypeToXPath(Match match)
        {
            var r = string.Empty;
            var g = match.Groups["type"];
            var s = g.Value;
            if (!g.Success || s == "*")
            {
                r = string.Empty;
            }
            else
            {
                r = "[local-name()='" + s + "']";
                AddSpecificity(0, 0, 1);
            }

            return r;
        }

        private string ClassToXPath(Match match)
        {
            var r = string.Empty;
            var g = match.Groups["class"];

            foreach (Capture c in g.Captures)
            {
                r += "[contains(concat(' ',@class,' '),' " + c.Value.Substring(1) + " ')]";
                AddSpecificity(0, 1, 0);
            }

            return r;
        }

        private string IdToXPath(Match match)
        {
            var r = string.Empty;
            var g = match.Groups["id"];
            if (g.Success)
            {
                // r = "[id('" + g.Value.Substring(1) + "')]";
                r = "[@id='" + g.Value.Substring(1) + "']";
                AddSpecificity(1, 0, 0);
            }

            return r;
        }

        private string GetAttributeMatch(string attSelector)
        {
            var fullAttName = attSelector.Trim();
            var pipePos = fullAttName.IndexOf("|");
            var attMatch = string.Empty;

            if (pipePos == -1 || pipePos == 0)
            {
                // att or |att => should be in the undeclared namespace
                var attName = fullAttName.Substring(pipePos + 1);
                attMatch = "@" + attName;
            }
            else if (fullAttName.StartsWith("*|"))
            {
                // *|att => in any namespace (undeclared or declared)
                attMatch = "@*[local-name()='" + fullAttName.Substring(2) + "']";
            }
            else
            {
                // ns|att => must macht a declared namespace
                var ns = fullAttName.Substring(0, pipePos);
                var attName = fullAttName.Substring(pipePos + 1);
                if (_nsTable.ContainsKey(ns))
                    attMatch = "@" + ns + ":" + attName;
                else
                    // undeclared namespace => selector should fail
                    attMatch = "false";
            }

            return attMatch;
        }

        private string PredicatesToXPath(Match match)
        {
            var r = string.Empty;
            var g = match.Groups["attributecheck"];

            foreach (Capture c in g.Captures)
            {
                r += "[" + GetAttributeMatch(c.Value) + "]";
                AddSpecificity(0, 1, 0);
            }

            g = match.Groups["attributevaluecheck"];
            var reAttributeValueCheck = new Regex("^" + CssStyleRule.attributeValueCheck + "?$");


            foreach (Capture c in g.Captures)
            {
                var valueCheckMatch = reAttributeValueCheck.Match(c.Value);

                var attName = valueCheckMatch.Groups["attname"].Value;
                var attMatch = GetAttributeMatch(attName);
                var eq = valueCheckMatch.Groups["eqtype"].Value; // ~,^,$,*,|,nothing
                var attValue = valueCheckMatch.Groups["attvalue"].Value;

                switch (eq)
                {
                    case "":
                        // [foo="bar"] => [@foo='bar']
                        r += "[" + attMatch + "='" + attValue + "']";
                        break;
                    case "~":
                        // [foo~="bar"] 
                        // an E element whose "foo" attribute value is a list of space-separated values, one of which is exactly equal to "bar"
                        r += "[contains(concat(' '," + attMatch + ",' '),' " + attValue + " ')]";
                        break;
                    case "^":
                        // [foo^="bar"]  
                        // an E element whose "foo" attribute value begins exactly with the string "bar"
                        r += "[starts-with(" + attMatch + ",'" + attValue + "')]";
                        break;
                    case "$":
                        // [foo$="bar"]  
                        // an E element whose "foo" attribute value ends exactly with the string "bar"
                        var a = attValue.Length - 1;

                        r += "[substring(" + attMatch + ",string-length(" + attMatch + ")-" + a + ")='" + attValue +
                             "']";
                        break;
                    case "*":
                        // [foo*="bar"]  
                        // an E element whose "foo" attribute value contains the substring "bar"
                        r += "[contains(" + attMatch + ",'" + attValue + "')]";
                        break;
                    case "|":
                        // [hreflang|="en"]  
                        // an E element whose "hreflang" attribute has a hyphen-separated list of values beginning (from the left) with "en"
                        r += "[" + attMatch + "='" + attValue + "' or starts-with(" + attMatch + ",'" + attValue +
                             "-')]";
                        break;
                }

                AddSpecificity(0, 1, 0);
            }

            return r;
        }

        private string PseudoClassesToXPath(Match match, XPathNavigator nav)
        {
            var specificityA = 0;
            var specificityB = 1;
            var specificityC = 0;
            var r = string.Empty;
            var g = match.Groups["pseudoclass"];

            foreach (Capture c in g.Captures)
            {
                var reLang = new Regex(@"^lang\(([A-Za-z\-]+)\)$");
                var reContains = new Regex("^contains\\((\"|\')?(?<stringvalue>.*?)(\"|\')?\\)$");

                var s = @"^(?<type>(nth-child)|(nth-last-child)|(nth-of-type)|(nth-last-of-type))\(\s*";
                s += @"(?<exp>(odd)|(even)|(((?<a>[\+-]?\d*)n)?(?<b>[\+-]?\d+)?))";
                s += @"\s*\)$";
                var reNth = new Regex(s);

                var p = c.Value.Substring(1);

                if (p == "root")
                {
                    r += "[not(parent::*)]";
                }
                else if (p.StartsWith("not"))
                {
                    var expr = p.Substring(4, p.Length - 5);
                    var sel = new CssXPathSelector(expr, _nsTable);

                    var xpath = sel.XPath;
                    if (xpath != null && xpath.Length > 3)
                    {
                        // remove *[ and ending ]
                        xpath = xpath.Substring(2, xpath.Length - 3);

                        r += "[not(" + xpath + ")]";

                        var specificity = sel.Specificity;

                        // specificity = 123
                        specificityA = (int)Math.Floor((double)specificity / 100);
                        specificity -= specificityA * 100;
                        // specificity = 23
                        specificityB = (int)Math.Floor((double)specificity / 10);

                        specificity -= specificityB * 10;
                        // specificity = 3
                        specificityC = specificity;
                    }
                }
                else if (p == "first-child")
                {
                    r += "[count(preceding-sibling::*)=0]";
                }
                else if (p == "last-child")
                {
                    r += "[count(following-sibling::*)=0]";
                }
                else if (p == "only-child")
                {
                    r += "[count(../*)=1]";
                }
                else if (p == "only-of-type")
                {
                    r += "[false]";
                }
                else if (p == "empty")
                {
                    r += "[not(child::*) and not(text())]";
                }
                else if (p == "target")
                {
                    r += "[false]";
                }
                else if (p == "first-of-type")
                {
                    r += "[false]";
                    //r += "[.=(../*[local-name='roffe'][position()=1])]";
                }
                else if (reLang.IsMatch(p))
                {
                    r += "[lang('" + reLang.Match(p).Groups[1].Value + "')]";
                }
                else if (reContains.IsMatch(p))
                {
                    r += "[contains(string(.),'" + reContains.Match(p).Groups["stringvalue"].Value + "')]";
                }
                else if (reNth.IsMatch(p))
                {
                    var m = reNth.Match(p);
                    var type = m.Groups["type"].Value;
                    var exp = m.Groups["exp"].Value;
                    var a = 0;
                    var b = 0;
                    if (exp == "odd")
                    {
                        a = 2;
                        b = 1;
                    }
                    else if (exp == "even")
                    {
                        a = 2;
                        b = 0;
                    }
                    else
                    {
                        var v = m.Groups["a"].Value;

                        if (v.Length == 0) a = 1;
                        else if (v.Equals("-")) a = -1;
                        else a = int.Parse(v);

                        if (m.Groups["b"].Success) b = int.Parse(m.Groups["b"].Value);
                    }


                    if (type.Equals("nth-child") || type.Equals("nth-last-child"))
                    {
                        string axis;
                        if (type.Equals("nth-child")) axis = "preceding-sibling";
                        else axis = "following-sibling";

                        if (a == 0)
                            r += "[count(" + axis + "::*)+1=" + b + "]";
                        else
                            r += "[((count(" + axis + "::*)+1-" + b + ") mod " + a + "=0)and((count(" + axis +
                                 "::*)+1-" + b + ") div " + a + ">=0)]";
                    }
                }

                AddSpecificity(specificityA, specificityB, specificityC);
            }

            return r;
        }

        private void SeperatorToXPath(Match match, StringBuilder xpath, string cur)
        {
            var g = match.Groups["seperator"];
            if (g.Success)
            {
                var s = g.Value.Trim();
                if (s.Length == 0)
                {
                    cur += "//*";
                }
                else if (s == ">")
                {
                    cur += "/*";
                }
                else if (s == "+" || s == "~")
                {
                    xpath.Append("[preceding-sibling::*");
                    if (s == "+") xpath.Append("[position()=1]");
                    xpath.Append(cur);
                    xpath.Append("]");
                    cur = string.Empty;
                }
            }

            xpath.Append(cur);
        }

        #endregion

        #region Internal Methods

        internal void GetXPath(XPathNavigator nav)
        {
            _specificity = 0;
            var xpath = new StringBuilder("*");

            var match = reSelector.Match(CssSelector);
            while (match.Success)
            {
                if (match.Success && match.Value.Length > 0)
                {
                    var x = string.Empty;
                    x += NsToXPath(match);
                    x += TypeToXPath(match);
                    x += ClassToXPath(match);
                    x += IdToXPath(match);
                    x += PredicatesToXPath(match);
                    x += PseudoClassesToXPath(match, nav);
                    SeperatorToXPath(match, xpath, x);
                }

                match = match.NextMatch();
            }

            if (nav != null) Status = XPathSelectorStatus.Parsed;
            sXpath = xpath.ToString();
        }

        private XmlNamespaceManager GetNSManager()
        {
            var nsman = new XmlNamespaceManager(new NameTable());

            foreach (var dicEnum in _nsTable) nsman.AddNamespace(dicEnum.Key, dicEnum.Value);
            //IDictionaryEnumerator dicEnum = _nsTable.GetEnumerator();
            //while(dicEnum.MoveNext())
            //{
            //    nsman.AddNamespace((string)dicEnum.Key, (string)dicEnum.Value);
            //}

            return nsman;
        }

        internal void Compile(XPathNavigator nav)
        {
            if (Status == XPathSelectorStatus.Start) GetXPath(nav);
            if (Status == XPathSelectorStatus.Parsed)
            {
                xpath = nav.Compile(sXpath);
                xpath.SetContext(GetNSManager());

                Status = XPathSelectorStatus.Compiled;
            }
        }

        public bool Matches(XPathNavigator nav)
        {
            if (Status != XPathSelectorStatus.Compiled) Compile(nav);
            if (Status == XPathSelectorStatus.Compiled)
                try
                {
                    return nav.Matches(xpath);
                }
                catch
                {
                    return false;
                }

            return false;
        }

        #endregion
    }
}