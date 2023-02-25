using System.IO;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Stylesheets;
using BaseProj.SharpVectors.SharpVectorCore.Svg.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg
{
    public interface ISvgWindow
    {
        IStyleSheet DefaultStyleSheet { get; }
        ISvgDocument Document { get; }
        long InnerHeight { get; }
        long InnerWidth { get; }
        string Source { get; set; }
        ISvgRenderer Renderer { get; }
        DirectoryInfo WorkingDir { get; }
        XmlDocumentFragment ParseXML(string source, XmlDocument document);
        string PrintNode(XmlNode node);

        void Alert(string message);
        /*void GetURL (string uri, EventListener callback);	*/
        /*void PostURL (string uri, string data, EventListener callback, string mimeType, string contentEncoding);*/
    }
}