using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using OWin.LayoutFileSystem;
using System;
using System.IO;
using System.Reflection;

namespace Test
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var options = new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                EnableDefaultFiles = true,
                DefaultFilesOptions = { DefaultFileNames = { "index.html" } },
                FileSystem = new LayoutPhysicalFileSystem(
                    System.IO.Path.Combine(GetInstallPath(), "Resources"))
            };

            app.UseFileServer(options);
        }

        public static string GetInstallPath()
        {
            string rootPath = null;
            if (string.IsNullOrEmpty(rootPath))
            {
                rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (rootPath.ToLower().EndsWith("bin"))
                {
                    rootPath = Directory.GetParent(rootPath).FullName;
                }
            }
            return rootPath;
        }
    }
}
