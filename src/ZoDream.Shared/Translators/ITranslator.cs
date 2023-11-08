using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Translators
{
    public interface ITranslator
    {
        /// <summary>
        /// 翻译一个文本
        /// </summary>
        /// <param name="sourceLang"></param>
        /// <param name="targetLang"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public Task<string> TranslateAsync(string sourceLang, string targetLang, 
            string text, CancellationToken token = default);
        /// <summary>
        /// 翻译整个语言包
        /// </summary>
        /// <param name="targetLang">目标语言</param>
        /// <param name="package"></param>
        /// <returns></returns>
        public Task<LanguagePackage> TranslateAsync(string targetLang, LanguagePackage package, CancellationToken token = default);
    }
}
