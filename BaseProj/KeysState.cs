using System.Windows.Input;

namespace BaseProj
{
    public static class KeysState
    {
        public static bool IsCtrlHold => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

        public static bool IsShiftHold => Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

        public static bool IsLeftMouseButtonHold => Mouse.LeftButton == MouseButtonState.Pressed;
    }
}