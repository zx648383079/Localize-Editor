using System;
using System.IO;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Readers.Angular
{
    public class AngularFinder : FileFinder
    {

        protected override bool IsMatchFile(FileInfo file)
        {
            return file.Extension == ".html" || file.Extension == ".ts";
        }

        protected override bool IsMatchLine(string line, FileInfo file)
        {
            if (file.Extension == ".html")
            {
                return line.Contains("i18n");
            }
            if (file.Extension == ".ts")
            {
                return line.Contains("$localize ");
            }
            return false;
        }

        protected override bool MatchLine(string line, string fileName, LanguagePackage package, FileInfo file)
        {
            throw new NotImplementedException();
        }
    }
}
