using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Readers.Angular
{
    public class XlfReader : IReader
    {
        public LanguagePackage Read(string file)
        {
            
        }

        public void Write(string file, LanguagePackage package)
        {
            throw new NotImplementedException();
        }
    }
}
