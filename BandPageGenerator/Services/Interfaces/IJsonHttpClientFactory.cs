namespace BandPageGenerator.Services.Interfaces
{
    public enum ClientNamingPolicy
    {
        CamelCase
    }

    public interface IJsonHttpClientFactory
    {
        JsonHttpClient CreateClient(ClientNamingPolicy namingPolicy);
    }
}