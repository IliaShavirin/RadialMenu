// This is based on MSDN Magazine article codes by Stephen Toub (stoub@microsoft.com)
// The article is http://msdn.microsoft.com/en-us/magazine/cc163696.aspx

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace BaseProj.SharpVectors.SharpVectorConverters.Utils
{
    public static class DirectoryUtils
    {
        private const int ERROR_FILE_NOT_FOUND = 0x2;
        private const int ERROR_ACCESS_DENIED = 0x5;
        private const int ERROR_NO_MORE_FILES = 0x12;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern SafeFindHandle FindFirstFile(string lpFileName,
            [In] [Out] [MarshalAs(UnmanagedType.LPStruct)]
            WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern bool FindNextFile(SafeFindHandle hndFindFile,
            [In] [Out] [MarshalAs(UnmanagedType.LPStruct)]
            WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern ErrorModes SetErrorMode(ErrorModes newMode);

        public static void DeleteDirectory(string directoryPath, bool recursive)
        {
            if (string.IsNullOrEmpty(directoryPath)) return;

            var dirInfo = new DirectoryInfo(directoryPath);
            if (dirInfo.Exists)
                // It is a directory...
                try
                {
                    dirInfo.Attributes = FileAttributes.Normal;
                    dirInfo.Delete(recursive);
                }
                catch (UnauthorizedAccessException)
                {
                    // One possible cause of this is read-only file, so first
                    // try another method of deleting the directory...
                    foreach (var file in FindFiles(dirInfo, "*.*",
                                 recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                        File.Delete(file);
                    }

                    dirInfo.Delete(recursive);
                }
        }

        public static IEnumerable<string> FindFiles(DirectoryInfo dir,
            string pattern, SearchOption searchOption)
        {
            // We suppressed this demand for each p/invoke call, so demand it upfront once
            new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();

            // Validate parameters
            if (dir == null)
                throw new ArgumentNullException("dir");
            if (pattern == null)
                throw new ArgumentNullException("pattern");

            // Setup
            var findData = new WIN32_FIND_DATA();
            var directories = new Stack<DirectoryInfo>();
            directories.Push(dir);

            // Process each directory
            var origErrorMode = SetErrorMode(ErrorModes.FailCriticalErrors);
            try
            {
                while (directories.Count > 0)
                {
                    // Get the name of the next directory and the corresponding search pattern
                    dir = directories.Pop();
                    var dirPath = dir.FullName.Trim();
                    if (dirPath.Length == 0)
                        continue;
                    var lastChar = dirPath[dirPath.Length - 1];
                    if (lastChar != Path.DirectorySeparatorChar &&
                        lastChar != Path.AltDirectorySeparatorChar)
                        dirPath += Path.DirectorySeparatorChar;

                    // Process all files in that directory
                    var handle = FindFirstFile(dirPath + pattern, findData);
                    if (handle.IsInvalid)
                    {
                        var error = Marshal.GetLastWin32Error();
                        if (error == ERROR_ACCESS_DENIED ||
                            error == ERROR_FILE_NOT_FOUND)
                            continue;
                        throw new Win32Exception(error);
                    }
                    else
                    {
                        try
                        {
                            do
                            {
                                if ((findData.dwFileAttributes &
                                     FileAttributes.Directory) == 0)
                                    yield return dirPath + findData.cFileName;
                            } while (FindNextFile(handle, findData));

                            var error = Marshal.GetLastWin32Error();
                            if (error != ERROR_NO_MORE_FILES) throw new Win32Exception(error);
                        }
                        finally
                        {
                            handle.Dispose();
                        }
                    }

                    // Add all child directories if that's what the user wants
                    if (searchOption == SearchOption.AllDirectories)
                        foreach (var childDir in dir.GetDirectories())
                            if ((File.GetAttributes(childDir.FullName) &
                                 FileAttributes.ReparsePoint) == 0)
                                directories.Push(childDir);
                }
            }
            finally
            {
                SetErrorMode(origErrorMode);
            }
        }

        private sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
            private SafeFindHandle() : base(true)
            {
            }

            protected override bool ReleaseHandle()
            {
                return FindClose(handle);
            }

            [DllImport("kernel32.dll")]
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [SuppressUnmanagedCodeSecurity]
            private static extern bool FindClose(IntPtr handle);
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        [BestFitMapping(false)]
        private class WIN32_FIND_DATA
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;

            public FileAttributes dwFileAttributes;
            public int dwReserved0;
            public int dwReserved1;
            public FILETIME ftCreationTime;
            public FILETIME ftLastAccessTime;
            public FILETIME ftLastWriteTime;
            public int nFileSizeHigh;
            public int nFileSizeLow;
        }

        [Flags]
        private enum ErrorModes
        {
            /// <summary>Use the system default, which is to display all error dialog boxes.</summary>
            Default = 0x0,

            /// <summary>
            ///     The system does not display the critical-error-handler message box.
            ///     Instead, the system sends the error to the calling process.
            /// </summary>
            FailCriticalErrors = 0x1,

            /// <summary>
            ///     64-bit Windows:  The system automatically fixes memory alignment faults and makes them
            ///     invisible to the application. It does this for the calling process and any descendant processes.
            ///     After this value is set for a process, subsequent attempts to clear the value are ignored.
            /// </summary>
            NoGpFaultErrorBox = 0x2,

            /// <summary>
            ///     The system does not display the general-protection-fault message box.
            ///     This flag should only be set by debugging applications that handle general
            ///     protection (GP) faults themselves with an exception handler.
            /// </summary>
            NoAlignmentFaultExcept = 0x4,

            /// <summary>
            ///     The system does not display a message box when it fails to find a file.
            ///     Instead, the error is returned to the calling process.
            /// </summary>
            NoOpenFileErrorBox = 0x8000
        }
    }
}