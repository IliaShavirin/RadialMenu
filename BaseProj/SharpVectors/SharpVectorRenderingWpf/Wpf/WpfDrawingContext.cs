using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf
{
    public sealed class WpfDrawingContext : DependencyObject, IEnumerable<DrawingGroup>
    {
        #region Private Fields

        private DrawingGroup _rootDrawing;

        private WpfDrawingSettings _settings;

        private readonly Stack<DrawingGroup> _drawStack;

        private readonly HashSet<string> _registeredIds;

        #endregion

        #region Constructors and Destructor

        public WpfDrawingContext(bool isFragment)
        {
            IsFragment = isFragment;
            _drawStack = new Stack<DrawingGroup>();
            _registeredIds = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

            _settings = new WpfDrawingSettings();
        }

        public WpfDrawingContext(bool isFragment, WpfDrawingSettings settings)
            : this(isFragment)
        {
            if (settings != null) _settings = settings;
        }

        #endregion

        #region Public Properties

        // Summary:
        //     Gets the number of elements contained in the System.Collections.Generic.Stack<DrawingGroup>.
        //
        // Returns:
        //     The number of elements contained in the System.Collections.Generic.Stack<DrawingGroup>.
        public int Count
        {
            get
            {
                if (_drawStack != null) return _drawStack.Count;

                return 0;
            }
        }

        public bool RenderingClipRegion { get; set; }

        public DrawingGroup Root
        {
            get => _rootDrawing;
            internal set
            {
                if (value != null) _rootDrawing = value;
            }
        }

        public DrawingGroup Links { get; private set; }

        public bool IsFragment { get; }

        public object Tag { get; set; }

        public WpfDrawingSettings Settings
        {
            get => _settings;
            set
            {
                if (value != null) _settings = value;
            }
        }

        public WpfLinkVisitor LinkVisitor { get; set; }

        public WpfEmbeddedImageVisitor ImageVisitor { get; set; }

        public WpfFontFamilyVisitor FontFamilyVisitor { get; set; }

        #endregion

        #region Internal Properties

        internal bool OptimizePath => _settings.OptimizePath;

        internal bool TextAsGeometry => _settings.TextAsGeometry;

        internal bool IncludeRuntime => _settings.IncludeRuntime;

        internal CultureInfo CultureInfo => _settings.CultureInfo;

        internal CultureInfo EnglishCultureInfo => _settings.NeutralCultureInfo;

        internal string DefaultFontName => _settings.DefaultFontName;

        #endregion

        #region Public Methods

        // Summary:
        //     Removes all objects from the System.Collections.Generic.Stack<DrawingGroup>.
        public void Clear()
        {
            if (_drawStack != null) _drawStack.Clear();
        }

        //
        // Summary:
        //     Determines whether an element is in the System.Collections.Generic.Stack<DrawingGroup>.
        //
        // Parameters:
        //   item:
        //     The object to locate in the System.Collections.Generic.Stack<DrawingGroup>. The value
        //     can be null for reference types.
        //
        // Returns:
        //     true if item is found in the System.Collections.Generic.Stack<DrawingGroup>; otherwise,
        //     false.
        public bool Contains(DrawingGroup item)
        {
            if (_drawStack != null) return _drawStack.Contains(item);

            return false;
        }

        //
        // Summary:
        //     Copies the System.Collections.Generic.Stack<DrawingGroup> to an existing one-dimensional
        //     System.Array, starting at the specified array index.
        //
        // Parameters:
        //   array:
        //     The one-dimensional System.Array that is the destination of the elements
        //     copied from System.Collections.Generic.Stack<DrawingGroup>. The System.Array must have
        //     zero-based indexing.
        //
        //   arrayIndex:
        //     The zero-based index in array at which copying begins.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     array is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     arrayIndex is less than zero.
        //
        //   System.ArgumentException:
        //     arrayIndex is equal to or greater than the length of array.  -or- The number
        //     of elements in the source System.Collections.Generic.Stack<DrawingGroup> is greater
        //     than the available space from arrayIndex to the end of the destination array.
        public void CopyTo(DrawingGroup[] array, int arrayIndex)
        {
            if (_drawStack != null) _drawStack.CopyTo(array, arrayIndex);
        }

        //
        // Summary:
        //     Returns the object at the top of the System.Collections.Generic.Stack<DrawingGroup>
        //     without removing it.
        //
        // Returns:
        //     The object at the top of the System.Collections.Generic.Stack<DrawingGroup>.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The System.Collections.Generic.Stack<DrawingGroup> is empty.
        public DrawingGroup Peek()
        {
            if (_drawStack != null) return _drawStack.Peek();

            return null;
        }

        //
        // Summary:
        //     Removes and returns the object at the top of the System.Collections.Generic.Stack<DrawingGroup>.
        //
        // Returns:
        //     The object removed from the top of the System.Collections.Generic.Stack<DrawingGroup>.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The System.Collections.Generic.Stack<DrawingGroup> is empty.
        public DrawingGroup Pop()
        {
            if (_drawStack != null) return _drawStack.Pop();

            return null;
        }

        //
        // Summary:
        //     Inserts an object at the top of the System.Collections.Generic.Stack<DrawingGroup>.
        //
        // Parameters:
        //   item:
        //     The object to push onto the System.Collections.Generic.Stack<DrawingGroup>. The value
        //     can be null for reference types.
        public void Push(DrawingGroup item)
        {
            if (_drawStack != null && item != null) _drawStack.Push(item);
        }

        //
        // Summary:
        //     Copies the System.Collections.Generic.Stack<DrawingGroup> to a new array.
        //
        // Returns:
        //     A new array containing copies of the elements of the System.Collections.Generic.Stack<DrawingGroup>.
        public DrawingGroup[] ToArray()
        {
            if (_drawStack != null) return _drawStack.ToArray();

            return new DrawingGroup[0];
        }

        //
        // Summary:
        //     Sets the capacity to the actual number of elements in the System.Collections.Generic.Stack<DrawingGroup>,
        //     if that number is less than 90 percent of current capacity.
        public void TrimExcess()
        {
            if (_drawStack != null) _drawStack.TrimExcess();
        }

        public void Initialize(WpfLinkVisitor linkVisitor, WpfFontFamilyVisitor fontFamilyVisitor,
            WpfEmbeddedImageVisitor imageVisitor)
        {
            LinkVisitor = linkVisitor;
            FontFamilyVisitor = fontFamilyVisitor;
            ImageVisitor = imageVisitor;

            _rootDrawing = new DrawingGroup();

            Push(_rootDrawing);

            if (LinkVisitor != null && LinkVisitor.Aggregates)
            {
                Links = new DrawingGroup();

                var groupId = LinkVisitor.AggregatedLayerName;
                if (!string.IsNullOrEmpty(groupId)) Links.SetValue(FrameworkElement.NameProperty, groupId);

                linkVisitor.Initialize(Links, this);
            }
        }

        public void Uninitialize()
        {
        }

        public void BeginDrawing()
        {
        }

        public void EndDrawing()
        {
            if (_rootDrawing != null && Links != null)
                if (Links.Children.Count != 0)
                    _rootDrawing.Children.Add(Links);
        }

        public bool IsRegisteredId(string elementId)
        {
            if (string.IsNullOrEmpty(elementId)) return false;

            if (_registeredIds != null && _registeredIds.Count != 0) return _registeredIds.Contains(elementId);

            return false;
        }

        public void RegisterId(string elementId)
        {
            if (string.IsNullOrEmpty(elementId)) return;

            if (_registeredIds != null) _registeredIds.Add(elementId);
        }

        public void UnRegisterId(string elementId)
        {
            if (string.IsNullOrEmpty(elementId)) return;

            if (_registeredIds != null) _registeredIds.Remove(elementId);
        }

        #endregion

        #region IEnumerable Members

        //
        // Summary:
        //     Returns an enumerator for the System.Collections.Generic.Stack<DrawingGroup>.
        //
        // Returns:
        //     An System.Collections.Generic.Stack<DrawingGroup>.Enumerator for the System.Collections.Generic.Stack<DrawingGroup>.
        public IEnumerator<DrawingGroup> GetEnumerator()
        {
            if (_drawStack != null) return _drawStack.GetEnumerator();

            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_drawStack != null) return _drawStack.GetEnumerator();

            return null;
        }

        #endregion
    }
}