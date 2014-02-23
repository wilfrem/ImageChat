using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ImageChat
{
    internal static class TemplateLoader
    {
        public static string Load(string templateName)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) ?? "", "Views",
                templateName);
            return File.ReadAllText(path);
        }
    }
}