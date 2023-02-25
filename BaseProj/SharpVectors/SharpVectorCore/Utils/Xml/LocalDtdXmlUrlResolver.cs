using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace BaseProj.SharpVectors.SharpVectorCore.Utils.Xml
{
    /// <summary>
    ///     Used to redirect external DTDs to local copies.
    /// </summary>
    public sealed class LocalDtdXmlUrlResolver : XmlUrlResolver
    {
        private readonly Dictionary<string, string> knownDtds;

        public LocalDtdXmlUrlResolver()
        {
            knownDtds = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public void AddDtd(string publicIdentifier, string localFile)
        {
            knownDtds.Add(publicIdentifier, localFile);
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (absoluteUri != null && knownDtds.ContainsKey(absoluteUri.AbsoluteUri))
                // ignore the known DTDs
                return Stream.Null;
            if (absoluteUri == null)
                // ignore null URIs
                return Stream.Null;
            return base.GetEntity(absoluteUri, role, ofObjectToReturn);
        }

        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            if (relativeUri.StartsWith("#"))
                return null;
            if (relativeUri.IndexOf("-//") > -1)
                return null;
            return base.ResolveUri(baseUri, relativeUri);
        }
    }
}