using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Core2Walkthrough.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

/*
 *  About: Moderately complex page that: 
 *  -- gets enabled activities, 
 *  -- allows a user to activate an activity, 
 *  -- prevents user from clicking on the activity until the time is up
 *  -- increments user's XP.
 *  
 *  Make use of:
 *  -- complex DB queries
 *  -- Entity Framework updates
 *  -- Entity Framework deletes
 *  -- HttpContext.Session
 */
namespace Core2Walkthrough.Pages
{
    public class ActivityModel : PageModel
    {
        private readonly AppDbContext _db;

        [TempData]
        public string Message { get; set; }// no private set b/c we need data backl

        // no accessor as this just shows the information recieved before rendering the page.
        public IList<ACTIVITY> ActivityList { get; private set; }

        //Constructor
        public ActivityModel(AppDbContext db)
        {
            //Get the database context so we can move data to and from the tables.
            _db = db;
        }

        //When the page is being fetched, load some data to be rendered.
        public async Task OnGetAsync()
        {
            int userID = 0;
            int.TryParse(HttpContext.Session.GetString("id"), out userID);

            var con = _db.Database.GetDbConnection();
            try
            {
                await con.OpenAsync();
                using (var command = con.CreateCommand())
                {
                    string q = @" SELECT a.ID,
		                        a.TITLE,
		                        a.DESCRIPTION,
		                        a.XP,
		                        a.COOL_OFF_MINUTES,
		                        a.IS_ACTIVE,
		                        a.DATE_ENTERED
                         , IIF(DATEDIFF(SECOND, b.DATE_ENTERED, GETDATE()) < a.cool_off_minutes * 60, a.cool_off_minutes * 60 - DATEDIFF(SECOND, b.DATE_ENTERED, GETDATE()), 0) as TIME_LEFT
                         FROM ACTIVITY a  WITH(NOLOCK)
                         LEFT JOIN ACTIVITY_LOG b  WITH(NOLOCK) on a.ID = b.ID_ACTIVITY
                         WHERE b.ID_USER = @userid 
                            AND a.IS_ACTIVE = 1

                        UNION

                        SELECT a.ID,
	                        a.TITLE,
	                        a.DESCRIPTION,
	                        a.XP,
	                        a.COOL_OFF_MINUTES,
	                        a.IS_ACTIVE,
	                        a.DATE_ENTERED
	                        , 0 as TIME_LEFT
                        FROM ACTIVITY a WITH(NOLOCK)
                        WHERE a.IS_ACTIVE = 1 
                        and a.ID not in (select z.ID
	                        FROM ACTIVITY z WITH(NOLOCK)
	                        LEFT JOIN ACTIVITY_LOG b  WITH(NOLOCK) on a.ID = b.ID_ACTIVITY
	                        WHERE b.ID_USER = @userid )";
                    command.CommandText = q;

                    DbParameter tempParameter = command.CreateParameter();
                    tempParameter.ParameterName = "@userid";
                    tempParameter.Value = userID;
                    command.Parameters.Add(tempParameter);

                    System.Data.Common.DbDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ActivityList = new List<ACTIVITY>();

                        while (await reader.ReadAsync())
                        {
                            var row = new ACTIVITY
                            {
                                ID = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Description = reader.GetString(2),
                                XP = reader.GetInt32(3),
                                COOL_OFF_MINUTES = reader.GetInt32(4),
                                IS_ACTIVE = reader.GetBoolean(5),
                                DATE_ENTERED = reader.GetDateTime(6),
                                TIME_LEFT = reader.GetInt32(7)
                            };
                            ActivityList.Add(row);
                        }
                    }
                    reader.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                con.Dispose();
            }
        }

        //The user signals there is an action to do.  Notice how the words between "OnPost" and "Async" match up with the CHTML's "asp-page-handler"
        // the 'id' is from "asp-route-" on the CHTML.  Could have used asp-route-foo, but the parameter would need to be changed to 'int foo'.
        public async Task<IActionResult> OnPostActivityDoAsync(int id)
        {
            int userID = 0;

            if (HttpContext.Session.GetString("id") != null)
            {
                int.TryParse(HttpContext.Session.GetString("id"), out userID);

                // 1.  Update XP on user profile
                ACTIVITY tempActivity = await _db.ACTIVITY_DBSet.FindAsync(id);
                USERS tempUser = await _db.USER_DBSet.FindAsync(userID);

                tempUser.XP += tempActivity.XP;
                _db.USER_DBSet.Update(tempUser);
                _db.SaveChanges();


                // 2.  Upate log.
                ACTIVITY_LOG temp = new ACTIVITY_LOG();

                // If you got this far then the button is enabled so remove all instances of the old.
                var temp2 = _db.ACTIVITY_LOG_DBSet.Where(s => s.ID_ACTIVITY == id && s.ID_USER == userID);
                _db.ACTIVITY_LOG_DBSet.RemoveRange(temp2);
                _db.SaveChanges();

                temp.ID_ACTIVITY = id;

                temp.ID_USER = userID;
                temp.DATE_ENTERED = DateTime.Now;

                _db.ACTIVITY_LOG_DBSet.Add(temp);
                await _db.SaveChangesAsync();
            }

            Message = $"Activity started; XP added.";

            return RedirectToPage("/Activity");
        }
    }
}