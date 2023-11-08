using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Readers.Database
{
    public class MySQLReader : IReader, IDatabaseReader
    {
        public string IdKey { get; set; } = "id";
        public string SourceKey { get; set; } = "source";
        public string TargetKey { get; set; } = "target";
        public string TableName { get; set; } = "";
        public string ConnectStringBuilder(string host, string username, string password,
            string schema)
        {
            return $"Server={host};Uid={username};Pwd={password};Database={schema};UseCompression=True;AllowBatch=true;UseUsageAdvisor=True;";
        }
        public string ConnectStringBuilder(string host, string username, string password,
            string schema, string table, string idKey, string sourceKey, string targetKey)
        {
            TableName = table;
            IdKey = idKey;
            SourceKey = sourceKey;
            TargetKey = targetKey;
            return ConnectStringBuilder(host, username, password, schema);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">Server=127.0.0.1;Uid=root;Pwd=root;Database=zodream;UseCompression=True;AllowBatch=true;UseUsageAdvisor=True;</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<LanguagePackage> ReadAsync(string file)
        {
            return Task.Factory.StartNew(() => {
                return Read(file);
            });
        }

        public LanguagePackage Read(string connectionString)
        {
            var package = new LanguagePackage("en");
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var sql = $"SELECT {IdKey},{SourceKey},{TargetKey} FROM `{TableName}`";
            var cmd = new MySqlCommand(sql, connection);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                package.Items.Add(new UnitItem(reader.GetString(1), reader.GetString(2))
                {
                    Id = reader.GetString(0),
                });
            }
            return package;
        }

        public Task WriteAsync(string file, LanguagePackage package)
        {
            return Task.Factory.StartNew(() => {
                Write(file, package);
            });
        }

        public void Write(string connectionString, LanguagePackage package)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var sql = $"UPDATE `{TableName}` SET {TargetKey}=@target WHERE {IdKey}=@id";
            foreach (var item in package.Items)
            {
                var cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.Add(new MySqlParameter("@target", item.Target));
                cmd.Parameters.Add(new MySqlParameter("@id", item.Id));
                cmd.ExecuteNonQuery();
            }
        }

        public async Task<string[]> LoadSchemaAsync(string host, string username, string password)
        {
            using var connection = new MySqlConnection(ConnectStringBuilder(host, username, password, "information_schema"));
            try
            {
                connection.Open();
            }
            catch
            {
                return Array.Empty<string>();
            }
            var sql = "SHOW DATABASES";
            var cmd = new MySqlCommand(sql, connection);
            using var reader = cmd.ExecuteReader();
            var items = new List<string>();
            var filters = new string[] { "information_schema", "mysql", "performance_schema", "sys" };
            while (await reader.ReadAsync())
            {
                var name = reader.GetString(0);
                if (filters.Contains(name))
                {
                    continue;
                }
                items.Add(name);
            }
            return items.ToArray();
        }

        public async Task<string[]> LoadTableAsync(string host, string username, string password, string schema)
        {
            using var connection = new MySqlConnection(ConnectStringBuilder(host, 
                username, password, schema));
            try
            {
                connection.Open();
            }
            catch
            {
                return Array.Empty<string>();
            }
            var sql = "SHOW TABLES";
            var cmd = new MySqlCommand(sql, connection);
            using var reader = cmd.ExecuteReader();
            var items = new List<string>();
            while (await reader.ReadAsync())
            {
                items.Add(reader.GetString(0));
            }
            return items.ToArray();
        }

        public async Task<string[]> LoadFieldAsync(string host, string username, string password, string schema, string table)
        {
            using var connection = new MySqlConnection(ConnectStringBuilder(host,
                username, password, schema));
            try
            {
                connection.Open();
            }
            catch
            {
                return Array.Empty<string>();
            }
            var sql = $"SHOW COLUMNS FROM `{table}`";
            var cmd = new MySqlCommand(sql, connection);
            using var reader = cmd.ExecuteReader();
            var items = new List<string>();
            while (await reader.ReadAsync())
            {
                items.Add(reader.GetString(0));
            }
            return items.ToArray();
        }
    }
}
