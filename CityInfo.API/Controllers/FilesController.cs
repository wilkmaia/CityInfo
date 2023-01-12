using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("/api/files")]
public class FilesController : ControllerBase
{
    private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

    public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
    {
        _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider ??
                                            throw new System.ArgumentNullException(
                                                nameof(fileExtensionContentTypeProvider));
    }
    
    [HttpGet("{fileId}")]
    public ActionResult GetFile(string fileId)
    {
        var pathToFile = @$"F:\Work\NearForm\csharp-upskilling\CSharpAdventOfCode\Day{fileId}\input";
        if (!System.IO.File.Exists(pathToFile))
        {
            return new NotFoundResult();
        }

        if (!_fileExtensionContentTypeProvider.TryGetContentType(pathToFile, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        var bytes = System.IO.File.ReadAllBytes(pathToFile);
        return PhysicalFile(pathToFile, contentType, Path.GetFileName(pathToFile));
    }
}