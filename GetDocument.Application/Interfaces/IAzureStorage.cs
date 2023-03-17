using Azure.Storage.Blobs;
using Domain.Azure;
using System.ComponentModel;

namespace Application.Interfaces;

public interface IAzureStorage
{
    Task<Blob?> GetBlobAsync(string blobFilename);
    Task<BlobContainerClient> CreateContainerAsync();
    Task CopyBlobAsync(string blobFilename, string sourceContainer, string destinationContainer);
    Uri? GetServiceSasUriForContainer(string? storedPolicyName = null);
    Task<Uri?> GetServiceSasUriForContainerAsync(string? storedPolicyName = null);
    //Task<Uri> GetServiceSasUriForBlob(BlobClient blobClient, string storedPolicyName = null);

}
