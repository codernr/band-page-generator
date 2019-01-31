using BandPageGenerator.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class NullTemplateDataTransformer : ITemplateDataTransformer
    {
        public Task AddTemplateData(Dictionary<string, object> templateData)
        {
            return Task.CompletedTask;
        }
    }
}
