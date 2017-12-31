using Core2Walkthrough.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

/*
 *  About: Basic page to display a single user's information.
 *  
 *  Make use of:
 *  -- basic DB binding
 *  -- Entity Framework querying
 *  -- routing parameter
 */
namespace Core2Walkthrough.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly AppDbContext _db;

        [TempData]
        public string Message { get; set; }

        [BindProperty]  // survive the journey _BACK_ from the user.
        public USERS UserData { get; set; }

        //Constructor
        public ProfileModel(AppDbContext db)
        {
            //Get the database context so we can move data to and from the tables.
            _db = db;
        }
        
        public void OnGet(int id)
        {
            var tempUser = _db.USER_DBSet.Find(id);
            UserData = tempUser;
        }        
    }
}