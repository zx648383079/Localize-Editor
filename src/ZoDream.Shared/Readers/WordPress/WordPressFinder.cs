using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Readers.WordPress
{
    public class WordPressFinder : FileFinder
    {
        protected override bool IsMatchFile(FileInfo file)
        {
            return file.Extension == ".php";
        }
        protected override bool IsMatchLine(string line, FileInfo file)
        {
            return line.Contains("__(");
        }

        protected override bool MatchLine(string line, string fileName, LanguagePackage package, FileInfo file)
        {
            throw new NotImplementedException();
        }
    }
}
