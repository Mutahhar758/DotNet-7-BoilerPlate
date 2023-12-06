﻿namespace Demo.WebApi.Application.Common.Models;

public class PaginationResponse<T>
{
    public PaginationResponse(List<T> data, int count, int page, int pageSize)
    {
        Data = data;
        Metadata = new PaginationMetadata
        {
            CurrentPage = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize),
            TotalCount = count
        };
    }

    public List<T> Data { get; set; }
    public PaginationMetadata Metadata { get; set; }

}

public class PaginationMetadata
{
    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }

    public int TotalCount { get; set; }

    public int PageSize { get; set; }

    public bool HasPreviousPage => CurrentPage > 1;

    public bool HasNextPage => CurrentPage < TotalPages;
}