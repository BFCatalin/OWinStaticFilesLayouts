using System;
using System.IO;
using System.Text.RegularExpressions;

namespace OWin.LayoutFileSystem
{
    public class LayoutHtmlFile
    {
        private string fileFullPath;

        public LayoutHtmlFile(string fileFullPath)
        {
            this.fileFullPath = fileFullPath;
        }

        public FileInfo Compile(string outputFolder)
        {
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            FileInfo fi = new FileInfo(this.fileFullPath);
            string compiledFile = Path.Combine(outputFolder, fi.Name);
            FileInfo compiledFileInfo = new FileInfo(compiledFile);
            if (fi.Exists && fi.Extension.StartsWith(".htm"))
            {
                using (StreamReader textStream = fi.OpenText())
                {
                    string directiveLine = textStream.ReadLine();
                    Match layoutsMatch = Regex.Match(directiveLine, "<layouts.+file=['|\"]{0,1}(.+\\.htm[l]{0,1}).+/>");
                    if (layoutsMatch.Success && layoutsMatch.Groups.Count > 0)
                    {
                        string layoutsFile = Path.Combine(fi.Directory.FullName, layoutsMatch.Groups[1].Value);
                        FileInfo layoutsFileInfo = new FileInfo(layoutsFile);
                        if (fi.LastWriteTime > compiledFileInfo.LastWriteTime || layoutsFileInfo.LastWriteTime > compiledFileInfo.LastWriteTime)
                        {
                            compiledFileInfo = null;
                            if (File.Exists(layoutsFile))
                            {
                                string layoutsContent = File.ReadAllText(layoutsFile);
                                string pageContent = textStream.ReadToEnd();
                                Match sectionMatch = Regex.Match(pageContent, "<section[^>]+name=['|\"]{1}([^\"]+)['|\"]{1}.*>((.|\\n|\\r|\\r\\n)*?)</section>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                while (sectionMatch.Success)
                                {
                                    if (sectionMatch.Groups.Count >= 3)
                                    {
                                        string sectionName = sectionMatch.Groups[1].Value;
                                        string sectionContent = sectionMatch.Groups[2].Value;
                                        layoutsContent = Regex.Replace(layoutsContent, "(<section[^>]+name=['|\"]{1}" + sectionName + "['|\"]{1}.*/>)", sectionContent);
                                    }
                                    sectionMatch = sectionMatch.NextMatch();
                                }
                                File.WriteAllText(compiledFile, layoutsContent);
                                compiledFileInfo = new FileInfo(compiledFile);
                            }
                            else
                            {
                                compiledFileInfo = LayoutHtmlFile.OutputCompileErrorPage(outputFolder, string.Format("Could not locate file '{0}'", layoutsFile));
                            }
                        }
                    }
                }
            }
            else
            {
                compiledFileInfo = LayoutHtmlFile.OutputCompileErrorPage(outputFolder, string.Format("Could not locate file '{0}'", fi.FullName));
            }
            return compiledFileInfo;
        }

        public static FileInfo OutputCompileErrorPage(string outputFolder, string errorMessage)
        {
            string errorHtmlFile = Path.Combine(outputFolder, "error.html");
            File.WriteAllText(errorHtmlFile, "<!DOCTYPE html><html xmlns=\"http://www.w3.org/1999/xhtml\"><head><title>Error compiling html files</title></head><body>" + errorMessage + "</body></html>");
            return new FileInfo(errorHtmlFile);
        }
    }
}
