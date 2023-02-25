using System.IO;
using System.Windows.Media.Imaging;

namespace BaseProj.ExtensionMethods
{
    public static class BitmapSourceExtensions
    {
        public static MemoryStream Encode(this BitmapSource bitmapSource, BitmapEncoder bitmapEncoder)
        {
            // Create a 'frame' for the BitmapSource, then add it to the encoder
            var bitmapFrame = BitmapFrame.Create(bitmapSource);
            bitmapEncoder.Frames.Add(bitmapFrame);

            // Prepare a memory stream to receive the encoded data, then 'save' into it
            var memoryStream = new MemoryStream();
            bitmapEncoder.Save(memoryStream);

            // Return the results of the stream as a byte array
            return memoryStream;
        }
    }
}