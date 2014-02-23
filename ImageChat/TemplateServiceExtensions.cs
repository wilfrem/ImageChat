using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RazorEngine.Templating;

namespace ImageChat
{
    static class TemplateServiceExtensions
    {
        /*const string BaseTemplate = "Shared/_Layout.cshtml";
        public static string DisplayWithBase<T>(this TemplateService service, string templatePath, T model, DynamicViewBag viewBag, string cacheName)
        {
            templatePath ="~/Views/"+templatePath;
            return service.Display(BaseTemplate, new { Path = templatePath, Model = model }, viewBag, cacheName);
        }*/
        public static string Display<T>(this TemplateService service, string templatePath, T model, DynamicViewBag viewBag, string cacheName)
        {
            var template = TemplateLoader.Load(templatePath);
            return service.Parse(template, model, viewBag, cacheName);
        }
    }
}
