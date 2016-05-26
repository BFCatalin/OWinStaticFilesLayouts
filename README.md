Allows hotsing of basic html layouts from file system.

It configures similarly to PhysicalFileSystem:

```D
public class Startup
{
  public void Configuration(IAppBuilder app)
  {
      var options = new FileServerOptions
      {
          EnableDirectoryBrowsing = true,
          EnableDefaultFiles = true,
          DefaultFilesOptions = { DefaultFileNames = { "index.html" } },
          FileSystem = new LayoutPhysicalFileSystem("C:\\www")
      };

      app.UseFileServer(options);
  }
}
```

And start the server like usual:

```D
private static void Main(string[] args)
{
    using (WebApp.Start<Startup>("http://localhost:12345"))
    {
        Console.ReadLine();
    }
}
```
