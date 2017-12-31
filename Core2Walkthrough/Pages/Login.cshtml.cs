using System.Linq;
using Core2Walkthrough.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

/*
 *  About: Basic page to check for a user name and log in.
 *  -- display any news
 *  -- display static text.
 *  -- house the log out button.
 *  
 *  Make use of:
 *  -- basic DB binding
 *  -- Entity Framework querying
 *  -- HttpContext.Session
 */
namespace Core2Walkthrough.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _db;

        [TempData]
        public string Message { get; set; }// no private set b/c we need data back

        [BindProperty] // survive the journey _BACK_ from the user.
        public USERS UserData { get; set; }

        //Constructor
        public LoginModel(AppDbContext db)
        {
            _db = db;
        }

        //User clicks a button.
        public IActionResult OnPost()
        {
            //http://www.entityframeworktutorial.net/Querying-with-EDM.aspx

            // 1.  Using the user name, see if a record can be found.
            var user = from a in _db.USER_DBSet
                       where a.Name == UserData.Name
                       select a;

            USERS tempUser = user.FirstOrDefault<USERS>();

            HttpContext.Session.Clear();

            if (tempUser == null)
            {
                Message = "No User Found";
            }
            else
            {
                // 2.  User is found so stash the name, id, and 'is admin' flag in the session.
                HttpContext.Session.SetString("name", tempUser.Name);
                HttpContext.Session.SetString("id", tempUser.ID.ToString());

                if (tempUser.IS_ADMIN)
                    HttpContext.Session.SetString("is_admin", "1");

                // 3.  Give a personalized welcome message.
                Message = "Hello " + tempUser.Name + "!";
                
                // 4.  To the index they go.. all logged in.
                return RedirectToPage("/Index");
            }

            return RedirectToPage();
        }
    }
}