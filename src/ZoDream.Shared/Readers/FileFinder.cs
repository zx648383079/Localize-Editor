using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Readers
{
    public abstract class FileFinder : IFinder
    {

        public Task<LanguagePackage> SearchAsync(string folder, CancellationToken token = default)
        {
            return Task.Factory.StartNew(() => {
                var package = new LanguagePackage("en");
                LookFolder(folder, file => {
                    LookFile(file, file.FullName.Substring(folder.Length), package, token);
                }, token);
                return package;
            }, token);
        }

        protected void LookFolder(string folder, Action<FileInfo> action, CancellationToken token)
        {
            var file = new FileInfo(folder);
            if (file.Exists)
            {
                action?.Invoke(file);
                return;
            }
            var folderInfo = new DirectoryInfo(folder);
            if (folderInfo.Exists)
            {
                LookFolder(folderInfo, action, token);
            }
        }

        protected void LookFolder(DirectoryInfo folder, Action<FileInfo> action, CancellationToken token)
        {
            if (!IsMatchFolder(folder))
            {
                return;
            }
            foreach (var item in folder.GetFiles())
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                action?.Invoke(item);
            }
            foreach (var item in folder.GetDirectories())
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                LookFolder(item, action!, token);
            }
        }

        protected void LookFile(FileInfo file, string relativePath, LanguagePackage package, CancellationToken token)
        {
            if (token.IsCancellationRequested || !IsMatchFile(file))
            {
                return;
            }
            MatchFile(file, relativePath, package, token);
        }

        protected virtual bool IsMatchFolder(DirectoryInfo folder)
        {
            return true;
        }
        protected virtual bool IsMatchFile(FileInfo file)
        {
            return true;
        }

        protected abstract bool IsMatchLine(string line, FileInfo file);

        protected virtual void MatchFile(FileInfo file, string relativePath, LanguagePackage package, CancellationToken token)
        {
            using var reader = new LineReader(file.FullName);
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null || token.IsCancellationRequested)
                {
                    break;
                }
                if (!IsMatchLine(line, file))
                {
                    continue;
                }
                MatchLine(line, relativePath, package, file);
            }
        }

        protected abstract bool MatchLine(string line, string fileName, LanguagePackage package, FileInfo file);
    }
}
