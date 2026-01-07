using System;
using System.Runtime.InteropServices;

namespace PFG_BackEnd.Helper;

public class RawPrinterHelper
{
     [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    private class DOCINFOA
    {
        [MarshalAs(UnmanagedType.LPStr)] public string pDocName = "OpenDrawer";
        [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile = null;
        [MarshalAs(UnmanagedType.LPStr)] public string pDataType = "RAW";
    }

    [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true)]
    private static extern bool OpenPrinter(string pPrinterName, out IntPtr phPrinter, IntPtr pDefault);

    [DllImport("winspool.Drv", SetLastError = true)]
    private static extern bool ClosePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true)]
    private static extern bool StartDocPrinter(IntPtr hPrinter, int level, [In] DOCINFOA di);

    [DllImport("winspool.Drv", SetLastError = true)]
    private static extern bool EndDocPrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", SetLastError = true)]
    private static extern bool StartPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", SetLastError = true)]
    private static extern bool EndPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", SetLastError = true)]
    private static extern bool WritePrinter(IntPtr hPrinter, byte[] pBytes, int dwCount, out int dwWritten);

    public static void SendBytes(string printerName, byte[] bytes)
    {
        if (!OpenPrinter(printerName, out var hPrinter, IntPtr.Zero))
            throw new InvalidOperationException($"No puc obrir la impressora '{printerName}'. Error: {Marshal.GetLastWin32Error()}");

        try
        {
            var di = new DOCINFOA();
            if (!StartDocPrinter(hPrinter, 1, di))
                throw new InvalidOperationException($"StartDocPrinter ha fallat. Error: {Marshal.GetLastWin32Error()}");

            try
            {
                if (!StartPagePrinter(hPrinter))
                    throw new InvalidOperationException($"StartPagePrinter ha fallat. Error: {Marshal.GetLastWin32Error()}");

                try
                {
                    if (!WritePrinter(hPrinter, bytes, bytes.Length, out var written) || written != bytes.Length)
                        throw new InvalidOperationException($"WritePrinter ha fallat. Error: {Marshal.GetLastWin32Error()}");
                }
                finally { EndPagePrinter(hPrinter); }
            }
            finally { EndDocPrinter(hPrinter); }
        }
        finally { ClosePrinter(hPrinter); }
    }
}
