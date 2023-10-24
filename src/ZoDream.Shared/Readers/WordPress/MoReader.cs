using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;

namespace ZoDream.Shared.Readers.WordPress
{
    public class MoReader : IReader
    {
        public Task<LanguagePackage> ReadAsync(string file)
        {
            return Task.Factory.StartNew(() => {
                return ReadFile(file);
            });
        }

        public LanguagePackage ReadFile(string file)
        {
            using var stream = File.Open(file, FileMode.Open, FileAccess.Read);
            var reader = new BinaryReader(stream);
            return ReadStream(reader);
        }

        public LanguagePackage ReadStream(BinaryReader reader)
        {
            var package = new LanguagePackage("en");
            var magic = reader.ReadUInt32();
            if (magic != 0x950412de && magic != 0x12de9504)
            {
                return package;
                // throw new Exception("Not mo file!");
            }
            var revision = reader.ReadInt32();
            var count = reader.ReadInt32(); // 文本数量
            var sourcePos = reader.ReadInt32(); // 源文本信息位置
            var targetPos = reader.ReadInt32(); // 译文信息位置
            var hashSize = reader.ReadInt32(); // 哈希表大小
            var hashPos = reader.ReadInt32();  // 哈希表位置

            Encoding encoding = null;

            reader.BaseStream.Seek(sourcePos, SeekOrigin.Begin);
            // 大小，位置
            var oData = new Tuple<int, int>[count];
            for (int i = 0; i < count; i++)
            {
                oData[i] = new Tuple<int, int>(reader.ReadInt32(), reader.ReadInt32());
            }

            reader.BaseStream.Seek(targetPos, SeekOrigin.Begin);
            var tData = new Tuple<int, int>[count];
            // 大小，位置
            for (var i = 0; i < count; i++)
            {
                tData[i] = new Tuple<int, int>(reader.ReadInt32(), reader.ReadInt32());
            }

            if (oData[0].Item1 == 0) // header
            {
                reader.BaseStream.Seek(tData[0].Item2, SeekOrigin.Begin);
                var msg = Encoding.ASCII.GetString(reader.ReadBytes(tData[0].Item1)).Split(new[] { '\n' });
                var charset = msg.Where(s => s.StartsWith("Content-Type")).First().Split(new[] { '=' }, 2)[1].Trim();
                encoding = Encoding.GetEncoding(charset);
            }
            else
            {
                throw new Exception("Can't find header!");
            }
            for (var i = 0; i < count; i++)
            {
                reader.BaseStream.Seek(tData[i].Item2, SeekOrigin.Begin);
                var tra = encoding.GetString(reader.ReadBytes(tData[i].Item1));
                if (i == 0)
                {
                    ReadHeader(tra, package);
                    continue;
                }
                reader.BaseStream.Seek(oData[i].Item2, SeekOrigin.Begin);
                var ori = encoding.GetString(reader.ReadBytes(oData[i].Item1));
                var item = new UnitItem();
                if (ori.Contains('\0'))
                {
                    var _ori = ori.Split(new[] { '\0' });
                    item.Id = item.Source = _ori[0];
                    item.SourcePlural = _ori[1];
                    item.SetTarget(tra.Split(new[] { '\0' }));
                }
                else
                {
                    item.Id = item.Source = ori;
                    item.Target = tra;
                }
                package.Items.Add(item);
            }
            return package;
        }

        private void ReadHeader(string content, LanguagePackage package)
        {
            foreach (var line in content.Split('\n'))
            {
                var args = line.Split(new char[] { ':' }, 2);
                if (args.Length < 2)
                {
                    continue;
                }
                var name = args[0].Trim();
                var value = args[1].EndsWith("\\n") ? args[1].Substring(0, args[1].Length - 2) : args[1];
                if (name == PoReader.LanguageTag)
                {
                    package.TargetLanguage = value;
                    continue;
                }
                package.MetaItems.Add(name, value);
            }
        }

        public Task WriteAsync(string file, LanguagePackage package)
        {
            return Task.Factory.StartNew(() => 
            {
                WriteFile(file, package);
            });
        }

        public void WriteFile(string file, LanguagePackage package)
        {
            var fs = File.OpenWrite(file);
            var writer = new BinaryWriter(fs);
            var items = package.Items.OrderBy(item => item.Target).ToArray();
            writer.Write(0x950412de);
            writer.Write(0);
            writer.Write(items.Length);
            writer.Write(7 * 4);
            writer.Write(7 * 4 + items.Length * 8);
            writer.Write(0);
        
            var idBuffer = new List<byte>();
            var strBuffer = new List<byte>();
            var count = items.Length + 1;
            var offsetItems = new int[count, 4];
            var encoding = Encoding.UTF8;
            var splitBuffer = encoding.GetBytes(new[] { '\0' });
            var lineBuffer = encoding.GetBytes(new[] { '\n' });

            // 写头
            foreach (var item in package.MetaItems)
            {
                if (item.Key == "Content-Type")
                {
                    continue;
                }
                strBuffer.AddRange(encoding.GetBytes(item.Key));
                strBuffer.AddRange(encoding.GetBytes(": "));
                strBuffer.AddRange(encoding.GetBytes(item.Value));
                strBuffer.AddRange(lineBuffer);
            }
            strBuffer.AddRange(encoding.GetBytes("Content-Type: text/plain; charset=UTF-8"));
            strBuffer.AddRange(lineBuffer);
            offsetItems[0, 3] = strBuffer.Count;
            idBuffer.AddRange(splitBuffer);
            strBuffer.AddRange(splitBuffer);

            // 写真实数据 

            for (int i = 1; i < count; i++)
            {
                var item = items[i - 1];
                var idOffset = idBuffer.Count;
                var strOffset = strBuffer.Count;
                if (string.IsNullOrWhiteSpace(item.SourcePlural))
                {
                    idBuffer.AddRange(encoding.GetBytes(item.Source));
                    strBuffer.AddRange(encoding.GetBytes(item.Target));
                } else
                {
                    idBuffer.AddRange(encoding.GetBytes(item.Source));
                    idBuffer.AddRange(splitBuffer);
                    idBuffer.AddRange(encoding.GetBytes(item.SourcePlural));
                    strBuffer.AddRange(encoding.GetBytes(item.Target));
                    foreach (var it in item.TargetPlural)
                    {
                        strBuffer.AddRange(splitBuffer);
                        strBuffer.AddRange(encoding.GetBytes(it));
                    }
                }
                offsetItems[i, 0] = idOffset;
                offsetItems[i, 1] = idBuffer.Count - idOffset;
                offsetItems[i, 2] = strOffset;
                offsetItems[i, 3] = strBuffer.Count - strOffset;
                idBuffer.AddRange(splitBuffer);
                strBuffer.AddRange(splitBuffer);
            }
            var keyOffset = 7 * 4 + count * 4 * 4;
            var valueOffset = keyOffset + idBuffer.Count;
            writer.Write(keyOffset);
            for (var i = 0; i < count; i++)
            {
                writer.Write(offsetItems[i, 1]);
                writer.Write(offsetItems[i, 0] + keyOffset);
            }
            for (var i = 0; i < count; i++)
            {
                writer.Write(offsetItems[i, 3]);
                writer.Write(offsetItems[i, 2] + valueOffset);
            }
            writer.Write(idBuffer.ToArray());
            writer.Write(strBuffer.ToArray());
        }


    }
}
