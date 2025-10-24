using System;

namespace LoveMachinePrototyper.NonPortable
{
    internal static class GameArchitecture
    {
        public static string Arch => IntPtr.Size == 4 ? "x86" : "x64";
    }
}
