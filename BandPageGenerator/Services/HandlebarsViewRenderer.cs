using BandPageGenerator.Services.Interfaces;
using HandlebarsDotNet;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;


namespace BandPageGenerator.Services
{
    public class HandlebarsViewRenderer : IViewRenderer
    {
        public HandlebarsViewRenderer()
        {
            Handlebars.RegisterHelper("date", DateFormatter);
            Handlebars.RegisterHelper("interval", IntervalFormatter);
        }

        public async Task<string> RenderViewToStringAsync<TModel>(string filePath, TModel model)
        {
            var template = await File.ReadAllTextAsync(filePath);

            var compiled = Handlebars.Compile(template);

            return compiled(model);
        }

        private static void DateFormatter(TextWriter output, dynamic context, params object[] arguments)
        {
            CheckArguments<DateTime>("date", arguments);

            DateTime date = (DateTime)arguments[0];
            string format = (string)arguments[1];

            output.WriteSafeString(date.ToString(format, CultureInfo.InvariantCulture));
        }

        private static void IntervalFormatter(TextWriter output, dynamic context, params object[] arguments)
        {
            CheckArguments<TimeSpan>("interval", arguments);

            TimeSpan time = (TimeSpan)arguments[0];
            string format = (string)arguments[1];

            output.WriteSafeString(time.ToString(format, CultureInfo.InvariantCulture));
        }

        private static void CheckArguments<TFormat>(string name, params object[] arguments)
        {
            if (arguments.Length != 2)
            {
                throw new HandlebarsException("{{date}} needs two parameter");
            }
            if (arguments[0].GetType() != typeof(TFormat) || arguments[1].GetType() != typeof(string))
            {
                throw new HandlebarsException("Invalid argument types, should be {{" + name + " " + typeof(TFormat).Name + " string}}");
            }
        }
    }
}
