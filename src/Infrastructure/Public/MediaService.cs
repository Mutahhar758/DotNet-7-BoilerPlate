using Demo.WebApi.Application.Common.Persistence;
using Demo.WebApi.Domain.Common;
using Demo.WebApi.Domain.Public;
using Demo.WebApi.Infrastructure.Common.Extensions;
using Mapster;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Demo.WebApi.Application.Public.Media;
using Demo.WebApi.Application.Storage;
using Demo.WebApi.Infrastructure.FileStorage;
using Demo.WebApi.Application.Common.Exceptions;

namespace Demo.WebApi.Infrastructure.Public;
public class MediaService : IMediaService
{
    private readonly IRepository<Media> _mediaRepository;
    private readonly IAzureStorageService _azureService;
    private readonly IStringLocalizer<MediaService> _localizer;
    private readonly AzureStorageSettings _azureSettings;

    public MediaService(IRepository<Media> mediaRepository, IAzureStorageService azureService, IStringLocalizer<MediaService> localizer, IOptions<AzureStorageSettings> azureSettings)
    {
        _mediaRepository = mediaRepository;
        _azureService = azureService;
        _localizer = localizer;
        _azureSettings = azureSettings.Value;
    }

    public async Task<MediaResponse> AddMediaAsync(MediaRequest request, bool compress = false, int? height = null, int? width = null)
    {
        var mediaStream = new MediaStream()
        {
            OriginalFileName = request.MediaFile.FileName
        };

        if (request.MediaFile.Length > _azureSettings.FileSizeLimit)
        {
            throw new ConflictException(_localizer[$"File Size can't be greater than {_azureSettings.FileSizeLimit / 1048576}."]);
        }

        string extension = Path.GetExtension(mediaStream.OriginalFileName).ToLower();

        if (compress && FileType.Image.GetDescriptionList().Any(x => x == extension))
            mediaStream.InputStream = ImageCompression.ResizeImage(request.MediaFile.OpenReadStream(), height!.Value, width!.Value);
        else
            mediaStream.InputStream = request.MediaFile.OpenReadStream();

        string[] allowedExtensions = _azureSettings.MediaAllowedExtension.Split(',');

        if (!allowedExtensions.Contains(extension))
        {
            throw new ConflictException(_localizer[$"Invalid Media extension format type. Allowed Types are: {_azureSettings.MediaAllowedExtension}."]);
        }

        string convertedFileName = $"{Guid.NewGuid()}{Path.GetExtension(mediaStream.OriginalFileName).ToLower()}";

        //string tempPath = UploadFileOnTempPath(mediaStream.InputStream, convertedFileName);

        var media = new Media
        {
            ConvertedFileName = convertedFileName,
            FileType = extension,
            OriginalFileName = mediaStream.OriginalFileName,
        };

        await _mediaRepository.AddAsync(media);

        (media.AccessURL, media.Path) = await _azureService.UploadAsync(mediaStream.InputStream, request.Path, convertedFileName);

        await _mediaRepository.UpdateAsync(media);

        return media.Adapt<MediaResponse>();
    }

    public async Task<MediaResponse> AddMediaAsync(MediaFromStreamRequest request)
    {
        var mediaStream = new MediaStream
        {
            OriginalFileName = request.MediaName,
            InputStream = new MemoryStream(request.Media)
        };
        string convertedFileName = $"{Guid.NewGuid()}{Path.GetExtension(mediaStream.OriginalFileName).ToLower()}";

        var media = new Media
        {
            ConvertedFileName = convertedFileName,
            FileType = request.Extension,
            OriginalFileName = mediaStream.OriginalFileName,
        };

        await _mediaRepository.AddAsync(media);

        (media.AccessURL, media.Path) = await _azureService.UploadAsync(mediaStream.InputStream, request.Path, convertedFileName);

        await _mediaRepository.UpdateAsync(media);

        return media.Adapt<MediaResponse>();
    }

    public async Task<bool> DeleteMediaAsync(int mediaId)
    {
        throw new NotImplementedException();
    }
}
