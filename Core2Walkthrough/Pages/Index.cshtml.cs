using System.Collections.Generic;
using System.Threading.Tasks;
using Core2Walkthrough.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

/*
 *  About: Starting landing page when a user would visit the site.
 *  2017.12.30 - created
 *  -- display any news
 *  -- display static text.
 *  -- house the log out button.
 *  
 *  Make use of:
 *  -- basic DB binding
 *  -- Entity Framework retrieves
 *  -- HttpContext.Session
 */
namespace Core2Walkthrough.Pages
{
    /*
     * http://www.entityframeworktutorial.net/stored-procedure-in-entity-framework.aspx
     * http://www.entityframeworktutorial.net/Querying-with-EDM.aspx
     * */


   
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;

        public IList<USERS> UserDatas { get; private set; }// private set so don't need to have data back.. just to show.
        public IList<NEWS> NewsList { get; private set; }// private set so don't need to have data back.. just to show.

        [TempData]
        public string Message { get; set; }// no private set b/c we need data back



        //Constructor
        public IndexModel(AppDbContext db)
        {
            //Get the database context so we can move data to and from the tables.
            _db = db;

 
        }

        //When the page is being fetched, load some data to be rendered.
        public async Task OnGetAsync()
        {
            UserDatas = await _db.USER_DBSet.AsNoTracking().ToListAsync();
            NewsList = await _db.NEWS_DBSet.AsNoTracking().ToListAsync();
        }

        public IActionResult OnPostLogOut()
        {
            HttpContext.Session.Clear();
            Message = "Logged out";

            return RedirectToPage();
        }

 

    }
}
