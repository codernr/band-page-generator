using Newtonsoft.Json.Serialization;

namespace BandPageGenerator.Services.Interfaces
{
    public interface IJsonHttpClient<TNamingStrategy> : IFormattedHttpClient where TNamingStrategy : NamingStrategy, new()
    {
    }
}
