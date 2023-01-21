using CityInfo.API.Services;

namespace CityInfo.Test;

public class PaginationMetadataTests
{
    private const int TotalItemCount = 100;
    private const int PageSize = 20;
    private const int CurrentPage = 1;

    [Fact]
    public void PaginationMetadata_ConstructNewObject_MetadataValuesMustMatchExpectations()
    {
        var paginationMetadata = new PaginationMetadata(TotalItemCount, PageSize, CurrentPage);
        
        Assert.Equal(1, paginationMetadata.CurrentPage);
        Assert.Equal(20, paginationMetadata.PageSize);
        Assert.Equal(100, paginationMetadata.TotalItemCount);
        Assert.Equal(5, paginationMetadata.TotalPageCount);
    }
}