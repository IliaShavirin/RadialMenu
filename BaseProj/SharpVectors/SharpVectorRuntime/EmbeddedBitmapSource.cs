using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BaseProj.SharpVectors.SharpVectorRuntime
{
    public class EmbeddedBitmapSource : BitmapSource
    {
        #region Public Methods

        public override void CopyPixels(Int32Rect sourceRect, IntPtr buffer,
            int bufferSize, int stride)
        {
            EnsureStream();
            base.CopyPixels(sourceRect, buffer, bufferSize, stride);
        }

        #endregion

        #region Private Fields

        private string _mimeType;

        private BitmapImage _bitmap;
        private MemoryStream _stream;

        #endregion Fields

        #region Constructors and Destructor

        public EmbeddedBitmapSource()
        {
            _mimeType = "image/png";
            //
            // Set the _useVirtuals private fields of BitmapSource to true. otherwise you will not be able to call BitmapSource methods.
            var field = typeof(BitmapSource).GetField("_useVirtuals",
                BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(this, true);
        }

        // ------------------------------------------------------------------

        public EmbeddedBitmapSource(MemoryStream stream)
            : this()
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            _stream = stream;
            //
            // Associated this class with source.
            BeginInit();

            _bitmap = new BitmapImage();

            _bitmap.BeginInit();
            _bitmap.StreamSource = _stream;
            _bitmap.EndInit();

            InitWicInfo(_bitmap);
            EndInit();
        }

        #endregion Constructors

        #region Public Properties

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EmbeddedBitmapData Data
        {
            get => new EmbeddedBitmapData(_stream);
            set
            {
                BeginInit();

                _stream = value.Stream;

                _bitmap = new BitmapImage();
                _bitmap.BeginInit();
                _bitmap.StreamSource = _stream;
                _bitmap.EndInit();

                InitWicInfo(_bitmap);
                EndInit();
            }
        }

        public override double DpiX
        {
            get
            {
                EnsureStream();
                return base.DpiX;
            }
        }

        public override double DpiY
        {
            get
            {
                EnsureStream();
                return base.DpiY;
            }
        }

        public override PixelFormat Format
        {
            get
            {
                EnsureStream();
                return base.Format;
            }
        }

        public override BitmapPalette Palette
        {
            get
            {
                EnsureStream();
                return base.Palette;
            }
        }

        public override int PixelWidth
        {
            get
            {
                EnsureStream();
                return base.PixelWidth;
            }
        }

        public override int PixelHeight
        {
            get
            {
                EnsureStream();
                return base.PixelHeight;
            }
        }

        public string MimeType
        {
            get => _mimeType;
            set
            {
                if (value != null && value.Length != 0) _mimeType = value;
            }
        }

        #endregion Properties

        #region Protected Methods

        protected override void CloneCore(Freezable sourceFreezable)
        {
            var cloneSource = (EmbeddedBitmapSource)sourceFreezable;
            CopyFrom(cloneSource);
            //base.CloneCore( sourceFreezable );
        }

        protected override void CloneCurrentValueCore(Freezable sourceFreezable)
        {
            var cloneSource = (EmbeddedBitmapSource)sourceFreezable;
            CopyFrom(cloneSource);
            //base.CloneCurrentValueCore( sourceFreezable );
        }

        protected override void GetAsFrozenCore(Freezable sourceFreezable)
        {
            var cloneSource = (EmbeddedBitmapSource)sourceFreezable;
            CopyFrom(cloneSource);
            //base.GetAsFrozenCore( sourceFreezable );
        }

        protected override void GetCurrentValueAsFrozenCore(Freezable sourceFreezable)
        {
            var cloneSource = (EmbeddedBitmapSource)sourceFreezable;
            CopyFrom(cloneSource);
            //base.GetCurrentValueAsFrozenCore( sourceFreezable );
        }

        protected override Freezable CreateInstanceCore()
        {
            return new EmbeddedBitmapSource();
        }

        #endregion Override Methods

        #region Private Methods

        /// <summary>
        ///     Call BeginInit every time the WICSourceHandle is going to be change.
        ///     again this methods is not exposed and reflection is needed.
        /// </summary>
        private void BeginInit()
        {
            var field = typeof(BitmapSource).GetField(
                "_bitmapInit", BindingFlags.NonPublic | BindingFlags.Instance);
            var beginInit = field.FieldType.GetMethod(
                "BeginInit", BindingFlags.Public | BindingFlags.Instance);
            beginInit.Invoke(field.GetValue(this), null);
        }

        /// <summary>
        ///     Call EndInit after the WICSourceHandle was changed and after using BeginInit.
        ///     again this methods is not exposed and reflection is needed.
        /// </summary>
        private void EndInit()
        {
            var field = typeof(BitmapSource).GetField(
                "_bitmapInit", BindingFlags.NonPublic | BindingFlags.Instance);
            var endInit = field.FieldType.GetMethod(
                "EndInit", BindingFlags.Public | BindingFlags.Instance);
            endInit.Invoke(field.GetValue(this), null);
        }

        /// <summary>
        ///     Set the WicSourceHandle property with the source associated with this class.
        ///     again this methods is not exposed and reflection is needed.
        /// </summary>
        /// <param name="source"></param>
        private void InitWicInfo(BitmapSource source)
        {
            //
            // Use reflection to get the private property WicSourceHandle Get and Set methods.
            var wicSourceHandle = typeof(BitmapSource).GetProperty(
                "WicSourceHandle", BindingFlags.NonPublic | BindingFlags.Instance);

            var wicSourceHandleGetMethod = wicSourceHandle.GetGetMethod(true);
            var wicSourceHandleSetMethod = wicSourceHandle.GetSetMethod(true);
            //
            // Call the Get method of the WicSourceHandle of source.
            var wicHandle = wicSourceHandleGetMethod.Invoke(source, null);
            //
            // Call the Set method of the WicSourceHandle of this with the value from source.
            wicSourceHandleSetMethod.Invoke(this, new[] { wicHandle });
        }

        private void CopyFrom(EmbeddedBitmapSource source)
        {
            BeginInit();

            _bitmap = source._bitmap;

            InitWicInfo(_bitmap);
            EndInit();
        }

        /// <summary>
        ///     In the designer Data is not set. To prevent exceptions when displaying in the Designer, add a dummy bitmap.
        /// </summary>
        private void EnsureStream()
        {
            if (_stream == null)
            {
                var dummyBitmap = Create(1, 1, 96.0, 96.0,
                    PixelFormats.Pbgra32, null, new byte[] { 0, 0, 0, 0 }, 4);
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(dummyBitmap));
                var stream = new MemoryStream();
                encoder.Save(stream);
                Data = new EmbeddedBitmapData(stream);
            }
        }

        #endregion Methods
    }
}