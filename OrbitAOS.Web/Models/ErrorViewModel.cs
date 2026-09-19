namespace OrbitAOS.Web.Models
{
    /// <summary>
    /// View model for the Error view.
    /// </summary>
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
