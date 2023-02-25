using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace BaseProj.SharpVectors.SharpVectorCore.Utils.Net
{
    /// <summary>
    ///     Summary description for DataWebResponse.
    /// </summary>
    /// <remarks>According to http://www.ietf.org/rfc/rfc2397.txt</remarks>
    [Serializable]
    public sealed class DataWebResponse : WebResponse
    {
        private static Regex re = new Regex(@"^data:(?<mediatype>.*?),(?<data>.*)$", RegexOptions.Singleline);
        private static Regex wsRemover = new Regex(@"\s", RegexOptions.Singleline);
        private static Regex charsetFinder = new Regex(@"charset=(?<charset>[^;]+)", RegexOptions.Singleline);
        private string contentType;

        private byte[] decodedData;

        internal DataWebResponse(Uri uri)
        {
            ResponseUri = uri;

            var fullUri = HttpUtility.UrlDecode(uri.AbsoluteUri);
            fullUri = fullUri.Replace(' ', '+');

            // remove all whitespace
            fullUri = contentType = wsRemover.Replace(fullUri, "");

            var match = re.Match(fullUri);

            if (match.Success)
            {
                contentType = match.Groups["mediatype"].Value;

                var data = match.Groups["data"].Value;

                if (contentType.Length == 0)
                {
                    contentType = "text/plain;charset=US-ASCII";
                }
                else if (contentType.StartsWith(";"))
                {
                    if (contentType.IndexOf(";charset=") > 0)
                        contentType = "text/plain" + contentType;
                    else
                        throw new Exception("Malformed data URI");
                }

                if (contentType.EndsWith(";base64", StringComparison.OrdinalIgnoreCase))
                {
                    contentType = contentType.Remove(contentType.Length - 7, 7);
                    decodedData = Convert.FromBase64String(data);
                }
                else
                {
                    var charsetMatch = charsetFinder.Match(contentType);
                    if (charsetMatch.Success && charsetMatch.Groups["charset"].Success)
                        try
                        {
                            ContentEncoding = Encoding.GetEncoding(charsetMatch.Groups["charset"].Value);
                        }
                        catch (NotSupportedException)
                        {
                            ContentEncoding = Encoding.ASCII;
                        }

                    decodedData = HttpUtility.UrlDecodeToBytes(data);
                }
            }
            else
            {
                throw new Exception("Malformed data URI");
            }
        }

        public override long ContentLength => decodedData.Length;

        public Encoding ContentEncoding { get; } = Encoding.ASCII;

        public override string ContentType => contentType;

        public override Uri ResponseUri { get; }

        public override Stream GetResponseStream()
        {
            MemoryStream ms;
            ms = new MemoryStream(decodedData, false);
            ms.Position = 0;
            return ms;
        }
    }
}