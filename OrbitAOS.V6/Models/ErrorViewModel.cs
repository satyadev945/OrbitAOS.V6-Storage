namespace OrbitAOS.V6.Models
{
    /// <summary>
    /// Error view model – Web layer.
    /// Used by the Error action and Error.cshtml view.
    /// </summary>
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
