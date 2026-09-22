using System.ComponentModel.DataAnnotations;

namespace OrbitAOS.V6.Models.ViewModels
{
    /// <summary>
    /// Home page view model – Web layer.
    /// Replaces ViewBag/ViewData usage in legacy controllers.
    /// </summary>
    public class HomeViewModel
    {
        public string WelcomeMessage { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; }
        public string? UserName { get; set; }
    }
}
