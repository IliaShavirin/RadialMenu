// <developer>kevin@kevlindev.com</developer>

using System.Collections;
using System.Collections.Generic;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    /// <summary>
    ///     A class to encapsulate all SvgTest functionality.  Used by SVG elements as a helper class
    /// </summary>
    public sealed class SvgTests : ISvgTests
    {
        #region Constructor and Destructor

        public SvgTests(SvgElement ownerElement)
        {
            this.ownerElement = ownerElement;
            this.ownerElement.attributeChangeHandler += AttributeChange;
        }

        #endregion

        #region Private Methods

        private void AttributeChange(object src, XmlNodeChangedEventArgs args)
        {
            var attribute = src as XmlAttribute;

            if (attribute.NamespaceURI.Length == 0)
                switch (attribute.LocalName)
                {
                    case "requiredFeatures":
                        requiredFeatures = null;
                        break;
                    case "requiredExtensions":
                        requiredExtensions = null;
                        break;
                    case "systemLanguage":
                        systemLanguage = null;
                        break;
                }
        }

        #endregion

        #region SvgReadOnlyStringList Class

        private sealed class SvgReadOnlyStringList : ISvgStringList
        {
            #region IEnumerable<string> Members

            public IEnumerator<string> GetEnumerator()
            {
                return new List<string>().GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new List<string>().GetEnumerator();
            }

            #endregion

            #region ISvgStringList Members

            public uint NumberOfItems => 0;

            public void Clear()
            {
            }

            public string Initialize(string newItem)
            {
                return null;
            }

            public string GetItem(uint index)
            {
                return null;
            }

            public string InsertItemBefore(string newItem, uint index)
            {
                return null;
            }

            public string ReplaceItem(string newItem, uint index)
            {
                return null;
            }

            public string RemoveItem(uint index)
            {
                return null;
            }

            public string AppendItem(string newItem)
            {
                return null;
            }

            public void FromString(string listString)
            {
            }

            #endregion
        }

        #endregion

        #region Private fields

        private static readonly SvgReadOnlyStringList ReadOnlyStringList = new SvgReadOnlyStringList();

        private readonly SvgElement ownerElement;
        private ISvgStringList requiredFeatures;
        private ISvgStringList requiredExtensions;
        private ISvgStringList systemLanguage;

        #endregion

        #region ISvgTest Members

        public ISvgStringList RequiredFeatures
        {
            get
            {
                if (requiredFeatures == null)
                {
                    if (ownerElement.HasAttribute("requiredFeatures"))
                        requiredFeatures = new SvgStringList(ownerElement.GetAttribute("requiredFeatures"));
                    else
                        requiredFeatures = ReadOnlyStringList;
                }

                return requiredFeatures;
            }
        }


        public ISvgStringList RequiredExtensions
        {
            get
            {
                if (requiredExtensions == null)
                {
                    if (ownerElement.HasAttribute("requiredExtensions"))
                        requiredExtensions = new SvgStringList(ownerElement.GetAttribute("requiredExtensions"));
                    else
                        requiredExtensions = ReadOnlyStringList;
                }

                return requiredExtensions;
            }
        }

        public ISvgStringList SystemLanguage
        {
            get
            {
                if (systemLanguage == null)
                {
                    if (ownerElement.HasAttribute("systemLanguage"))
                        systemLanguage = new SvgStringList(ownerElement.GetAttribute("systemLanguage"));
                    else
                        systemLanguage = ReadOnlyStringList;
                }

                return systemLanguage;
            }
        }

        public bool HasExtension(string extension)
        {
            var result = false;

            for (uint i = 0; i < RequiredExtensions.NumberOfItems; i++)
                if (RequiredExtensions.GetItem(i) == extension)
                {
                    result = true;
                    break;
                }

            return result;
        }

        #endregion
    }
}