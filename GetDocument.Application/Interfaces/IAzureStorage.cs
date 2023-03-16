using Domain.Azure;

namespace Application.Interfaces;

public interface IAzureStorage
{
    Task<Blob?> GetBlobAsync(string blobFilename);
    Uri? GetServiceSasUriForContainer(string? storedPolicyName = null);
    Task<Uri?> GetServiceSasUriForContainerAsync(string? storedPolicyName = null);
    //Task<Uri> GetServiceSasUriForBlob(BlobClient blobClient, string storedPolicyName = null);

}
