using System;
using System.Runtime.InteropServices;

namespace ConsoleColorRGB
{
    public class RGBConsole
    {
        private const int STD_OUTPUT_HANDLE = -11;

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool SetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbiex);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool GetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbiex);

        [StructLayout(LayoutKind.Sequential)]
        private struct COLORREF
        {
            public uint ColorDWORD;
            public COLORREF(uint r, uint g, uint b)
            {
                ColorDWORD = r | (g << 8) | (b << 16);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CONSOLE_SCREEN_BUFFER_INFO_EX
        {
            public int cbSize;
            public COORD dwSize;
            public COORD dwCursorPosition;
            public short wAttributes;
            public SMALL_RECT srWindow;
            public COORD dwMaximumWindowSize;
            public ushort wPopupAttributes;
            public bool bFullscreenSupported;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public COLORREF[] ColorTable;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct COORD
        {
            public short X;
            public short Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SMALL_RECT
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        public static void SetConsoleColor(int index, byte r, byte g, byte b)
        {
            if (index < 0 || index > 15) throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and 15.");

            IntPtr hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
            if (hConsole == IntPtr.Zero) throw new InvalidOperationException("Failed to get console handle.");

            CONSOLE_SCREEN_BUFFER_INFO_EX csbiex = new CONSOLE_SCREEN_BUFFER_INFO_EX();
            csbiex.cbSize = Marshal.SizeOf(typeof(CONSOLE_SCREEN_BUFFER_INFO_EX));

            if (!GetConsoleScreenBufferInfoEx(hConsole, ref csbiex)) throw new InvalidOperationException("Failed to get console buffer info.");

            csbiex.ColorTable[index] = new COLORREF(r, g, b);

            if (!SetConsoleScreenBufferInfoEx(hConsole, ref csbiex)) throw new InvalidOperationException("Failed to set console color.");
        }
    }
}
