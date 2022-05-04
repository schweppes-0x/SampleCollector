using System;
using System.Threading.Tasks;
using SampleCollector.Models;

namespace SampleCollector.Interfaces
{
    public interface ISampleCollector
    {
        Task ProcessMessageAsync(string key, BinaryData binaryData);
        Task ProcessMessageAsync(BinaryData binaryData, CollectorOptions options);

        Task ProcessMessageAsync(string key, string content);

    }
}