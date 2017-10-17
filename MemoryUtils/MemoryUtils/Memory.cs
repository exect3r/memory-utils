using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace MemoryUtils
{
    public static class Memory {

        [Flags]
        public enum ProcessAccessFlags : uint {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess,
            int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress,
            byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        public static IntPtr Handle { get; set; }
        public static IntPtr BaseAddress { get; set; }

        public static void Attach(string name) {
            Process[] ps = Process.GetProcessesByName(name);
            if (ps.Length == 0) throw new Exception("Process not found");

            Process p = ps[0];
            Handle = OpenProcess(ProcessAccessFlags.All, false, p.Id);
            BaseAddress = p.MainModule.EntryPointAddress;
        }

        public static byte[] Get(int address, int byteCount) {
            if (Handle == null) throw new Exception("No process attached.");
            byte[] buffer = new byte[byteCount];
            int count = 0;

            ReadProcessMemory((int)Handle, address, buffer, byteCount, ref count);
            return buffer;
        }

        public static int GetAddress(int staticAddress, params int[] offsets) {
            if (Handle == null) throw new Exception("No process attached.");
            if (offsets.Length == 0) return staticAddress;

            int address = BitConverter.ToInt32(Get(staticAddress, 4), 0);
            for (int i = 0; i < offsets.Length - 1; i++)
                address = BitConverter.ToInt32(Get(address + offsets[i], 4), 0);

            return address + offsets.Last();
        }


        public static void Set(byte[] data, params int[] ads) {
            if (Handle == null) throw new Exception("No process attached.");
            int c = 0;
            foreach (int address in ads)
                WriteProcessMemory((int)Handle, address, data, data.Length, ref c);
        }
    }
}
