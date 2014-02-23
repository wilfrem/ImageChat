using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RazorEngine.Templating;

namespace ImageChat
{
    internal static class TemplateServiceExtensions
    {
        public static string Display<T>(this TemplateService service, string templatePath, T model,
            DynamicViewBag viewBag, string cacheName)
        {
            var template = TemplateLoader.Load(templatePath);
            return service.Parse(template, model, viewBag, cacheName);
        }
    }
}