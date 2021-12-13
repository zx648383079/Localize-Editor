using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Readers
{
    public class CharReader : IEnumerator<char>, IEnumerable<char>
    {

        public readonly string Content;
        public int Position { get; set; } = -1;
        public CharReader(string content)
        {
            Content = content;
        }

        public int Length => Content.Length;

        public string? ReadLine()
        {
            var sb = new StringBuilder();
            while (MoveNext())
            {
                var code = Current;
                if (code == '\n')
                {
                    break;
                }
                if (code == '\r')
                {
                    if (NextIs('\n'))
                    {
                        MoveNext();
                    }
                    break;
                }
                sb.Append(code);
            }
            if (!CanNext && sb.Length < 1)
            {
                return null;
            }
            return sb.ToString();
        }

        public char Current => Content[Position];


        public void Dispose()
        {

        }

        public bool MoveNext()
        {
            if (!CanNext)
            {
                return false;
            }
            Position++;
            return true;
        }

        public void Reset()
        {
            Position = -1;
        }

        IEnumerator<char> IEnumerable<char>.GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        public bool CanNext => Position < Content.Length - 1;
        public bool CanBack => Position > 1;

        object IEnumerator.Current => Current;

        public int IndexOf(char c, int offset = 0)
        {
            return Content.IndexOf(c, Position + offset);
        }
        public int IndexOf(string s, int offset = 0)
        {
            return Content.IndexOf(s, Position + offset);
        }

        public string Read(int length = 1, int offset = 0)
        {
            if (length == 0)
            {
                return string.Empty;
            }
            var pos = (length < 0 ? Position + length : Position) + offset;
            if (pos > Content.Length - 1)
            {
                return string.Empty;
            }
            var len = length < 0 ? -length : length;
            return Content.Substring(pos, len);
        }

        public string ReadSeek(int position, int length = 1)
        {
            return Content.Substring(position, length);
        }

        public bool NextIs(params char[] items)
        {
            if (!CanNext)
            {
                return false;
            }
            var c = Content[Position + 1];
            foreach (var item in items)
            {
                if (c == item)
                {
                    return true;
                }
            }
            return false;
        }

        public bool NextIs(params string[] items)
        {
            if (!CanNext)
            {
                return false;
            }
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }
                if (Content.Substring(Position + 1, item.Length) == item)
                {
                    return true;
                }
            }
            return false;
        }

        public int MinIndex(params char[] items)
        {
            var index = -1;
            var min = -1;
            for (int i = items.Length - 1; i >= 0; i--)
            {
                var j = IndexOf(items[i]);
                if (j >= 0 && (min < 0 || j <= min))
                {
                    index = i;
                    min = j;
                }
            }
            return index;
        }

        public int MinIndex(params string[] items)
        {
            var index = -1;
            var min = -1;
            for (int i = items.Length - 1; i >= 0; i--)
            {
                var j = IndexOf(items[i]);
                if (j >= 0 && (min < 0 || j <= min))
                {
                    index = i;
                    min = j;
                }
            }
            return index;
        }

        /// <summary>
        /// 反向遍历，不移动当前位置
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="offset">默认从前一个位置开始</param>
        public void Reverse(Func<char, int, bool?> cb, int offset = -1)
        {
            var i = Position + offset;
            while (i >= 0)
            {
                if (cb(Content[i], i) == false)
                {
                    break;
                }
                i--;
            }
        }

        /// <summary>
        /// 字符是否是上一个字符，并计算连续出现的次数
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public int ReverseCount(char code)
        {
            var count = 0;
            Reverse((i, _) => {
                if (i != code)
                {
                    return false;
                }
                count++;
                return null;
            });
            return count;
        }
    }
}
