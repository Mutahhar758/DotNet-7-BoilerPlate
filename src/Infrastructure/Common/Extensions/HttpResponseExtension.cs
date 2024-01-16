using Demo.WebApi.Application.Common.Enums;
using Demo.WebApi.Application.Common.Models;
using System.Net;

namespace Demo.WebApi.Infrastructure.Common.Extensions;

public static class HttpResponseExtension
{
    public static T ToInformationResponse<T>(this T data, string? message = null, HttpStatusCode httpStatus = HttpStatusCode.OK)
        where T : HttpResponseDetail
    {
        data.Message = new HttpResponseDto
        {
            Type = HttpResponseType.Information.ToString(),
            StatusCode = (int)httpStatus,
            Message = message,
        };

        return data;
    }

    public static HttpResponseDetail InformationResponse(string? message = null, HttpStatusCode httpStatus = HttpStatusCode.OK)
   => ToInformationResponse(new HttpResponseDetail(), message, httpStatus);

    public static HttpResponseDetail ToWarningResponse(this HttpResponseDetail data, string? message = null, HttpStatusCode httpStatus = HttpStatusCode.OK)
    {
        data.Message = new HttpResponseDto
        {
            Type = HttpResponseType.Warning.ToString(),
            StatusCode = (int)httpStatus,
            Message = message,
        };
        return data;
    }

    public static T ToSuccessResponse<T>(this T data, string? message = null, HttpStatusCode httpStatus = HttpStatusCode.OK)
        where T : HttpResponseDetail
    {
        data.Message = new HttpResponseDto
        {
            Type = HttpResponseType.Success.ToString(),
            StatusCode = (int)httpStatus,
            Message = message,
        };

        return data;
    }

    public static HttpResponseDetail SuccessResponse(string? message = null, HttpStatusCode httpStatus = HttpStatusCode.OK)
    => ToSuccessResponse(new HttpResponseDetail(), message, httpStatus);

    public static HttpResponseDetail ToErrorResponse(this HttpResponseDetail data, string? message = null, HttpStatusCode httpStatus = HttpStatusCode.BadRequest)
    {
        data.Message = new HttpResponseDto
        {
            Type = HttpResponseType.Error.ToString(),
            StatusCode = (int)httpStatus,
            Message = message,
        };
        return data;
    }
}
