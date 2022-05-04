using Microsoft.AspNetCore.StaticFiles;
using SampleCollector.Enums;

namespace SampleCollector.Extensions
{
    public static class FileExtensions
    {
        private static readonly FileExtensionContentTypeProvider Provider = new FileExtensionContentTypeProvider();

        public static string GetContentType(this string fileName)
        {
            if (!Provider.TryGetContentType(fileName, out var contentType))
                contentType = "application/octet-stream";
            
            return contentType;
        }

        public static FileType DetermineFileType(string content)
        {
            content = content.Trim();
            if (content.StartsWith("{") || content.StartsWith("["))
            {
                return FileType.JSON;
            }
            else if (content.StartsWith("<?"))
            {
                return FileType.XML;
            }
            else if (content.StartsWith("<"))
            {
                return FileType.HTML;
            }
            else
            {
                return FileType.CSV;
            }
        }
    }
}