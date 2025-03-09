using PublicCommon;
namespace Shared.Wrappers;
public class BaseResult
    {
    public bool Success { get; set; }
    public List<Error>? Errors { get; set; }
    public DateTime? ServerFetchedTime { get; set; } = DateTimeExtension.CurrentTime;
    public DateTime? ClientReceivedTime { get; set; } = DateTimeExtension.CurrentTime;
    public TimeSpan MinCacheTimeSpan { get; set; } = CONSTANTS.Default_MinCacheTimeSpan;// 6 hours
    public TimeSpan MaxCacheTimeSpan { get; set; } = CONSTANTS.Default_MaxCacheTimeSpan;//15 days
    // public bool SameAsPrevious { get; set; }//in case request sent with lastCacheSetTime,if same now then true

    public static BaseResult Ok()
        => new() { Success = true };

    public static BaseResult OkNoClientCaching()
        => new() { Success = true, MaxCacheTimeSpan = TimeSpan.Zero, MinCacheTimeSpan = TimeSpan.Zero };

    public static BaseResult Failure()
        => new() { Success = false, MaxCacheTimeSpan = TimeSpan.Zero, MinCacheTimeSpan = TimeSpan.Zero };

    public static BaseResult Failure(Error error)
        => new() { Success = false, Errors = [error], MaxCacheTimeSpan = TimeSpan.Zero, MinCacheTimeSpan = TimeSpan.Zero };

    public static BaseResult Failure(IEnumerable<Error> errors)
        => new() { Success = false, Errors = errors.ToList(), MaxCacheTimeSpan = TimeSpan.Zero, MinCacheTimeSpan = TimeSpan.Zero };

    public static implicit operator BaseResult(Error error)
        => new() { Success = false, Errors = [error], MaxCacheTimeSpan = TimeSpan.Zero, MinCacheTimeSpan = TimeSpan.Zero };

    public static implicit operator BaseResult(List<Error> errors)
        => new() { Success = false, Errors = errors, MaxCacheTimeSpan = TimeSpan.Zero, MinCacheTimeSpan = TimeSpan.Zero };

    public BaseResult AddError(Error error)
        {
        Errors ??= [];
        Errors.Add(error);
        Success = false;
        MaxCacheTimeSpan = TimeSpan.Zero;
        MinCacheTimeSpan = TimeSpan.Zero;
        return this;
        }
    }

public class BaseResult<TData> : BaseResult
    {
    public TData? Data { get; set; }

    public static BaseResult<TData> Ok(TData data)
        => new() { Success = true, Data = data };

    public static BaseResult<TData> OkNoClientCaching(TData data)//for create,update
    => new() { Success = true, Data = data, MaxCacheTimeSpan = TimeSpan.Zero, MinCacheTimeSpan = TimeSpan.Zero };

    public static BaseResult<TData> Ok(TData data, DateTime? serverFetchedTime)
        => new() { Success = true, Data = data, ServerFetchedTime = serverFetchedTime };

    public static BaseResult<TData> Ok(TData data, TimeSpan MinCacheTimeSpan)//for single GetById
  => new() { Success = true, Data = data, MinCacheTimeSpan = MinCacheTimeSpan, ServerFetchedTime = DateTimeExtension.CurrentTime };

    public static BaseResult<TData> Ok(TData data, DateTime? serverFetchedTime, TimeSpan MinCacheTimeSpan)
        => new() { Success = true, Data = data, ServerFetchedTime = serverFetchedTime, MinCacheTimeSpan = MinCacheTimeSpan };

    public new static BaseResult<TData> Failure()
        => new() { Success = false, MaxCacheTimeSpan = TimeSpan.Zero, MinCacheTimeSpan = TimeSpan.Zero };

    public new static BaseResult<TData> Failure(Error error)
        => new() { Success = false, Errors = [error], MaxCacheTimeSpan = TimeSpan.Zero, MinCacheTimeSpan = TimeSpan.Zero };

    public new static BaseResult<TData> Failure(IEnumerable<Error> errors)
        => new() { Success = false, Errors = errors.ToList(), MaxCacheTimeSpan = TimeSpan.Zero, MinCacheTimeSpan = TimeSpan.Zero };

    public static implicit operator BaseResult<TData>(TData data)
        => new() { Success = true, Data = data };

    public static implicit operator BaseResult<TData>(Error error)
        => new() { Success = false, Errors = [error], MaxCacheTimeSpan = TimeSpan.Zero, MinCacheTimeSpan = TimeSpan.Zero };

    public static implicit operator BaseResult<TData>(List<Error> errors)
        => new() { Success = false, Errors = errors, MaxCacheTimeSpan = TimeSpan.Zero, MinCacheTimeSpan = TimeSpan.Zero };
    }
