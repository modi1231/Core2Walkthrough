using Core2Walkthrough.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.Threading.Tasks;

/*
 *  About: Moderately complex page to verify if a user name is taken, and add if not.
 *  
 *  Make use of:
 *  -- basic DB binding
 *  -- Entity Framework querying
 *  -- routing parameter
 */
namespace Core2Walkthrough.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _db;

        [TempData]
        public string Message { get; set; }

        [BindProperty] // survive the journey _BACK_ from the user.
        public USERS UserData { get; set; }

        //Constructor
        public RegisterModel(AppDbContext db)
        {
            _db = db;
        }

        //User clicks a button.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            object temp = null;

            //1.  Check if that user name has been registered.
            var con = _db.Database.GetDbConnection();
            try
            {
                await con.OpenAsync();
                using (var command = con.CreateCommand())
                {
                    string q = @" SELECT 1 as MyResult
                                  FROM USERS with(nolock)
                                    WHERE Name = @userName ";
                    command.CommandText = q;

                    DbParameter tempParameter = command.CreateParameter();
                    tempParameter.ParameterName = "@userName";
                    tempParameter.Value = UserData.Name;
                    command.Parameters.Add(tempParameter);

                    temp = command.ExecuteScalar();

                    //2.  If exists then do not do add, and give a message back.
                    if (temp != null && (int)temp == 1)
                    {
                        Message = $"User {UserData.Name} name already exists.";
                        return Page();
                    }

                    //3.  If not used then do add.
                    UserData.DATE_ENTERED = DateTime.Now;

                    _db.USER_DBSet.Add(UserData);
                    await _db.SaveChangesAsync();

                    Message = $"User {UserData.Name} registered!";
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

            return RedirectToPage("/Index");
        }
    }
}