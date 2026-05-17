using System.Collections.Generic;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Readers
{
    public interface IReader : ITranslateComparator
    {
        public Task<IList<LanguagePackage>> ReadAsync(string file);

        public Task WriteAsync(string file, LanguagePackage package);
        public Task WriteAsync(string file, IEnumerable<LanguagePackage> items);


    }

    public interface ITranslateComparator
    {
        /// <summary>
        /// 判断两个是否是相同的源
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool IsMatchSource(ITranslateUnit from, ITranslateUnit to);
        /// <summary>
        /// 转换翻译文本
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void TranslateTarget(ITranslateUnit from, ITranslateUnit to);
    }
}
