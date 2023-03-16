using Application.Interfaces;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class AzureStorage : IAzureStorage
{
    #region Dependency Injection / Constructor
    private readonly BlobContainerClient _blobContainerClient;
    private readonly ILogger<AzureStorage> _logger;
    public AzureStorage(BlobContainerClient blobContainerClient, ILogger<AzureStorage> logger)
    {
        _blobContainerClient = blobContainerClient;
        _logger = logger;
    }

    #endregion

    #region Generate SasUri
    // TODO: make private method
    public Uri? GetServiceSasUriForContainer(string? storedPolicyName = null)
    {
        Uri? sasUri = null;
        // Check and create container if not exist in azure storage.
        _blobContainerClient.CreateIfNotExistsAsync();

        // Check whether this BlobContainerClient object has been authorized with Shared Key.
        if (_blobContainerClient.CanGenerateSasUri)
        {
            // Create a SAS token that's valid for one hour.
            BlobSasBuilder sasBuilder = new()
            {
                // Specify the container name.                
                BlobContainerName = _blobContainerClient.Name,
                // The Resource ="c" means Create a service SAS for a blob container.
                Resource = "c"
            };

            // If no stored access policy is specified, create the policy
            // by specifying StartsOn, ExpiresOn and permissions.
            if (storedPolicyName == null)
            {
                sasBuilder.StartsOn = DateTimeOffset.UtcNow;
                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(5);
                //sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read | BlobContainerSasPermissions.Write);
                sasBuilder.SetPermissions(BlobSasPermissions.Tag | BlobSasPermissions.Read | BlobSasPermissions.Write);
            }
            else
            {
                sasBuilder.Identifier = storedPolicyName;
            }
            // Get the SAS URI for the specified container.
             sasUri = _blobContainerClient.GenerateSasUri(sasBuilder);

            // Return the SAS URI for blob container.
            return sasUri;
        }
        else
        {
            return sasUri;
        }
    }


    public async Task<Uri?> GetServiceSasUriForContainerAsync(string? storedPolicyName = null)
    {
        // Check and create container if not exist in azure storage.
        await _blobContainerClient.CreateIfNotExistsAsync();

        // Check whether this BlobContainerClient object has been authorized with Shared Key.
        if (_blobContainerClient.CanGenerateSasUri)
        {
            // Create a SAS token that's valid for one hour.
            BlobSasBuilder sasBuilder = new()
            {
                // Specify the container name.                
                BlobContainerName = _blobContainerClient.Name,
                // The Resource ="c" means Create a service SAS for a blob container.
                Resource = "c"
            };

            // If no stored access policy is specified, create the policy
            // by specifying StartsOn, ExpiresOn and permissions.
            if (storedPolicyName == null)
            {
                sasBuilder.StartsOn = DateTimeOffset.UtcNow;
                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(5);
                //sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read | BlobContainerSasPermissions.Write);
                sasBuilder.SetPermissions(BlobSasPermissions.Tag | BlobSasPermissions.Read | BlobSasPermissions.Write);
            }
            else
            {
                sasBuilder.Identifier = storedPolicyName;
            }
            // Get the SAS URI for the specified container.
            Uri sasUri = _blobContainerClient.GenerateSasUri(sasBuilder);

            // Return the SAS URI for blob container.
            return sasUri;
        }
        else
        {
            return null;
        }
    }
   
    #endregion
}
