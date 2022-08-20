using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SuperStudio.CommonUtils;

namespace SuperRename.Core.Utils
{
    public static class FileUtils
    {



        public static string changeFileExt(string origin, string ext)
        {
            if (origin == null || ext == null || !FileUtil.IsProperFilename(origin) || !FileUtil.IsProperFilename(ext)) return origin;
            if (ext.Length <= 0) return origin;
            if (origin.Length <= 0) return "." + ext;
            if (origin.LastIndexOf(".", StringComparison.OrdinalIgnoreCase) < 0)
            {
                return origin + "." + ext;
            }
            else
            {
                string[] o = origin.Split('.');
                o[o.Length - 1] = ext;
                return String.Join(".", o);
            }
        }
    }
}
