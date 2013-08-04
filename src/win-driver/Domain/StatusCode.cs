namespace WinDriver.Domain
{
    public static class StatusCode
    {
        public static readonly int Success = 0;
        public static readonly int NoSuchElement = 7;
        public static readonly int NoSuchFrame = 8;
        public static readonly int UnknownCommand = 9;
        public static readonly int StaleElementReference = 10;
        public static readonly int ElementNotVisible = 11;
        public static readonly int InvalidElementState = 12;
        public static readonly int UnknownError = 13;
        public static readonly int ElementIsNotSelectable = 15;
        public static readonly int JavaScriptError = 17;
        public static readonly int XPathLookupError = 19;
        public static readonly int Timeout = 21;
        public static readonly int NoSuchWindow = 23;
        public static readonly int InvalidCookieDomain = 24;
        public static readonly int UnableToSetCookie = 25;
        public static readonly int UnexpectedAlertOpen = 26;
        public static readonly int NoAlertOpenError = 27;
        public static readonly int ScriptTimeout = 28;
        public static readonly int InvalidElementCoordinates = 29;
        public static readonly int IMENotAvailable = 30;
        public static readonly int IMEEngineActivationFailed = 31;
        public static readonly int InvalidSelector = 32;
    }
}