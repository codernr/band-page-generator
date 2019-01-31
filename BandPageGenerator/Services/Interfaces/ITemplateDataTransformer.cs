using System.Collections.Generic;
using System.Threading.Tasks;

namespace BandPageGenerator.Services.Interfaces
{
    public interface ITemplateDataTransformer
    {
        Task AddTemplateData(Dictionary<string, object> templateData);
    }
}
