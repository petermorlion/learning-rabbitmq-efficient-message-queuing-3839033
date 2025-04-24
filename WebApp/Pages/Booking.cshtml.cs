using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExploreCalifornia.Pages
{
    public class BookingModel : PageModel
    {
        public string TourName { get; set; } = "";
        
        public void OnGet()
        {
            TourName = Request.Query["tourname"].ToString();
        }
    }
}
