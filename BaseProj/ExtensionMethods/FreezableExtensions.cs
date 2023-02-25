using System.Windows;

namespace BaseProj.ExtensionMethods
{
    public static class FreezableExtensions
    {
        public static void TryFreeze(this Freezable freezable)
        {
            if (freezable.CanFreeze)
                freezable.Freeze();
        }
    }
}