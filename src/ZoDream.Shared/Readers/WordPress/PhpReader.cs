using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Readers.WordPress
{
    public class PhpReader : IReader
    {
        public Task<LanguagePackage> ReadAsync(string file)
        {
            throw new NotImplementedException();
        }

        public Task WriteAsync(string file, LanguagePackage package)
        {
            throw new NotImplementedException();
        }
    }
}
