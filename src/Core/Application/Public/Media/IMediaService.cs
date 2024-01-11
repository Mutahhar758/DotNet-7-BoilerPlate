namespace Demo.WebApi.Application.Public.Media;
public interface IMediaService : IScopedService
{
    Task<MediaResponse> AddMediaAsync(MediaRequest request, bool compress = false, int? height = null, int? width = null);
    Task<MediaResponse> AddMediaAsync(MediaFromStreamRequest request);
    Task<bool> DeleteMediaAsync(int mediaId);
}
