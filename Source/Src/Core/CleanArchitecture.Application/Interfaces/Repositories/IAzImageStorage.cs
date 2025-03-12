namespace CleanArchitecture.Application.Interfaces.Repositories;

public interface IAzImageStorage
    {
    Task<(List<ImageInfo> imagesUploaded, List<ImageInfo> imagesDeleted)> DeleteAndUploadImagesToCardId(int cardId, List<ImageInfo> imagesToUpload, List<ImageInfo> imagesToDelete, CancellationToken cancellationToken);

    Task<(List<ImageInfo> imagesUploaded, List<ImageInfo> imagesDeleted)> DeleteAndUploadImagesToTownId(int townId, List<ImageInfo> imagesToUpload, List<ImageInfo> imagesToDelete, CancellationToken cancellationToken);

    Task<List<(string filename, bool result)>> DeleteWholeCardImagesFolder(int cardId, CancellationToken cancellationToken);

    Task<List<(string filename, bool result)>> DeleteWholeTownImagesFolder(int townId, CancellationToken cancellationToken);

    Task<List<ImageInfo>> UploadImagesToCardId(int cardId, IEnumerable<ImageInfo> imagesList, CancellationToken cancellationToken);

    Task<List<ImageInfo>> UploadImagesToTownId(int townId, IEnumerable<ImageInfo> imagesList, CancellationToken cancellationToken);

    Task<List<(string fileName, bool deletedResult)>?> DeleteFilesOfCardId(List<string> fileFullPath, CancellationToken cancellationToken);

    Task<List<(string fileName, bool deletedResult)>?> DeleteFilesOfTownId(List<string> fileFullPath, CancellationToken cancellationToken);

    //NotUsingMethodsOnlyForReference
    //Task<bool> DeleteFileOfCardId(int cardId, string fileName, CancellationToken cancellationToken);
    //Task<List<(string fileName, bool deletedResult)>?> DeleteFilesOfCardId(int cardId, List<string?> fileNames, CancellationToken cancellationToken);

    //Task TierChangeToHot(int cardId, string fileName, DateTime date, CancellationToken cancellationToken);
    //Task<List<string>> UploadImagesToCardId(int cardId, IEnumerable<ImageInfoBase64Url?> images, CancellationToken cancellationToken);

    //Task<string?> UploadImageToCardId(int cardId, string? base64Image, CancellationToken cancellationToken, string? fileName = null);
    }
