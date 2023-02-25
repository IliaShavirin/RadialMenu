using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Utils
{
    public sealed class WpfApplicationContext
    {
        public static DirectoryInfo ExecutableDirectory
        {
            get
            {
                DirectoryInfo di;
                try
                {
                    var f = new FileIOPermission(PermissionState.None);
                    f.AllLocalFiles = FileIOPermissionAccess.Read;

                    f.Assert();

                    di = new DirectoryInfo(Path.GetDirectoryName(
                        Assembly.GetExecutingAssembly().Location));
                }
                catch (SecurityException)
                {
                    di = new DirectoryInfo(Directory.GetCurrentDirectory());
                }

                return di;
            }
        }

        public static DirectoryInfo DocumentDirectory => new DirectoryInfo(Directory.GetCurrentDirectory());

        public static Uri DocumentDirectoryUri
        {
            get
            {
                var sUri = DocumentDirectory.FullName + "/";
                sUri = "file://" + sUri.Replace("\\", "/");
                return new Uri(sUri);
            }
        }
    }
}