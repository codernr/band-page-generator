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
            Handlebars.RegisterHelper("date", this.DateFormatter);
            Handlebars.RegisterHelper("interval", this.IntervalFormatter);
        }

        public async Task<string> RenderViewToStringAsync<TModel>(string filePath, TModel model)
        {
            var template = await File.ReadAllTextAsync(filePath);

            var compiled = Handlebars.Compile(template);

            return compiled(model);
        }

        private void DateFormatter(TextWriter output, dynamic context, params object[] arguments)
        {
            this.CheckArguments<DateTime>("date", arguments);

            DateTime date = (DateTime)arguments[0];
            string format = (string)arguments[1];

            output.WriteSafeString(date.ToString(format, CultureInfo.InvariantCulture));
        }

        private void IntervalFormatter(TextWriter output, dynamic context, params object[] arguments)
        {
            this.CheckArguments<TimeSpan>("interval", arguments);

            TimeSpan time = (TimeSpan)arguments[0];
            string format = (string)arguments[1];

            output.WriteSafeString(time.ToString(format, CultureInfo.InvariantCulture));
        }

        private void CheckArguments<TFormat>(string name, params object[] arguments)
        {
            if (arguments.Length != 2)
            {
                throw new HandlebarsException("{{date}} needs two parameter");
            }
            if (arguments[0].GetType() != typeof(TFormat) || arguments[1].GetType() != typeof(string))
            {
                throw new HandlebarsException("Invalid argument types, should be {{" + name + " " + typeof(TFormat).GetType().Name + " string}}");
            }
        }
    }
}
