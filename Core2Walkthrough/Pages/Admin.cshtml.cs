using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core2Walkthrough.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

/*
 *  About: Moderately complex page that: 
 *  -- adds activities, 
 *  -- disable/enable activities
 *  -- remove news
 *  -- add news
 *  -- remove users
 *  
 *  Make use of:
 *  -- Entity Framework updates
 *  -- Entity Framework deletes
 *  -- Entity Framework create
 *  -- HttpContext.Session
 */
namespace Core2Walkthrough.Pages
{
    public class AdminModel : PageModel
    {
        private readonly AppDbContext _db; // access route to and from the DB.

        [TempData]
        public string Message { get; set; }// no private set b/c we need data back

        [BindProperty] // survive the journey _BACK_ from the user.
        public NEWS NewsAdd { get; set; }// private not set because the data is needed back

        public IList<NEWS> NewsList { get; private set; }// private set so don't need to have data back.. just to show.

        public IList<USERS> UserList { get; private set; }// private set so don't need to have data back.. just to show.

        public IList<ACTIVITY> ActivityList { get; private set; }// private set so don't need to have data back.. just to show.

        [BindProperty]// survive the journey _BACK_ from the user.
        public ACTIVITY ActivityAdd { get; set; }

        //Constructor
        public AdminModel(AppDbContext db)
        {
            //Get the database context so we can move data to and from the tables.
            _db = db;
        }

        //When the page is being fetched, load some data to be rendered.
        public async Task OnGetAsync()
        {

            NewsList = await _db.NEWS_DBSet.AsNoTracking().ToListAsync();
            UserList = await _db.USER_DBSet.AsNoTracking().ToListAsync();
            ActivityList = await _db.ACTIVITY_DBSet.AsNoTracking().ToListAsync();
        }

        //The user signals there is an action to do.  Notice how the words between "OnPost" and "Async" match up with the CHTML's "asp-page-handler"
        public async Task<IActionResult> OnPostNewsAddAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //Snag the current date time.
            NewsAdd.DATE_ENTERED = DateTime.Now;
            
            //Add it to the News database set.
            _db.NEWS_DBSet.Add(NewsAdd);
            
            //Update it.
            await _db.SaveChangesAsync();

            //Inform the user of much success.
            Message = $"News added!";

            //Send it back to the admin page.
            return RedirectToPage("/Admin");
        }

        //The user signals there is an action to do.  Notice how the words between "OnPost" and "Async" match up with the CHTML's "asp-page-handler"
        public async Task<IActionResult> OnPostActivityAddAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            ActivityAdd.DATE_ENTERED = DateTime.Now;
            _db.ACTIVITY_DBSet.Add(ActivityAdd);
            await _db.SaveChangesAsync();

            Message = $"Activity added!";

            return RedirectToPage("/Admin");
        }

        //The user signals there is an action to do.  Notice how the words between "OnPost" and "Async" match up with the CHTML's "asp-page-handler"
        // the 'id' is from "asp-route-" on the CHTML.  Could have used asp-route-foo, but the parameter would need to be changed to 'int foo'.
        public async Task<IActionResult> OnPostActivityDisableAsync(int id)
        {
            //Find the data row first, before editing can happen.
            var temp = await _db.ACTIVITY_DBSet.FindAsync(id);

            if (temp != null)
            {
                temp.IS_ACTIVE = !temp.IS_ACTIVE;

                //make sure the database context knows there is something to update.
                _db.Attach(temp).State = EntityState.Modified;

                //Have it run the update.
                _db.ACTIVITY_DBSet.Update(temp);
                await _db.SaveChangesAsync();

            }

            Message = $"Activity updated";

            return RedirectToPage("/Admin");
        }

        //The user signals there is an action to do.  Notice how the words between "OnPost" and "Async" match up with the CHTML's "asp-page-handler"
        // the 'id' is from "asp-route-" on the CHTML.  Could have used asp-route-foo, but the parameter would need to be changed to 'int foo'.
        public async Task<IActionResult> OnPostDeleteNewsAsync(int id)
        {
            var temp = await _db.NEWS_DBSet.FindAsync(id);

            if (temp != null)
            {
                _db.NEWS_DBSet.Remove(temp);
                await _db.SaveChangesAsync();

            }

            Message = $"News deleted.";


            return RedirectToPage();
        }

        //The user signals there is an action to do.  Notice how the words between "OnPost" and "Async" match up with the CHTML's "asp-page-handler"
        // the 'id' is from "asp-route-" on the CHTML.  Could have used asp-route-foo, but the parameter would need to be changed to 'int foo'.
        public async Task<IActionResult> OnPostDeleteUserAsync(int id)
        {
            //Find the record to remove.
            var temp = await _db.USER_DBSet.FindAsync(id);

            if (temp != null)
            {
                //Tell the DB context to remove it.
                _db.USER_DBSet.Remove(temp);
                //Save it.
                await _db.SaveChangesAsync();
            }

            Message = $"User deleted.";

            return RedirectToPage();
        }
    }
}