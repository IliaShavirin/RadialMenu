// <developer>kevin@kevlindev.com</developer>
// <completed>0</completed>

using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg
{
    /// <summary>
    ///     Defines the interface required by a renderer to render the SVG DOM.
    /// </summary>
    /// <remarks>
    ///     The <see cref="ISvgRenderer">ISvgRenderer</see> is used to render
    ///     a <see cref="SvgElement">SvgElement</see> object onto a bitmap.
    ///     During the rendering process, it will also generate
    ///     <see cref="RenderingNode">RenderingNode</see> objects for each
    ///     <see cref="XmlElement">XmlElement</see> object in the DOM tree to
    ///     assist in the rendering.
    /// </remarks>
    public interface ISvgRenderer
    {
        /// <summary>
        ///     The window that is being rendered to.
        /// </summary>
        ISvgWindow Window { get; set; }

        /// <summary>
        ///     Controls the rendering of the document.
        /// </summary>
        SvgRectF InvalidRect { get; set; }

        /// <summary>
        ///     Event Delegate to report when the SVG renderer does it's work.
        /// </summary>
        RenderEvent OnRender { get; set; }

        /// <summary>
        ///     Renders an <see cref="SvgElement">SvgElement</see> object onto a
        ///     bitmap and returns that bitmap.
        /// </summary>
        /// <param name="node">
        ///     The SvgElement object to be rendered.
        /// </param>
        /// <returns>
        ///     A bitmap with <c>node</c> rendered onto it.
        /// </returns>
        void Render(ISvgElement node);

        /// <summary>
        ///     Renders an <see cref="SvgDocument">SvgDocument</see> object onto
        ///     a bitmap and returns that bitmap.
        /// </summary>
        /// <param name="node">
        ///     The SvgDocument object to be rendered.
        /// </param>
        /// <returns>
        ///     A bitmap with <c>node</c> rendered onto it.
        /// </returns>
        void Render(ISvgDocument node);

        /// <summary>
        ///     Allows you to establish or add to the existing invalidation rectangle
        /// </summary>
        /// <param name="rect"></param>
        void InvalidateRect(SvgRectF rect);

        ISvgRect GetRenderedBounds(ISvgElement element, float margin);
    }
}