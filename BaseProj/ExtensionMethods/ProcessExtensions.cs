using System;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace BaseProj.ExtensionMethods
{
    public static class ProcessExtensions
    {
        public static string GetProcessExecutableFullPath(this Process proc)
        {
            var path = "";
            try
            {
                var Query = "SELECT ExecutablePath FROM Win32_Process WHERE ProcessId = " + proc.Id;

                using (var mos = new ManagementObjectSearcher(Query))
                {
                    using (var moc = mos.Get())
                    {
                        var ExecutablePath = (from mo in moc.Cast<ManagementObject>() select mo["ExecutablePath"])
                            .First().ToString();

                        path = ExecutablePath;
                    }
                }
            }
            catch (Exception)
            {
            }

            return path;
        }
    }
}