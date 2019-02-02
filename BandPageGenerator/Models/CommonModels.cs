using System;

namespace BandPageGenerator.Models
{
    public class MustacheDateTime
    {
        public DateTime DateTime;

        public Func<dynamic, string, object> Render = (dyn, format) => ((DateTime)dyn.DateTime).ToString(format);
    }
}
