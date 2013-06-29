namespace WinDriver.Dto
{
    public class WebDriverResponse
    {
        public string SessionId { get; set; }

        public int Status { get; set; }

        public object Value { get; set; }
    }
}