using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Readers
{
    public interface IFinder
    {
        /// <summary>
        /// 提取翻译文件内容
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public Task<LanguagePackage> SearchAsync(string folder, CancellationToken token = default);
    }
}
