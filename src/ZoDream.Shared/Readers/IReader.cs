using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Readers
{
    public interface IReader
    {
        public Task<LanguagePackage> ReadAsync(string file);

        public Task WriteAsync(string file, LanguagePackage package);
    }
}
