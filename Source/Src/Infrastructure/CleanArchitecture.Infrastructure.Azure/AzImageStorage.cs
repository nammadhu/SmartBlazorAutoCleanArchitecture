using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BASE;
using BASE.Common;
using CleanArchitecture.Application.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Azure;

public class AzImageStorage(AppConfigurations appConfigurations, ILogger<AzImageStorage> logger) : IAzImageStorage
//: IAzImageStorage
    {
    private readonly string? connectionString = appConfigurations?.AppSettings?.ConnectionStrings.StorageAccountConnectionString;

    //icard/12/file1.jpeg
    //itown/1/file1.jpeg
    private readonly string? containerNameCard = appConfigurations?.AppSettings?.ConnectionStrings.ContainerNameCards;

    private readonly string? containerNameTown = appConfigurations?.AppSettings?.ConnectionStrings.ContainerNameTowns;

    //"DefaultEndpointsProtocol=https;AccountName=mytown;AccountKey=pls dont paste the secret key in code,this is just for format reference.here its referring from webapp connection strings;EndpointSuffix=core.windows.net";

    //for dev and prod different storage account but container and  files path all similar

    //Create card using this
    private async Task<List<ImageInfo>> UploadImagesToId(string containerName, string idPath, IEnumerable<ImageInfo> imagesList, CancellationToken cancellationToken)
        {
        logger.LogWarning($"Saving To Id:{idPath} images Upload({imagesList.Count()})");
        try
            {
            if (ListExtensions.IsEmptyList(imagesList)) return [];
            else if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(idPath))
                {
                logger.LogError($"COntainerName({containerName == null}) or idPath({idPath == null}) is null");
                return [];
                }
            else
                {
                if (!idPath.EndsWith("/")) idPath += "/";

                var imageInfoList = imagesList!.Where(x => ImageInfoBase64Url.IsBase64(x.ImageData?.Base64)).ToList();

                logger.LogWarning($"connectionString:{connectionString}");
                logger.LogWarning($"containerName:{containerName}");
                var container = new BlobContainerClient(connectionString, containerName); // Get a reference to a blob container in Azure Blob Storage

                // Get the number of blobs in the iCard's folder
                //todo may be this can be avoided & removed
                var blobs = container.GetBlobs(BlobTraits.None, BlobStates.None, $"{idPath}", cancellationToken);
                var count = blobs.Count();

                List<string> resultURLs = [];

                foreach (var imageInfo in imageInfoList)
                    {
                    if (!ImageInfoBase64Url.IsBase64(imageInfo?.ImageData?.Base64)) continue;//required for warnings
                    count++;
                    //todo extension .jpg why only
                    //todo image storage can be like number-filename.extension
                    //todo so always refer the number and proceed further can be
                    //var blob = container.GetBlobClient($"{cardId}/{count}.jpg"); // Create a new blob in the iCard's folder with a sequential number
                    var blob = container.GetBlobClient($"{idPath}{imageInfo!.ImageData!.GetFileNameToUpload(count)}");
                    var base64Data = imageInfo!.ImageData!.Base64![(imageInfo!.ImageData!.Base64!.IndexOf(',') + 1)..];
                    var bytes = Convert.FromBase64String(base64Data);
                    var stream = new MemoryStream(bytes);

                    await blob.UploadAsync(stream, overwrite: true, cancellationToken); // Upload the image to the blob

                    //default itself cool,so this separately not requried
                    //await blob.SetAccessTierAsync(AccessTier.Cool); // Set the blob's access tier to Cool
                    // Store the blob's URI in a database...
                    imageInfo.Url = blob.Uri.AbsoluteUri;
                    imageInfo.ImageData = null;
                    // Return the blob's URI
                    //Uri.AbsoluteUri  https://www.example.com/path/to/resource?query=string#fragment
                    //Uri.AbsolutePath /path/to/resource
                    }
                return imageInfoList; // Return the blob's URI
                }
            }
        catch (Exception e)
            {
            logger.LogError($"Saving Error for Id:{idPath} images Upload({imagesList.Count()}) with exception:{e.ToString()}");
            throw;
            }
        }

    public async Task<List<ImageInfo>> UploadImagesToTownId(int townId, IEnumerable<ImageInfo> imagesList, CancellationToken cancellationToken)
    => await UploadImagesToId(containerNameTown ?? "", townId + "/", imagesList, cancellationToken);

    public async Task<List<ImageInfo>> UploadImagesToCardId(int cardId, IEnumerable<ImageInfo> imagesList, CancellationToken cancellationToken)
            => await UploadImagesToId(containerNameCard ?? "", cardId + "/", imagesList, cancellationToken);

    public async Task<(List<ImageInfo> imagesUploaded, List<ImageInfo> imagesDeleted)> DeleteAndUploadImagesToCardId(int cardId, List<ImageInfo> imagesToUpload, List<ImageInfo> imagesToDelete, CancellationToken cancellationToken)
        => await DeleteAndUploadImagesToId(containerNameCard ?? "", cardId.ToString(), imagesToUpload, imagesToDelete, cancellationToken);

    public async Task<(List<ImageInfo> imagesUploaded, List<ImageInfo> imagesDeleted)> DeleteAndUploadImagesToTownId(int townId, List<ImageInfo> imagesToUpload, List<ImageInfo> imagesToDelete, CancellationToken cancellationToken)
        => await DeleteAndUploadImagesToId(containerNameTown ?? "", townId.ToString(), imagesToUpload, imagesToDelete, cancellationToken);

    //UpdateCard Card_Draft using this
    private async Task<(List<ImageInfo> imagesUploaded, List<ImageInfo> imagesDeleted)> DeleteAndUploadImagesToId(string containerName, string idPath, List<ImageInfo> imagesToUpload, List<ImageInfo> imagesToDelete, CancellationToken cancellationToken)
        {
        logger.LogWarning($"Saving To id:{idPath} images Upload({imagesToUpload.Count})/Delete({imagesToDelete.Count})");
        try
            {
            if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(idPath))
                {
                logger.LogError($"COntainerName({containerName == null}) or idPath({idPath == null}) is null");
                return ([], []);
                }

            if (ListExtensions.IsEmptyList(imagesToUpload) && ListExtensions.IsEmptyList(imagesToDelete)) return ([], []);
            else
                {
                if (!idPath.EndsWith("/")) idPath += "/";

                var container = new BlobContainerClient(connectionString, containerName);
                // Get a reference to a blob container in Azure Blob Storage
                // Get the number of blobs in the iCard's folder
                var blobResult = container.GetBlobs(BlobTraits.None, BlobStates.None, $"{idPath}", cancellationToken);

                List<BlobItem> blobs = [];
                int countExisitngBlobs;
                if (!ListExtensions.IsEmptyList(imagesToDelete))
                    {
                    blobs = blobResult.ToList();
                    countExisitngBlobs = blobs.Count();
                    }
                else countExisitngBlobs = blobResult.Count();

                List<string> resultURLs = [];

                //upload
                if (!ListExtensions.IsEmptyList(imagesToUpload))
                    {
                    imagesToUpload = imagesToUpload!.Where(x => ImageInfoBase64Url.IsBase64(x.ImageData?.Base64)).ToList();
                    foreach (var imageInfo in imagesToUpload)
                        {
                        if (!ImageInfoBase64Url.IsBase64(imageInfo?.ImageData?.Base64)) continue;//required for warnings
                        countExisitngBlobs++;

                        var blob = container.GetBlobClient($"{idPath}{imageInfo!.ImageData!.GetFileNameToUpload(countExisitngBlobs)}");
                        var base64Data = imageInfo!.ImageData!.Base64![(imageInfo!.ImageData!.Base64!.IndexOf(',') + 1)..];
                        var bytes = Convert.FromBase64String(base64Data);
                        var stream = new MemoryStream(bytes);

                        await blob.UploadAsync(stream, overwrite: true, cancellationToken); // Upload the image to the blob

                        //default itself cool,so this separately not required
                        //await blob.SetAccessTierAsync(AccessTier.Cool); // Set the blob's access tier to Cool
                        // Store the blob's URI in a database...
                        imageInfo.Url = blob.Uri.AbsoluteUri;
                        imageInfo.ImageData = null;
                        // Return the blob's URI
                        //Uri.AbsoluteUri  https://www.example.com/path/to/resource?query=string#fragment
                        //Uri.AbsolutePath /path/to/resource
                        }
                    }

                // Delete logic
                if (ListExtensions.HasData(imagesToDelete) && ListExtensions.HasData(blobs))
                    {
                    foreach (var imageToDelete in imagesToDelete)
                        {
                        if (imageToDelete == null) continue;
                        //fullpath: "https://mytown.blob.core.windows.net/icard/dev/21/21__BI__8myFileMadhu_0620112539.JPG"
                        //db having full path only
                        //blobs.Name will be like "dev/21/21__BI__8myFileMadhu_0620112539.JPG"

                        //imageToDelete.Url.Replace(container.Uri.AbsoluteUri + "/", string.Empty)
                        //dev/21/21__BI__8myFileMadhu_0620112539.JPG

                        //container.Uri.OriginalString & container.Uri.AbsoluteUri both has
                        //https://mytown.blob.core.windows.net/icard

                        string? blobName = imageToDelete.Url?.Replace(container.Uri.AbsoluteUri + "/", string.Empty);
                        if (string.IsNullOrEmpty(blobName)) continue;

                        if (blobName != null && blobs.Any(b => b.Name == blobName))
                            {
                            var blobToDelete = container.GetBlobClient($"{blobName}");
                            var response = await blobToDelete.DeleteAsync(cancellationToken: cancellationToken);
                            //response.Status 202 success,IsError false,ReasonPhrase:"Accepted"
                            imageToDelete.DeletedStatus = (response.Status == 202 && !response.IsError);
                            }
                        }
                    }
                return new(imagesToUpload, imagesToDelete);
                }
            }
        catch (Exception e)
            {
            logger.LogError($"Saving Error for Id:{idPath} images Upload({imagesToUpload.Count})/Delete({imagesToDelete.Count}) with exception:{e.ToString()}");
            throw;
            }
        }

    //cleanup of card using this
    public async Task<List<(string filename, bool result)>> DeleteWholeCardImagesFolder(int cardId, CancellationToken cancellationToken)
        => await DeleteWholeImagesFolder(containerNameCard ?? "", cardId.ToString(), cancellationToken);

    public async Task<List<(string filename, bool result)>> DeleteWholeTownImagesFolder(int townId, CancellationToken cancellationToken)
    => await DeleteWholeImagesFolder(containerNameTown ?? "", townId.ToString(), cancellationToken);

    private async Task<List<(string filename, bool result)>> DeleteWholeImagesFolder(string containerName, string idPath, CancellationToken cancellationToken)
        {
        if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(idPath))
            {
            logger.LogError($"COntainerName({containerName == null}) or idPath({idPath == null}) is null");
            return [];
            }
        var blobContainerClient = new BlobContainerClient(connectionString, containerName); // Get a reference to a blob container in Azure Blob Storage

        var blobItems = blobContainerClient.GetBlobs(prefix: idPath.TrimEnd('/'));
        List<(string filename, bool result)> result = [];
        foreach (BlobItem blobItem in blobItems)
            {
            //Console.WriteLine("deleting "+blobItem.Name);
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobItem.Name);
            result.Add((blobItem.Name, await blobClient.DeleteIfExistsAsync()));
            }
        return result;
        }

    public async Task<List<(string fileName, bool deletedResult)>?> DeleteFilesOfCardId(List<string?> fileFullPath, CancellationToken cancellationToken)
        => await DeleteFilesOfId(containerNameCard ?? "", fileFullPath, cancellationToken);

    public async Task<List<(string fileName, bool deletedResult)>?> DeleteFilesOfTownId(List<string?> fileFullPath, CancellationToken cancellationToken)
        => await DeleteFilesOfId(containerNameTown ?? "", fileFullPath, cancellationToken);

    private async Task<List<(string fileName, bool deletedResult)>?> DeleteFilesOfId(string containerName, List<string?> fileFullPath, CancellationToken cancellationToken)
        {
        if (string.IsNullOrEmpty(containerName))
            {
            logger.LogError($"COntainerName is null");
            return null;
            }
        if (fileFullPath.IsEmptyList()) return null;
        fileFullPath.RemoveAll(x => string.IsNullOrEmpty(x));

        List<(string fileName, bool deletedResult)> results = [];

        var container = new BlobContainerClient(connectionString, containerName); // Get a reference to a blob container in Azure Blob Storage
        foreach (var fileName in fileFullPath)
            {
            string? blobName = fileName?.Replace(container.Uri.AbsoluteUri + "/", string.Empty);
            if (!string.IsNullOrEmpty(blobName))
                {
                //actually we are having full path instead of only filename,so delete(filename) is enough
                var blob = container.GetBlobClient($"{blobName}"); // Get a reference to the blob
                results.Add(new(fileName!, (await blob.DeleteIfExistsAsync(cancellationToken: cancellationToken)).Value));
                // Delete the blob
                }
            }
        return results;
        }

    #region NotUsingMethodsOnlyForReference

    private async Task<string?> UploadImageToCardId(int cardId, string? base64Image, CancellationToken cancellationToken, string? fileName = null)
        {
        if (base64Image == null || !ImageInfoBase64Url.IsBase64(base64Image)) return null;
        var base64Data = base64Image[(base64Image.IndexOf(',') + 1)..];
        var bytes = Convert.FromBase64String(base64Data);
        var stream = new MemoryStream(bytes);
        var container = new BlobContainerClient(connectionString, containerNameCard); // Get a reference to a blob container in Azure Blob Storage

        // Get the number of blobs in the iCard's folder
        var blobs = container.GetBlobs(BlobTraits.None, BlobStates.None, $"{cardId}/");
        var count = blobs.Count();

        //same as ImageINfoBase64Url.GetFileNameToUpload
        var blob = container.GetBlobClient($"{cardId}/{fileName ?? $"{count + 1}.{CONSTANTS.DefaultImageExtension}"}"); // Create a new blob in the iCard's folder with a sequential number
        await blob.UploadAsync(stream, overwrite: true, cancellationToken); // Upload the image to the blob

        //byte default all is cool,so this line not seaprately requried
        //await blob.SetAccessTierAsync(AccessTier.Cool); // Set the blob's access tier to Cool
        // Store the blob's URI in a database...
        return blob.Uri.AbsoluteUri; // Return the blob's URI
                                     //Uri.AbsoluteUri  https://www.example.com/path/to/resource?query=string#fragment
                                     //Uri.AbsolutePath /path/to/resource
        }

    private async Task<List<string>> UploadImagesToCardId(int cardId, IEnumerable<ImageInfoBase64Url?> images, CancellationToken cancellationToken)
        {
        if (ListExtensions.IsEmptyList(images)) return [];
        else
            {
            images = images!.Where(x => ImageInfoBase64Url.IsBase64(x?.Base64)).ToList();
            var container = new BlobContainerClient(connectionString, containerNameCard); // Get a reference to a blob container in Azure Blob Storage

            // Get the number of blobs in the iCard's folder
            var blobs = container.GetBlobs(BlobTraits.None, BlobStates.None, $"{cardId}/", cancellationToken);
            var count = blobs.Count();

            List<string> resultURLs = [];

            foreach (var base64Image in images)
                {
                if (!ImageInfoBase64Url.IsBase64(base64Image)) continue;//required for warnings
                count++;
                //todo extension .jpg why only
                //todo image storage can be like number-filename.extension
                //todo so always refer the number and proceed further can be
                //var blob = container.GetBlobClient($"{cardId}/{count}.jpg"); // Create a new blob in the iCard's folder with a sequential number
                var blob = container.GetBlobClient($"{cardId}/{base64Image!.GetFileNameToUpload(count)}");
                var base64Data = base64Image!.Base64![(base64Image!.Base64!.IndexOf(',') + 1)..];
                var bytes = Convert.FromBase64String(base64Data);
                var stream = new MemoryStream(bytes);

                await blob.UploadAsync(stream, overwrite: true, cancellationToken); // Upload the image to the blob

                //default itself cool,so this separately not requried
                //await blob.SetAccessTierAsync(AccessTier.Cool); // Set the blob's access tier to Cool
                // Store the blob's URI in a database...
                resultURLs.Add(blob.Uri.AbsoluteUri);
                // Return the blob's URI
                //Uri.AbsoluteUri  https://www.example.com/path/to/resource?query=string#fragment
                //Uri.AbsolutePath /path/to/resource
                }
            return resultURLs; // Return the blob's URI
            }
        }

    private async Task<bool> DeleteFileOfCardId(int cardId, string fileName, CancellationToken cancellationToken)
        {
        var container = new BlobContainerClient(connectionString, containerNameCard); // Get a reference to a blob container in Azure Blob Storage
        var blob = container.GetBlobClient($"{cardId}/{fileName}"); // Get a reference to the blob
        return (await blob.DeleteIfExistsAsync(cancellationToken: cancellationToken)).Value; // Delete the blob
        }

    private async Task TierChangeToHot(int cardId, string fileName, DateTime date, CancellationToken cancellationToken)
        {
        var container = new BlobContainerClient(connectionString, containerNameCard); // Get a reference to a blob container in Azure Blob Storage
        var blob = container.GetBlobClient($"{cardId}/{fileName}"); // Get a reference to the blob
        if (DateTime.UtcNow.Date == date.Date)
            {
            await blob.SetAccessTierAsync(AccessTier.Hot, cancellationToken: cancellationToken); // Set the blob's access tier to Hot
            }
        else
            {
            await blob.SetAccessTierAsync(AccessTier.Cool, cancellationToken: cancellationToken); // Set the blob's access tier to Cool
            }
        }

    #endregion NotUsingMethodsOnlyForReference
    }
