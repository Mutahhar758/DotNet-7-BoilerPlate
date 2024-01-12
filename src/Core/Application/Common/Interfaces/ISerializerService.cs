using Newtonsoft.Json;

namespace Demo.WebApi.Application.Common.Interfaces;

public interface ISerializerService : ITransientService
{
    string Serialize<T>(T obj, JsonSerializerSettings settings = null);

    string Serialize<T>(T obj, Type type);

    T Deserialize<T>(string text);
}