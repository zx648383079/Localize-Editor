using System;
using System.Collections.Generic;
using System.Text;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Readers
{
    public interface IReader
    {
        public LanguagePackage Read(string file);

        public void Write(string file, LanguagePackage package);
    }
}
