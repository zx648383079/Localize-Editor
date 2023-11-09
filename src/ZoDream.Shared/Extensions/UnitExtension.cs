using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Extensions
{
    public static class UnitExtension
    {

        public static void AddLine(this ITranslateUnit unit, string fileName, string line)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }
            foreach (var item in unit.Location)
            {
                if (item.FileName == fileName)
                {
                    item.Add(line);
                    return;
                }
            }
            unit.Location.Add(new SourceLocation(fileName, line));
        }


        public static void AddLine(this ITranslateUnit unit, SourceLocation location)
        {
            if (string.IsNullOrEmpty(location.FileName))
            {
                return;
            }
            foreach (var item in unit.Location)
            {
                if (item.FileName == location.FileName)
                {
                    item.Add(location.LineNumber);
                    return;
                }
            }
            unit.Location.Add(location);
        }

        public static int IndexOf(this ITranslateUnit unit, SourceLocation e)
        {
            return unit.IndexOf(e.FileName);
        }

        public static int IndexOf(this ITranslateUnit unit, string file)
        {
            for (int i = 0; i < unit.Location.Count; i++)
            {
                if (unit.Location[i].FileName == file)
                {
                    return i;
                }
            }
            return -1;
        }

        public static void AddLine(this IList<SourceLocation> items, string fileName, string line)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }
            foreach (var item in items)
            {
                if (item.FileName == fileName)
                {
                    item.Add(line);
                    return;
                }
            }
            items.Add(new SourceLocation(fileName, line));
        }


        public static void AddLine(this IList<SourceLocation> items, SourceLocation location)
        {
            if (string.IsNullOrEmpty(location.FileName))
            {
                return;
            }
            foreach (var item in items)
            {
                if (item.FileName == location.FileName)
                {
                    item.Add(location.LineNumber);
                    return;
                }
            }
            items.Add(location);
        }

        public static int IndexOf(this IList<SourceLocation> items, SourceLocation e)
        {
            return items.IndexOf(e.FileName);
        }

        public static int IndexOf(this IList<SourceLocation> items, string file)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].FileName == file)
                {
                    return i;
                }
            }
            return -1;
        }

        public static void AddLine(this ITranslateUnit unit, List<SourceLocation> items)
        {
            if (items.Count == 0)
            {
                return;
            }
            foreach (var item in items)
            {
                var i = unit.IndexOf(item);
                if (i < 0)
                {
                    unit.Location.Add(item);
                    continue;
                }
                unit.Location[i].Add(item.LineNumber);
            }
        }


        public static void SetTarget(this ITranslateUnit unit, string[] items)
        {
            for (var i = 0; i < items.Length; i++)
            {
                if (i < 1)
                {
                    unit.Target = items[i];
                }
                else
                {
                    unit.TargetPlural.Add(items[i]);
                }
            }
        }

        public static void AddTarget(this ITranslateUnit unit, string item)
        {
            if (string.IsNullOrEmpty(unit.Target))
            {
                unit.Target = item;
                return;
            }
            unit.TargetPlural.Add(item);
        }

        /// <summary>
        /// 判断两个是否是同一个待翻译语句
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsSameId(this ITranslateUnit unit, ITranslateUnit target)
        {
            if (string.IsNullOrWhiteSpace(unit.Id)  || string.IsNullOrWhiteSpace(target.Id))
            {
                return unit.Source == target.Source;
            }
            return unit.Id == target.Id;
        }
        /// <summary>
        /// 判断两个是否包含相同的文件
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool HasSameFile(this ITranslateUnit unit, ITranslateUnit target)
        {
            if (unit.Location.Count == 0 || target.Location.Count == 0)
            {
                return true;
            }
            foreach (var item in unit.Location)
            {
                foreach (var it in target.Location)
                {
                    if (item.FileName == it.FileName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 不复制翻译内容
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="target"></param>
        public static void InstanceTo(this ITranslateUnit unit, ITranslateUnit target)
        {
            target.Id = unit.Id;
            target.Source = unit.Source;
            target.SourcePlural = unit.SourcePlural;
            target.Location = new(unit.Location.ToArray());
            target.Comment = unit.Comment;
        }
        /// <summary>
        /// 不复制翻译内容
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="target"></param>
        public static void InstanceFrom(this ITranslateUnit unit, ITranslateUnit target)
        {
            target.InstanceTo(unit);
        }
        /// <summary>
        /// 不复制翻译内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static T Instance<T>(this ITranslateUnit unit)
            where T : ITranslateUnit, new()
        {
            return new T
            {
                Id = unit.Id,
                Source = unit.Source,
                SourcePlural = unit.SourcePlural,
                Location = new(unit.Location.ToArray()),
                Comment = unit.Comment
            };
        }
        /// <summary>
        /// 完全复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static T Clone<T>(this ITranslateUnit unit)
            where T: ITranslateUnit, new()
        {
            return new T
            {
                Id = unit.Id,
                Source = unit.Source,
                SourcePlural = unit.SourcePlural,
                Target = unit.Target,
                TargetPlural = unit.TargetPlural,
                Location = unit.Location,
                Comment = unit.Comment
            };
        }
    }
}
