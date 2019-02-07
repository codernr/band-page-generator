using BandPageGenerator.Services.Interfaces;
using HandlebarsDotNet;
using System;
using System.IO;
using System.Threading.Tasks;


namespace BandPageGenerator.Services
{
    public class HandlebarsViewRenderer : IViewRenderer
    {
        public HandlebarsViewRenderer()
        {
            Handlebars.RegisterHelper("date", this.DateFormatter);
        }

        public async Task<string> RenderViewToStringAsync<TModel>(string filePath, TModel model)
        {
            var template = await File.ReadAllTextAsync(filePath);

            var compiled = Handlebars.Compile(template);

            return compiled(model);
        }

        private void DateFormatter(TextWriter output, dynamic context, params object[] arguments)
        {
            if (arguments.Length != 2)
            {
                throw new HandlebarsException("{{date}} needs two parameter");
            }
            if (arguments[0].GetType() != typeof(DateTime) || arguments[1].GetType() != typeof(string))
            {
                throw new HandlebarsException("Invalid argument types, should be {{date DateTime string}}");
            }

            DateTime date = (DateTime)arguments[0];
            string format = (string)arguments[1];

            output.WriteSafeString(date.ToString(format));
        }
    }
}
