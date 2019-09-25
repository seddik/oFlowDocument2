using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace oFlowDocument2
{
    public static class Tools
    {
        // Optim.Tools.Tools
        public static bool IsEmpty(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        public static string AppDir { get; }
        public static string GetLocalFile(params string[] v)
        {
            List<string> all = v.ToList();
            all.Insert(0, AppDir);
            return Path.Combine(all.ToArray());
        }

        static Tools()
        {
            AppDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public static string[] ToSplit(this string s, string separator)
        {
            try
            {
                return s.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            }
            catch
            {

            }

            return null;
        }

        public static string ToSplit(this string s, string separator, int idx)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return "";
            }

            try
            {
                return s.ToSplit(separator)[idx];
            }
            catch
            {

            }

            return "";
        }
    }
}