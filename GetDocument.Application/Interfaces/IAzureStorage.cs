
namespace Application.Interfaces;

public interface IAzureStorage
{
    Uri? GetServiceSasUriForContainer(string? storedPolicyName = null);
    Task<Uri?> GetServiceSasUriForContainerAsync(string? storedPolicyName = null);
    //Task<Uri> GetServiceSasUriForBlob(BlobClient blobClient, string storedPolicyName = null);

}
