using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core2Walkthrough.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

/*
 *  About: very basic page - the list of users and their XP.
 *  -- link to the user's profiles in the page.
 */
namespace Core2Walkthrough.Pages
{
    public class LeaderboardModel : PageModel
    {
        private readonly AppDbContext _db;

        public IList<USERS> UserList { get; private set; }

        public LeaderboardModel(AppDbContext db)
        {
            _db = db;
        }

        public async Task OnGetAsync()
        {
            UserList = await (from a in _db.USER_DBSet
                       orderby a.XP descending, a.Name
                       select a).ToListAsync();
        }

    }
}