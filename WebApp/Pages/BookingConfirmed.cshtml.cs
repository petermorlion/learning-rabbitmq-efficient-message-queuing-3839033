using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExploreCalifornia.Pages
{
    public class BookingConfirmedModel : PageModel
    {
        public string TourName { get; set; } = "";
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";

        public void OnGet()
        {
            TourName = Request.Query["tourname"].ToString();
            Name = Request.Query["name"].ToString();
            Email = Request.Query["email"].ToString();
        }
    }
}
