using System;
using System.Runtime.InteropServices;

namespace BaseProj.ExtensionMethods
{
    public static class SizeInBytesExtensions
    {
        public static int SizeInBytes(this Array instance)
        {
            return Marshal.SizeOf(instance.GetType().GetElementType()) * instance.Length;
        }

        public static int SizeInBytes<T>(this Array instance)
        {
            return Marshal.SizeOf<T>() * instance.Length;
        }

        public static int SizeInBytes(this object instance)
        {
            return Marshal.SizeOf(instance.GetType());
        }
    }
}