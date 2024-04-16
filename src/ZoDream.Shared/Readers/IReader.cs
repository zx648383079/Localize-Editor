using System.Collections.Generic;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Readers
{
    public interface IReader
    {
        public Task<IList<LanguagePackage>> ReadAsync(string file);

        public Task WriteAsync(string file, LanguagePackage package);
        public Task WriteAsync(string file, IEnumerable<LanguagePackage> items);
    }
}
