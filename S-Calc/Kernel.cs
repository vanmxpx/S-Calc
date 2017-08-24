using S_Calc.Common.Controls.CustomKeyboard;

namespace S_Calc
{
    public static class Kernel
    {
        public static CustomKeyboard Keyboard { get; private set; }

        public static MainActivity Activity { get; set; }
        static Kernel()
        {
            Keyboard = new CustomKeyboard();
        }
    }
}