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

If any html page needs to be a layout add a ```html <section /> ``` element and specify a unique name within the file.
Below we have two sections: head and boady:

```html
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Basic layout</title>
    <section name="head" />
</head>
<body>
    <section name="body" />
</body>
</html>
```

To use this html file as layout page two things are needed:
  * ```html <layouts /> ``` element
    * file attribute will specify the path to the layouts file, relative to the location of the current html file
  * ```html <section ></section> ``` element
    * name attribute will specify the name of a section where the current section's inner html will be put

Add jQuery to layouts and the some body HTML elements:

```html
<layouts file="_layouts.html" />
<section name="head">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.2/jquery.min.js"></script>
    <script>
      //add some jQuery initialization
    </script>
</section>
<section name="body">
<!-- add some html to be placed in the body of the layouts page -->
</section>
```
