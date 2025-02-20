namespace PublicCommon
{
    public static class DateTimeExtension
    {
        //todo must in prod add to utc ,so better ,then can change to anything easily
        public static DateTime CurrentTime => DateTime.Now;

        public static DateTime OldTime => new DateTime(1947, 8, 15);

        public const string StandardFormat = "yyyy-MM-ddTHH:mm:ss";//even on air safe & excludes '/'
        public static string CurrentTimeInString => DateTime.Now.ToString(StandardFormat);
    }
}