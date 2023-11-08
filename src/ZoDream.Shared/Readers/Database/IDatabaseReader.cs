using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Readers.Database
{
    public interface IDatabaseReader: IReader
    {
        /// <summary>
        /// 生成链接字符
        /// </summary>
        /// <param name="host"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="schema"></param>
        /// <param name="table"></param>
        /// <param name="idKey"></param>
        /// <param name="sourceKey"></param>
        /// <param name="targetKey"></param>
        /// <returns></returns>
        public string ConnectStringBuilder(string host, string username, string password, 
            string schema, string table, string idKey, string sourceKey, string targetKey);

        public Task<string[]> LoadSchemaAsync(string host, string username, string password);

        public Task<string[]> LoadTableAsync(string host, string username, string password, string schema);

        public Task<string[]> LoadFieldAsync(string host, string username, string password,
            string schema, string table);
    }
}
