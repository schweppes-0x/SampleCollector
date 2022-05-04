using System;
using SampleCollector.Enums;
using SampleCollector.Extensions;

namespace SampleCollector.Models
{
    public class SampleMessage
    {
        /// <summary>
        /// Unique identifier, this is used to generate the filename
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the collector which the message belongs to
        /// </summary>
        public string CollectorName { get; set; }

        /// <summary>
        /// The binarydata which the file contains
        /// </summary>
        public BinaryData MessageContent { get; set; }

        /// <summary>
        /// The specific filetype (xml,json,csv) which is determined by the content of the file
        /// </summary>
        public FileType FileType
        {
            get
            {
                return FileExtensions.DetermineFileType(MessageContent.ToString());
            }
        }

        /// <summary>
        /// The name of the file which will be uploaded to the AzureStorage. This is determined by multiple factors like the current year,month,day, hour and the generated GUID.
        /// </summary>
        public string FileName => $"{CollectorName}/{DateTimeOffset.UtcNow.Year}/{DateTimeOffset.UtcNow.Month}/{DateTimeOffset.UtcNow.Day}/{DateTimeOffset.UtcNow.Hour}/{Id}.{FileType.ToString().ToLower()}";
    }
}