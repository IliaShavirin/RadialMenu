using System;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore;
using BaseProj.SharpVectors.SharpVectorCore.Utils.Dom;

namespace BaseProj.SharpVectors.SharpVectorDom
{
    /// <summary>
    ///     Summary description for Conversion.
    /// </summary>
    internal static class Conversion
    {
        internal static INode Cast(XmlNode node)
        {
            try
            {
                return (INode)node;
            }
            catch (InvalidCastException)
            {
                throw new DomException(DomExceptionType.WrongDocumentErr);
            }
        }

        internal static XmlNode Cast(INode node)
        {
            try
            {
                return (XmlNode)node;
            }
            catch (InvalidCastException)
            {
                throw new DomException(DomExceptionType.WrongDocumentErr);
            }
        }
    }
}