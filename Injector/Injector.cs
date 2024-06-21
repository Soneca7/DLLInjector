using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;

public static class DllInjector
{
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out IntPtr lpNumberOfBytesWritten);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr LoadLibrary(string lpFileName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    public enum InjectionMethod
    {
        ByProcessName,
        ByPID
    }

    public static bool InjectDll(string processIdentifier, string dllPath, InjectionMethod method = InjectionMethod.ByProcessName)
    {
        int processId;
        Process targetProcess = null;

        if (method == InjectionMethod.ByProcessName)
        {
            targetProcess = Process.GetProcessesByName(processIdentifier).FirstOrDefault();
        }
        else if (method == InjectionMethod.ByPID)
        {
            if (!int.TryParse(processIdentifier, out processId))
            {
                return false;
            }

            try
            {
                targetProcess = Process.GetProcessById(processId);
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        if (targetProcess != null)
        {
            IntPtr hProcess = OpenProcess(0x1F0FFF, false, targetProcess.Id); // 0x1F0FFF for all access

            if (hProcess != IntPtr.Zero)
            {
                IntPtr memAlloc = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)((dllPath.Length + 1) * Marshal.SizeOf(typeof(char))), 0x1000, 0x40);

                if (memAlloc != IntPtr.Zero)
                {
                    IntPtr bytesWritten;
                    byte[] buffer = Encoding.Default.GetBytes(dllPath);
                    WriteProcessMemory(hProcess, memAlloc, buffer, (uint)((dllPath.Length + 1) * Marshal.SizeOf(typeof(char))), out bytesWritten);

                    IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");

                    if (loadLibraryAddr != IntPtr.Zero)
                    {
                        IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, loadLibraryAddr, memAlloc, 0, IntPtr.Zero);

                        if (hThread != IntPtr.Zero)
                        {
                            WaitForSingleObject(hThread, 0xFFFFFFFF);
                            CloseHandle(hThread);
                            CloseHandle(hProcess);
                            return true;
                        }
                    }
                }

                CloseHandle(hProcess);
            }
        }

        return false;
    }
}
