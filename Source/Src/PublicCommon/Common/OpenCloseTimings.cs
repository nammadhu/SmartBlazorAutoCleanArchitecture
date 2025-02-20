namespace PublicCommon.Common;

public class ConstantsOftimings
{
    public static readonly List<OpenCloseTiming> OpenCloseTimingsDefaultSingle = [
           new OpenCloseTiming(TimeSpan.FromHours(10),TimeSpan.FromHours(18))];

    public static readonly List<OpenCloseTiming> OpenCloseTimingsDouble = [
   new OpenCloseTiming(TimeSpan.FromHours(10),TimeSpan.FromHours(13)),
        new OpenCloseTiming(TimeSpan.FromHours(16),TimeSpan.FromHours(20))];

    public static readonly List<OpenCloseTimingsOfDay> TimingsUsualWeekDaysDefault = new List<OpenCloseTimingsOfDay>
        {
            new OpenCloseTimingsOfDay(DayOfWeek.Monday,OpenCloseTimingsDefaultSingle),
            new OpenCloseTimingsOfDay(DayOfWeek.Tuesday,OpenCloseTimingsDefaultSingle),
            new OpenCloseTimingsOfDay(DayOfWeek.Wednesday,OpenCloseTimingsDefaultSingle),
            new OpenCloseTimingsOfDay(DayOfWeek.Thursday,OpenCloseTimingsDefaultSingle),
            new OpenCloseTimingsOfDay(DayOfWeek.Friday,OpenCloseTimingsDefaultSingle),
            new OpenCloseTimingsOfDay(DayOfWeek.Saturday,[new OpenCloseTiming(TimeSpan.FromHours(10),TimeSpan.FromHours(14))]),
            new OpenCloseTimingsOfDay(DayOfWeek.Sunday)//holiday
        };
}
public class OpenCloseTiming
{
    public OpenCloseTiming()
    {
    }

    public OpenCloseTiming(TimeSpan? open, TimeSpan? close)
    {
        OpenTime = open;
        CloseTime = close;
    }

    public TimeSpan? OpenTime { get; set; } //= TimeSpan.FromHours(10) + TimeSpan.FromMinutes(30);//10.30am
    public TimeSpan? CloseTime { get; set; } //= TimeSpan.FromHours(18) + TimeSpan.FromMinutes(30);//6.30pm

    public static List<OpenCloseTiming>? DeSerializeOpenCloseTimings(string? openCloseTimingsJson, List<OpenCloseTimingsOfDay>? timingsUsual = null)
    {
        // return !string.IsNullOrEmpty(timingsTodayJson)?JsonSerializer.Deserialize<List<Timing>>(timingsTodayJson):null;

        //if (!string.IsNullOrEmpty(timingsTodayJson))
        //return  JsonSerializer.Deserialize<List<Timing>>(timingsTodayJson);
        if (JsonExtensions.TryDeserialize<List<OpenCloseTiming>>(openCloseTimingsJson, out List<OpenCloseTiming> result))
            return result;
        else return timingsUsual?.FirstOrDefault(t => t.Day == DateTime.Today.DayOfWeek)?.Timings;
    }

    public static List<OpenCloseTiming>? DeSerializeTimings(string? timingsTodayJson, string? timingsRegularJson = null)
    {
        // return !string.IsNullOrEmpty(timingsTodayJson)?JsonSerializer.Deserialize<List<Timing>>(timingsTodayJson):null;

        //if (!string.IsNullOrEmpty(timingsTodayJson))
        //return  JsonSerializer.Deserialize<List<Timing>>(timingsTodayJson);
        if (JsonExtensions.TryDeserialize<List<OpenCloseTiming>>(timingsTodayJson, out List<OpenCloseTiming> result))
            return result;
        else return OpenCloseTimingsOfDay.DeserializeTimingsUsual(timingsRegularJson)?
                .FirstOrDefault(t => t.Day == DateTime.Today.DayOfWeek)?.Timings;
    }

    public static string? SerializeTimings(List<OpenCloseTiming>? timings) => JsonExtensions.Serialize(timings);
}

public class OpenCloseTimingsOfDay
{
    public DayOfWeek Day { get; set; }
    public List<OpenCloseTiming>? Timings { get; set; }

    public OpenCloseTimingsOfDay()
    {
    }

    public OpenCloseTimingsOfDay(DayOfWeek day)
    {
        Day = day;
    }

    public OpenCloseTimingsOfDay(DayOfWeek day, List<OpenCloseTiming> timings)
    {
        Day = day; Timings = timings;
    }

    public static List<OpenCloseTimingsOfDay>? DeserializeTimingsUsual(string? timingsRegularJson)
    {
        //return !string.IsNullOrEmpty(timingsUsualJson) ? JsonSerializer.Deserialize<List<WeeklyTiming>>(timingsUsualJson) : null;
        return JsonExtensions.TryDeserialize<List<OpenCloseTimingsOfDay>?>(timingsRegularJson, out List<OpenCloseTimingsOfDay>? result) ? result : null;
    }

    public static string? SerializeTimingsUsual(List<OpenCloseTimingsOfDay>? timings) => JsonExtensions.Serialize(timings);
}


#region carddetail_razor_cs_code
    private void Add1MoreTimingForToday()
    {
        model.TimingsToday ??= new List<OpenCloseTiming>();
        if (model.TimingsToday.Count < ConstantsTown.MaxAllowedTimingsPerDay) // Limit to 4 sets
        {
            model.TimingsToday.Add(new OpenCloseTiming());
        }
    }

    private void Reduce1TimingForToday()
    {
        if (model.TimingsToday != null && model.TimingsToday.Count > 0)
            model.TimingsToday.RemoveAt(model.TimingsToday.Count - 1);
    }

    private void Reduce1TimingForWeekDay(DayOfWeek day)
    {
        if (model.TimingsUsual != null && model.TimingsUsual.FirstOrDefault(t => t.Day == day)?.Timings != null && model.TimingsUsual.FirstOrDefault(t => t.Day == day)?.Timings?.Count > 0)
            model.TimingsUsual.FirstOrDefault(t => t.Day == day)?.Timings?
                .RemoveAt(model.TimingsUsual.FirstOrDefault(t => t.Day == day)?.Timings?.Count ?? 0 - 1);
    }

    private void Add1MoreTimingForWeekDay(DayOfWeek day)
    {
        if (model.TimingsUsual != null)
        {
            var dayTimings = model.TimingsUsual.FirstOrDefault(t => t.Day == day)?.Timings;
            if (ListExtensions.HasData(dayTimings))
            {
                if (dayTimings!.Count < ConstantsTown.MaxAllowedTimingsPerDay) // Limit to 4 sets
                {
                    dayTimings.Add(new OpenCloseTiming());
                }
                else
                {//show alert message as cant add more than 4 time slot for a day
                }
            }
            else model.TimingsUsual.Add(new OpenCloseTimingsOfDay() { Day = day, Timings = new List<OpenCloseTiming>() { new OpenCloseTiming() } });
        }
        else
            model.TimingsUsual = [new OpenCloseTimingsOfDay() { Day = day, Timings = new List<OpenCloseTiming>() { new OpenCloseTiming() } }];
    }
#endregion carddetail_razor_cs_code


#region CardDetail.razor code begins here
     <!-- TimingsToday: Display-only list of timings -->
                @if(cardDetail.TimingsToday != null && ListExtensions.HasData(cardDetail.TimingsToday))
{
                    < MudStack Row = "true" >
                        < h5 > Today </ h5 >
                        @foreach(var timing in cardDetail.TimingsToday)
                            {
                            < p > @timing.OpenTime.TimeSpanConvertTo12Hr()=>@timing.CloseTime.TimeSpanConvertTo12Hr() </ p >
                            }
                    </ MudStack >
                    }
                else
                    {
                    <MudText>No Information of Today Timings.</MudText>
                    }

                < !--WeeklyTimings: Display - only list of weekly timings -->
                @if (cardDetail.TimingsUsual != null && ListExtensions.HasData(cardDetail.TimingsUsual))
                    {
                    <MudExpansionPanel Icon="@Icons.Material.Filled.ViewTimeline" Text="Usual Weekly Timings"
                                       Expanded=false>
                        @foreach (var weeklyTiming in cardDetail.TimingsUsual)
                            {
                            @if (weeklyTiming.Timings != null)
                                {
                                <div>
                                    <MudStack Row="true">
                                        <p>@weeklyTiming.Day</p>
                                        @foreach (var timing in weeklyTiming.Timings)
                                            {
                                            <p>@timing.OpenTime.TimeSpanConvertTo12Hr()=>@timing.CloseTime.TimeSpanConvertTo12Hr()  </p>
                                            }
                                    </ MudStack >
                                </ div >
                                }
                            }
                    </ MudExpansionPanel >
                    }
                else
{
                    < MudText > No Information of Usual Timings.</ MudText >
                    }
#endregion CardDetail.razor code


#region cardDetailEdit_razor_code
                        < !--TimingsToday: Editable list of timings -->
                        @if (model.TimingsToday != null)
                        {
                            <div id="TodayTimings">
                                <h5>Today Timings</h5>
                                <MudStack Row="true" Wrap="Wrap.Wrap">
                                    @for (int i = 0; i < model.TimingsToday.Count; i++)
                                    {
                                        < OpenCloseTime Timing = "model.TimingsToday[i]" />
                                        if (model.TimingsToday.Count < ConstantsTown.MaxAllowedTimingsPerDay)
    {
                                            < MudButton Variant = "Variant.Filled" Color = "Color.Primary"
                                                       OnClick = "Add1MoreTimingForToday" > +</ MudButton >
                                        }
    else
    {
                                            < MudButton Variant = "Variant.Filled" Color = "Color.Secondary"
                                                       OnClick = "Reduce1TimingForToday" > -</ MudButton >
                                        }
}

                                </ MudStack >
                            </ div >
                        }


                        < !--WeeklyTimings: Editable list of weekly timings -->
                        @if (model.TimingsUsual != null)
                        {
                            <div id="UsualTimings">
                                <MudExpansionPanel Icon="@Icons.Material.Filled.ViewTimeline" Text="Usual Weekly Timings" Expanded=false
                                                   Style="background-color: #F0F0F0; border: 1px solid #ccc; padding: 8px; cursor: pointer; font-weight: bold;">
                                    @for (int i = 0; i < model.TimingsUsual.Count; i++)
                                    {
    var weeklyTiming = model.TimingsUsual[i];
    weeklyTiming.Timings ??= CONSTANTS.OpenCloseTimingsDefaultSingle;
                                        < div >
                                            < p > @weeklyTiming.Day </ p >
                                            < MudStack Row = "true" Wrap = "Wrap.Wrap" >
                                                @if(weeklyTiming.Timings != null)
                                                {
        @for(int j = 0; j < weeklyTiming.Timings.Count; j++)
                                                    {
                                                        < OpenCloseTime Timing = "weeklyTiming.Timings[j]" PickerVariantType = "@PickerVariant.Inline" />

                                                        if (weeklyTiming.Timings.Count < ConstantsTown.MaxAllowedTimingsPerDay)
            {
                                                            < MudButton Variant = "Variant.Filled" Color = "Color.Primary"
                                                                       OnClick = "()=>Add1MoreTimingForWeekDay(weeklyTiming.Day)" > +</ MudButton >
                                                        }
            else
            {
                                                            < MudButton Variant = "Variant.Filled" Color = "Color.Secondary"
                                                                       OnClick = "()=>Reduce1TimingForWeekDay(weeklyTiming.Day)" > -</ MudButton >
                                                        }

        }
    }
                                            </ MudStack >
                                        </ div >
                                    }
                                </ MudExpansionPanel >
                            </ div >
                        }
#endregion cardDetailEdit_razor_code
