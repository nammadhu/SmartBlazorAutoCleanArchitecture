namespace CleanArchitecture.Application.Features.Towns.Commands;

public class CU_TownCommandHandler(ITownRepository repository, IUnitOfWork unitOfWork,
    ITranslator translator, IMapper mapper, IAzImageStorage azImageStorage, ServerCachingServiceTowns _cachingServiceTown, ILogger<CU_TownCommandHandler> logger) : IRequestHandler<CU_TownCommand, BaseResult<TownDto>>
    {
    public async Task<BaseResult<TownDto>> Handle(CU_TownCommand request, CancellationToken cancellationToken)
        {
        try
            {
            if (request.Id > 0)//UPDATE
                {
                var exData = await repository.GetByIdAsync(request.Id, cancellationToken);
                if (exData is null)
                    {
                    return new Error(ErrorCode.NotFound, $"Cant update Non-Existing town with id {request.Id}", nameof(request.Id));
                    }
                var townToUpdate = exData.CloneBySerializing();
                if (townToUpdate == null) throw new Exception("townToUpdate Clone Failed");
                var isDuplicateResult = await NameExists(repository, translator, request, exData, cancellationToken);
                if (isDuplicateResult != null) return isDuplicateResult;

                //todo must modify all specific properties here with validation on validator
                townToUpdate = mapper.Map(request, townToUpdate);
                townToUpdate.Id = request.Id;

                List<ImageInfo> listToUpload = new();
                List<ImageInfo> listToDelete = new();
                //main images(only),had to check difference
                TownImagesUpdate(request, exData, townToUpdate, ConstantsTown.TownMainImagesPrefix, listToUpload, listToDelete);
                if (ListExtensions.HasData(listToUpload) || ListExtensions.HasData(listToDelete))
                    await ImageUploadOrDelete(townToUpdate, ConstantsTown.TownMainImagesPrefix, listToUpload, listToDelete, cancellationToken);

                repository.Update(townToUpdate);
                var success = await unitOfWork.SaveChangesAsync(cancellationToken);
                if (success)
                    {
                    var data = mapper.Map<TownDto>(townToUpdate);
                    _cachingServiceTown.AddOrUpdateTownInTowns(data);
                    //return data;
                    return BaseResult<TownDto>.OkNoClientCaching(data);
                    }
                return new Error(ErrorCode.Exception, $"Some issue in {nameof(CU_TownCommandHandler)}-UpdateCard");
                }
            else//create
                {
                var isDuplicateResult = await NameExists(repository, translator, request, null, cancellationToken);
                if (isDuplicateResult != null) return isDuplicateResult;

                var obj = mapper.Map<Town>(request);
                //var obj = request.To<CreateUpdateTownCommand, Town>();
                //todo should modify above
                //var product = new Town(request.Name, request.Price, request.BarCode);

                //obj.IdTown = idNextGenerator.GetNextID();//in case of id disabling
                var result = await repository.AddAsync(obj, cancellationToken);
                var success = await unitOfWork.SaveChangesAsync(cancellationToken);

                //images upload to blob if exists and update to db
                if (CU_TownCommand.IsNewBase64Valid(request))
                    {
                    var uploadedImageURLs = await azImageStorage.UploadImagesToTownId(result.Id,
                                request.ToUploadImages(), cancellationToken);
                    if (uploadedImageURLs.HasData())
                        {
                        foreach (var imageInfo in uploadedImageURLs)
                            {
                            if (imageInfo?.ImageName != null)
                                result.GetType().GetProperty(imageInfo.ImageName)?.SetValue(result,
                                    uploadedImageURLs.FirstOrDefault(x => x.ImageName == imageInfo.ImageName)?.Url
                                    ); // Get uploaded URL
                            }
                        repository.Update(result);
                        var imagePathUpdate = await unitOfWork.SaveChangesAsync(cancellationToken);
                        }
                    }

                if (success)
                    {
                    var data = mapper.Map<TownDto>(result);
                    _cachingServiceTown.AddOrUpdateTownInTowns(data);
                    //return data;
                    return BaseResult<TownDto>.OkNoClientCaching(data);
                    //return new BaseResult<TownDto>() { Success = success, Data = mapper.Map<TownDto>(existingData) };
                    }
                return new Error(ErrorCode.Exception, $"Some issue in {nameof(CU_TownCommandHandler)}-CreateCardWithData");
                }
            }
        catch (Exception ex)
            {
            logger.LogError(ex, "Failed");
            return new Error(ErrorCode.Exception, translator.GetString("Exception in Town Creation/UpdateCard"));
            }
        }

    private static async Task<BaseResult<TownDto>?> NameExists(ITownRepository repository, ITranslator translator, CU_TownCommand request, Town? existingData, CancellationToken cancellationToken)
        {
        if (existingData is null && request.Id > 0)//existing should have matching record
            {
            return new Error(ErrorCode.ModelIsNull, "Existing town should have matching record", nameof(request.Id));
            }
        if ((existingData == null || !request.Title.Equals(existingData.Title, StringComparison.CurrentCultureIgnoreCase))
        && await repository.IsNameExistsAsync(request.Title, cancellationToken))
        //if different name trying then make sure other data of same name not exists
            {
            return new Error(ErrorCode.NotFound, translator.GetString($"Name({request.Title}) Already exists"), nameof(request.Title));//this should be duplicated data error
            }
        return null;
        }

    private void TownImagesUpdate(CU_TownCommand request, Town existingTown, Town existingtoUpdate, string townMainImagesPrefix, List<ImageInfo> listToUpload, List<ImageInfo> listToDelete)
        {
        if (CU_TownCommand.IsNotImageUrlNorBase64(request) && CU_TownCommand.IsNotImageUrls(existingTown))
            {
            //nothing just ignore,no difference
            }
        else if (CU_TownCommand.IsNotImageUrlNorBase64(request) && !CU_TownCommand.IsNotImageUrls(existingTown))
            {
            //means now all null, existing has some data
            //on first screen CU component ,might not send detail page
            //but details page would send this
            //so if whole component didnt come then dont do anything,leave as it is by copying earlier to now
            existingtoUpdate.Image1 = existingTown.Image1;
            existingtoUpdate.Image2 = existingTown.Image2;
            }
        else if (!(CU_TownCommand.IsNotImageUrlNorBase64(request) && CU_TownCommand.IsNotImageUrls(existingTown)))
            {//todo also one more like both are equal then also skip
            var (listToUploadBrand, listToDeleteBrand) = request.GetUploadAndDeleteImages(existingTown);
            if (ListExtensions.HasData(listToUploadBrand))
                {
                listToUploadBrand.ForEach(x => x.ImageName = townMainImagesPrefix + x.ImageName ?? "");
                listToUpload.AddRange(listToUploadBrand);
                }
            if (ListExtensions.HasData(listToDeleteBrand))
                {
                listToDeleteBrand.ForEach(x => x.ImageName = townMainImagesPrefix + x.ImageName ?? "");
                listToDelete.AddRange(listToDeleteBrand);
                }
            }
        }

    private async Task ImageUploadOrDelete(Town exTownToUpdate, string brandPrefix, List<ImageInfo> listToUpload, List<ImageInfo> listToDelete, CancellationToken cancellationToken)
        {
        if (ListExtensions.HasData(listToUpload) || ListExtensions.HasData(listToDelete))
            {
            (List<ImageInfo> imagesUploaded, List<ImageInfo> imagesDeleted) = await azImageStorage.DeleteAndUploadImagesToTownId(exTownToUpdate.Id, listToUpload, listToDelete, cancellationToken);

            var brandImagesUploadedResult = imagesUploaded?.Where(x => x?.ImageName != null && x.ImageName.StartsWith(brandPrefix))?.ToList();

            var brandImagesDeletedResult = imagesDeleted?.Where(x => x?.ImageName != null && x.ImageName.StartsWith(brandPrefix))?.ToList();
            //delete first then upload entry,so replacement all handled properly
            //brand objects first
            if (brandImagesDeletedResult != null && ListExtensions.HasData(brandImagesDeletedResult))
                foreach (var imageInfo in brandImagesDeletedResult)
                    {
                    imageInfo.ImageName = imageInfo.ImageName?.Replace(brandPrefix, string.Empty);
                    ImageInfo matched = brandImagesDeletedResult.FirstOrDefault(x => x.ImageName == imageInfo.ImageName);
                    if (matched != null && matched.DeletedStatus == true && imageInfo.ImageName != null)
                        exTownToUpdate.GetType().GetProperty(imageInfo.ImageName)?.SetValue(exTownToUpdate, null);
                    }
            if (brandImagesUploadedResult != null && ListExtensions.HasData(brandImagesUploadedResult))
                foreach (var imageInfo in brandImagesUploadedResult)
                    {
                    imageInfo.ImageName = imageInfo.ImageName?.Replace(brandPrefix, string.Empty);
                    ImageInfo matched = brandImagesUploadedResult.FirstOrDefault(x => x.ImageName == imageInfo.ImageName);
                    if (matched != null && imageInfo.ImageName != null)
                        exTownToUpdate.GetType().GetProperty(imageInfo.ImageName)?.SetValue(exTownToUpdate, matched?.Url);
                    }
            }
        }
    }
