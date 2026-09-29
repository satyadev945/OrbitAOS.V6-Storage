namespace OrbitAOS.Web.Models
{
    /// <summary>
    /// View model for the Error page, carrying the request identifier for diagnostics.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>Gets or sets the current request or activity identifier.</summary>
        public string? RequestId { get; set; }

        /// <summary>Gets a value indicating whether the request ID should be displayed.</summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
