# Core2Walkthrough

Follow up to Core2Walkthrough_Basic

For: DIC Tutorial project to walk through the more advanced topics of Core 2 ASP.NET

Dream.In.Code.NET tutorial on Core2 ASP.NET Advanced.


=================

dreamincode.net tutorial backup ahead of decommissioning






 Post icon  Posted 06 January 2018 - 01:40 PM 
 
  ASP.NET - Razor Pages and Core 2. Advanced. 1 of 2 
  
  [u][b]Requirements:[/u][/b]
Visual Studios 2017 Community or higher.
More than a passing familiarity with C# and SQL

[u][b]Github:[/u][/b]
https://github.com/modi1231/Core2Walkthrough

[url="http://www.dreamincode.net/forums/topic/408395-aspnet-razor-pages-core-2-and-entity-framework-basics/"] ASP.NET - Razor Pages, Core 2, and Entity Framework. Basics. [/url]
[url="http://www.dreamincode.net/forums/topic/408586-aspnet-razor-pages-and-core-2-advanced-2-of-2/"]ASP.NET - Razor Pages and Core 2. Advanced. 2 of 2 [/url]

Welcome to a further exploration of Core 2 ASP.NET with Razor pages.  This takes the basic setup, from the previous tutorial, and adds a host of new functionality to make a fairly complex site that hits on most of the bases.  

Please start with the 'Basic' tutorial for a further explanation on Razor pages, Core 2, and what the project will look like.

The sign post for what will be expanded is as follows:
- Actual DB implementation
- Pulling 'news' on a page load
- Creating a basic registration system
- Allowing log in
- Conditionally showing certain information based on logged in or not and roles.
- Sessions
- Multiple 'post's on a page, and even one with giving a parameter
- Custom queries that go beyond a simple SELECT ALL.
- Timed actions from the user
- A leader board for ranking.

The idea is to make a boiled down web game.  Folk log in, can initiate some action for points, and have to wait until X minutes to be able to do it again.  In the mean time there is 'administrative' functionality to add new activities, news, remove users, etc.

Essentially this should provide you with the scaffolding for a fully functional website.

This is a large area to cover so I will hit interesting areas and expect folk to reference the github repo for the full code.

Given the previous project - we left off with an in memory database, but we could shuffle data to and from the table using a class as the database outline.  Let's look to change that.

[u]Things to remember: [/u]
- DB context is your data henchmen and wing man.  If there's the need to move data to a database or what not make sure he's there with you.
- A given page's "model" is any set of properties and variables in its code behind.
- If you want to see any user input, and have it survive the journey, flag the property or variable as [BindProperty]

[u][b]Tutorial[/b][/u]

[b]1. [/b]  Persistent database data.
To start let's make an actual database for persistent data.  It turns out to be a snap to do this and allows a plethora of new functions.

Go to View -> SQL Server Object Explorer, and give it a few seconds to spin up.

You should have a tree view of various databases.  Dive into (localdb)\projectsv13, and into the folder called 'databases'.  Right click on that folder -> New and call it 'DB_Core2Walkthrough'.  

This creates a local MDF in what ever migratory app data location Visual Studios lands on.  

Right click on the new database -> properties.

Copy the contents of the 'connection string' property to Notepad doc as we will need it shortly.

From there open up 'startup.cs', focus on 'ConfigureServices', and comment out the 'in memory database' line.  Replace it with a db context for Sql Server and our Connection String name.

[code]
            //   services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("name"));

            //Important to db linking to actual sql database.
            services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(Configuration.GetConnectionString("Core2WalkthroughContext")));
[/code]

Save that and open up 'appsettings.json'.  This file holds all sorts of tricky little application wide settings in the simple json format.  No more complex web configs!  

Inside the root braces add a "ConnectionStrings: " section, and in there add and entry called 'Core2WalkthroughContext' (see how that matches what we did in the startup.cs!), and paste in your connection string you saved in the Notepad doc.   

Here's is what mine looked like:

[code]
  "ConnectionStrings": {
    //Important to db linking
    // From the DB's properties when you click on the databse in the 'sql server object explorer'
    "Core2WalkthroughContext": "Data Source=(localdb)\\ProjectsV13;Initial Catalog=DB_Core2Walkthrough;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"

  }
[/code]

Save it up.

Back in the 'sql server object explorer' drill into our database, right click on 'Tables' -> Add New Table.  Call it 'USERS'.

Here we need to mimic the property names from our 'USERS' class in the data folder and their data types. 

We are almost ready to fire this up and test to verify functionality.

Right now we have the DB site ready.. and the project ready to connect, but we need that little piece of information to mate our Razor page code with the table.  In comes our AppDbContext.CS.  Open it up and in the 'OnModelCreating' add a single line to help the project know what database goes to what data class.

[code]            modelBuilder.Entity<USERS>().ToTable("USERS");[/code]

Save that and run it.  Data should be writing to the persistent DB and when you stop the proejct and start it again the data is still there.  Cool! 

If you haven't fiddled around with the localdb and SQL server it would be a good time to stop and do so.  You can directly add data to the tables, edit what is there, etc.  

The steps adding a database table to our local SQL Server database, adding data class, and matching that up the entity to the db will be repeated three more times so make a note to come back to this if you have questions.

[b]2. [/b]  Expand User DB to more columns.

Let us expand out User to include a few more columns.  In the 'USERS.cs' in data add properties for XP, IS_ADMIN, and DESCRIPTION.  We will need these later on expanding out various ways to interact with the USER data.

Make corresponding columns in the 'USERS' table in the local sql server.  Right click on the table 'Users' -> View Designer and add them at the bottom.

Save it all.  

[b]3. [/b]  Let there be news.
I find with most websites that have user registration and activity from time to time the site needs to communicate to their people.  Typically this is a spot on the main page where 'news' is displayed.  You certainly could hardcode the news in HTML and redeploy but that is a pain.  Instead save the news information into a database table and let the page show the most current!

To start let's make our class.  In the 'Data' folder of the solution create a new class called 'NEWS'.  There is not much needed at the moment so and ID, text, and a date entered properties is all that should go in there.

Remember - the helper "[Display(Name =" will be used for any labels first.. if not present it will use the variable name.  Having it here provides a more user friendly way to give the property a name.

[code]
    public class NEWS
    {
        public int ID { get; set; }

        [Display(Name = "Text")]
        public string TEXT { get; set; }

        public DateTime DATE_ENTERED { get; set; }

    }
[/code]

In the database (SQL Server Object Explorer tab) create a table appropriately named 'NEWS' and three columns that match our class.

Finally go to our AppDBContext.cs -> OnModelCreating, and add a line right below our 'USERS' line to let the solution know to marry the table to the class for us.

[code]modelBuilder.Entity<NEWS>().ToTable("NEWS");[/code]

Super.  'News' is now primed and ready to go!  

In a bit we will clean up the index to be more presentable, but let's think who should be able to add news and where that may go.  Not every user should have the ability to add news.. only administrators to the site.  (Recall we added a boolean flag to the USERS table to indicate 'IS_ADMIN').  What is needed is a special page that will will lock down for admins only.

[b]3. [/b] Administration page
The Admin page will be a single page where folk who have the right flag, and are logged in, will be able to do all sorts of sweeping changes to the site.  Add news, remove news, remove users, add activities, etc.  All this functionality will be contained in one spot and help keep the website humming.

For the first part we will be concerned about adding functionality to the page, and later in the tutorial about locking it down.

Start by right clicking on the 'Pages' folder -> add razor page.  Give it a title "Admin", and ok through the creation.  

Open up the 'code behind' for Admin.cshtml.cs and we will begin to add our familiar options.

At the top add: 
-  the db context so data can flow to and from the database, 
- our message string so we can indicate to admins something was successful, 
- a news object for adding news (remember BindProperty means the data survives the voyage back from the client to the server), 
- and a list of news to display if we want to delete it.

These parts are now the page's "model", and help shuffle data to and from the page to the code behind.

Side note, if your page needs to move data you will always have a DB context object.  

[code]
        private readonly AppDbContext _db;// access route to and from the DB.

        [TempData]
        public string Message { get; set; }// no private set b/c we need data back

        [BindProperty] // survive the journey _BACK_ from the user.
        public NEWS NewsAdd { get; set; }// private not set because the data is needed back

        public IList<NEWS> NewsList { get; private set; }// private set so don't need to have data back.. just to show.
[/code]

Add a constructor and have it set our DB context variable.
[code]
        //Constructor
        public AdminModel(AppDbContext db)
        {
            //Get the database context so we can move data to and from the tables.
            _db = db;
        }
[/code]

The OnGet (when the page loads) gets our database data and sets up our list to show the user.

The line is pretty nifty in compactness.  It asks the DB context object to go get the *entire* news data from the database and convert it to a list.  The list will be assigned to the page's model and in the HTML we will display it.
[code]
        //When the page is being fetched, load some data to be rendered.
        public async Task OnGetAsync()
        {

            NewsList = await _db.NEWS_DBSet.AsNoTracking().ToListAsync();
        }
[/code]

That should take care of getting the news data from the database.

[b]4.1 [/b]  Admin - Add news.
Here is where things get a little more tricky: adding and removing the news.  Thankfully half the battle is won with the variables declared.

First let's start with adding data.   

In the CSHTML of the Admin page add a section for a form post to have a label for 'NewsAdd' 's TEXT to display any name we gave it and an input box.  Using the ASP page handler helpers we give the 'submit' button a unique name.

Remember the asp-for in the label will first try and use any "[Display(Name =" added in the NEWS class in the 'Data' folder before just using the property name.
[code]
    <h4>Add News</h4>
    <form method="post">
        <div>
            <label asp-for="NewsAdd.TEXT"></label>
            <input asp-for="NewsAdd.TEXT" />
        </div>
        <input asp-page-handler="NewsAdd" type="submit" value="Add News" />
    </form>
[/code]

Save that, and head back to the code behind.

From the previous tutorial we know that we can have many 'post' methods coming from one form with the 'asp-page-handler' we can decipher which comes from what call.

Start by adding an 'OnPostAsync' method and wedge in the 'NewsAdd' from the asp-page-handler.  Yup.. a little black magic on the Razor Page's part knows to match this up.  

Start by checking if the model is valid, set the 'date entered' to now, and add the NEWS object to the db context's dataset for NEWS.

Await a 'save chagnes' call, add a nice message to tell the admin save was added, and redirect back to itself. 

[code]
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
[/code]


Bada bing, bada boom.  An admin now can save data.  

You should be able to run the project and verify text is being added to the Database through directly viewing the data in the database table as outlined above.

[b]4.2 [/b]  Admin - Remove news.
To remove news I would think the admin would like to know the text and when it was added.  Sounds like showing what is in a table, but we can be a little more helpful and add a button on each row for 'delete' with a value of that row's ID from the database.  It is way less complicated than it sounds.

Starting with the CSHTML we will use a basic HTML table to show the t ext and the data entered. You could show the ID, but really the admin won't care for it.

Since this will have user action that the page needs to handle AFTER we surround it with a form post.
[code]
  <form method="post">
        <table class="table">
            <thead>
                <tr>
                    @*<th>id</th>*@
                    <th>Name</th>
                    <th>Date Added</th>
                </tr>
            </thead>
[/code]

The body holds more interesting magic.  

Using the joy of Core2 in the body utilize a for-each loop to go through the list of news pulled at the load of the page and create rows with <td> for each property in our object.

The 'Core 2 Basics' tutorial outlined the use of the '@' symbol to do this.

Create one third TD of a submit button.  We have the typical asp-page-handler to give it a specific function in the code behind, but we also provide a parameter - the id!  The magic here is "asp-route-" is required, but "id" could be any parameter name we want, but it must correspond to the parameter for our 'on post async' function in the code behind.

[code]
            <tbody>
                @foreach (var temp in Model.NewsList)
                {
                    <tr>
                        @*<td>@temp.ID </td>*@
                        <td>@temp.TEXT</td>
                        <td>@temp.DATE_ENTERED</td>
                        <td>
                            @*Important information here.. matching up the 'asp-page-handler' to the CS function, and the asp-rout- variable name and value.*@
                            <button type="submit" asp-page-handler="DeleteNews" asp-route-id="@temp.ID">Delete</button>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    </form>
[/code]

Save that and head to the code behind.  Much like the 'Add News' we need an 'on post async' method.

The method name is from the asp-page-handler="DeleteNews, and the parameter is from the "asp-route-".  The value is provided there as well.
[code]
        public async Task<IActionResult> OnPostDeleteNewsAsync(int id)
[/code]

The body is pretty simple.  First, use the ID to ask the DB context to find the row in the database.  If found flag the row as 'remove', sand save it.  The DB context takes care of the nitty gritty.

Once done tell the admin it happened and redirect back to the admin page.  (Yes.. another possible way to head bacvk to the same page).
[code]
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
[/code]

Test it out.  You should now be able to see any news data added as well as remove it!  Nifty!

[b]5. [/b]  Add the news to the index.
It is time to clean up index and make the page more akin to what users would expect to see.  Head to the index.CSHTML and clear it out.  I threw in some basic welcoming message with plain HTML as it looked nice.

Add a div for the news to be shown on page load.

Head to the code behind for the index page.

Since we need data from the database we need the trusty db context and a list of NEWS objects to display.  The message string is for future use.

[code]
        private readonly AppDbContext _db;

        public IList<NEWS> NewsList { get; private set; }// private set so don't need to have data back.. just to show.

        [TempData]
        public string Message { get; set; }// no private set b/c we need data back
[/code]

Per usual the constructor for the page must be assigned our DB context.

[code]
        //Constructor
        public IndexModel(AppDbContext db)
        {
            //Get the database context so we can move data to and from the tables.
            _db = db;
        }
[/code]

Finally, when the page is loaded get data from the NEWS table in the database.

[code]
        //When the page is being fetched, load some data to be rendered.
        public async Task OnGetAsync()
        {
            NewsList = await _db.NEWS_DBSet.AsNoTracking().ToListAsync();
        }
[/code]

With our page's model in hand head back to the HMTL.  Using some Razor love we display each row of news text and the date.

[code]
<div>
    <h3>News</h3>
    <div style="margin-left:20px;">
        @foreach (var temp in Model.NewsList)
        {
            <h4>@temp.DATE_ENTERED.ToShortDateString()</h4>
            <span style="margin-left:20px;">@temp.TEXT</span>
            <br />
        }
    </div>

</div>
[/code]

Save it, wipe your brow, and test it.  

Congratulations - the basic steps here are the bulk of the important concepts you will need to get data to a database, remove data, and view data.  Only a bit of black magic goes on with OnPost method names and variables, but the rest is there.  

Keep in mind each page's "model" consists of the properties and there are clear lines drawn for each post to each method.  

Swallow this down, pat yourself on the back, and get a cookie.  You certainly deserve it!  It only gets more interesting from here in part 2.
  
  ================
   Post icon  Posted 06 January 2018 - 01:57 PM 
   ASP.NET - Razor Pages and Core 2. Advanced. 2 of 2 
  
  [u][b]Requirements:[/u][/b]
Visual Studios 2017 Community or higher.
More than a passing familiarity with C# and SQL

[u][b]Github:[/u][/b]
https://github.com/modi1231/Core2Walkthrough

[url="http://www.dreamincode.net/forums/topic/408395-aspnet-razor-pages-core-2-and-entity-framework-basics/"] ASP.NET - Razor Pages, Core 2, and Entity Framework. Basics. [/url]
[url="http://www.dreamincode.net/forums/topic/408585-aspnet-razor-pages-and-core-2-advanced-1-of-2/"] ASP.NET - Razor Pages and Core 2. Advanced. 1 of 2 [/url]

Welcome back.  You should have gone through the Core2 Basics tutorial and the part 1 of this tutorial.  If not you may find yourself frustrated and lost so I highly suggest going back through them.

In this section this dives further into more complex areas of database interaction, custom queries, sessions, and mops up with some extra functions that couldn't be reasonably wedged into the website's concept.

Remember - the web page's concept is like a game where a user needs to log in, is presented with some activities, engages those activities, is rewarded, and must wait X minutes until they can repeat the activities.  Sounds like Mafia wars, amirite?   

[u]Things to remember: [/u]
- DB context is your data henchmen and win gman.  If there's the need to move data to a database or what not make sure he's there with you.
- A given page's "model" is any set of properties and variables in its code behind.
- If you want to see any user input, and have it survive the journey, flag the property or variable as [BindProperty]

In the same vein as the 'add news' let's shore up adding a user. 

[u][b]6. [/b]  Registration[/u]
Typically web pages have a 'registration' page and this one is no different.  We will have a spot for user registration, and some moderate validation to make sure we do not have someone registering the same name multiple times.  

Right click the Page's folder -> add Razor page, and call it 'Register'.

Our page is basic and for the tutorial password management will be a topic you need to cover.  Definitely do not store passwords plain text in a production database, and at the least salt and hash them.  

In the registration page code behind start to build the page's model.  Data interaction is needed so we call up our friend 'db context'.  Also add an object for our USER class, and a message variable in case we need to tell them to pick a new user name.  

Per usual the constructor assigns the db context.
[code]
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
[/code]
With model in hand go to the HTML side and add a simple form post with labels for a user name and description.  (Description is there for later use with a user's profile page)

[code]<h2>Register</h2>
<h3>@Model.Message</h3>

<form method="post">
    @*<div asp-validation-summary="All"></div>*@
    <div><label asp-for="@Model.UserData.Name"></label>: <input asp-for="UserData.Name" /></div>
    <div><label asp-for="@Model.UserData.DESCRIPTION"></label>: <input asp-for="UserData.DESCRIPTION" /></div>
    <input type="submit" />
</form>[/code]

Obviously a more real world page would have things like passwords, emails, and other options, but in the job to keep this simple I wanted to remove kruft best used for later.

Notice since we only have one post the code behind doesn't need a specific name with an 'asp-page-handler='.

Back to the code behind... 

Our single, generic, post method begins with validating the model.  The temp object will be to catch our return from the query to see if the user name exists already.
[code]
        //User clicks a button.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            object temp = null;
[/code]

Strap in - we are going off road.  With the news (previous tutoral) we didn't care if it was duplicate or not.  Just took what the admin gave and passed the package to the DB context to store.  Here we care.  No duplicate user names is the 'business requirement' so we need to be more complex with our DB call.

First - ask the db context, politely, for a database connection handle.
[code]
            //1.  Check if that user name has been registered.
            var con = _db.Database.GetDbConnection();
[/code]
Next we tap the connection to open it.

[code]            try
            {
                await con.OpenAsync();
[/code]

Next, ask the open connection - provided to us by the DB context - for a SQL command object.
[code]                 using (var command = con.CreateCommand())
                {

[/code]
Fourth, create a SQL string and assign it to the command object's text.

The query is simple.  Return '1' if the USERS table has the same as the user is trying to register, and nothing/null if it was not found.

I want this to be case insensitive to I use a SQL 'TOLOWER' to cast what ever is in the name field to all lower case letters.
[code]
                    string q = @" SELECT 1 as MyResult
                                  FROM USERS with(nolock)
                                  WHERE TOLOWER(Name) = @userName ";
                    command.CommandText = q;
[/code]

So far so good, right?

From the command object (provided by the connection object provided by our faithful DB context) we can create a parameter to use in the query.  

The parameter name should match our @ in the query above, and the data from our page model is cast to lower as well.  (Again, being case insensitive).
[code]
                    DbParameter tempParameter = command.CreateParameter();
                    tempParameter.ParameterName = "@userName";
                    tempParameter.Value = UserData.Name.ToLower();
                    command.Parameters.Add(tempParameter);
[/code]

A bit far in the weeds, but almost done.

Now use the 'execute scalar' which returns the first column's first row (so one cell) of data.  Remember this is either 1 or nothing.
https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlcommand.executescalar(v=vs.110).aspx
[code]
                    temp = command.ExecuteScalar();
[/code]

If the temp object is not null and is equal to 1 then the user tried to register for a name that already exists.  We don't want that so we add text to the 'message' object in the pages model and return to our existing page.
[code]                    //2.  If exists then do not do add, and give a message back.
                    if (temp != null && (int)temp == 1)
                    {
                        Message = $"User {UserData.Name} name already exists.";
                        return Page();
                    }
[/code]

If we are here that means the IF statement wasn't triggered so we loop back around to how we did previous USER adds (or like the NEWS add in the Admin page).

Provide a 'date entered', give the db context the User object, and save the changes. 
[code]
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
[/code]

You should be able to save this all and run it to verify if a user is being added to a table, and verify what happens when the same name is added twice.  An informative foray into more complex queries, but we made it!


[u][b]6.1 [/b]  Admin to remove users[/u]
If someone can register we may want the admin to remove a user.  The ying to the yang if you will.  

Head to the previously created 'admin' page.  Much like how we wrote up how to remove news we will replicate that for users.

At the top add a collection of users so the model has a handle on it.
[code]
        public IList<USERS> UserList { get; private set; }// private set so don't need to have data back.. just to show.
[/code]
In the 'on get async' have the DB context get the user list.
[code]
            UserList = await _db.USER_DBSet.AsNoTracking().ToListAsync();
[/code]
Flip to the HTML to add a 'form post' to display the users and just like the News we create a button to the user's ID.

[code]
    <h3>Users</h3>
    <form method="post">
        <table class="table">
            <thead>
                <tr>
                    @*<th>id</th>*@
                    <th>Name</th>
                    <th>Date Entered</th>
                </tr>
            </thead>

            <tbody>
                @foreach (var temp in Model.UserList)
                {
                    <tr>
                        @*<td>@temp.ID </td>*@
                        <td>@temp.Name</td>
                        <td>@temp.DATE_ENTERED</td>
                        <td>
                                <button type="submit" asp-page-handler="DeleteUser" asp-route-id="@temp.ID">Delete</button>
                        </td>
                    </tr>
                }

            </tbody>

        </table>
    </form>
[/code]

Back to the code behind create the OnPost Async for 'Delete User'.  Nothing crazy here and should look darn similar to the remove news.

[code]
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
[/code]

Users can register and can be removed with only a minor bumpy road into a more complex query to the database.  Pat yourself on the back, and grab a cookie.  You definitely deserved it.

[u][b]7. [/b]  Profiles[/u]
With most sites - folks who register have a profile.  This could display information from registration, stats, scores, or pictures.  (Much like how DIC does it).  Our site wouldn't be any different.  We can use the user's ID from the database as a way to uniquely pull their profile up.

Start by creating the Razor page per normal (right click Pages -> new razor page -> "Profile").

Per usual we are looking to pull data so DB context needs to be there as well as our USER object to fill.

[code]
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
[/code]

The only wrinkle is we want to take any number given on the page's "Get" to search the DB and load any data found.  Looks familiar to how the admin page removes users and news.

[code]
         public void OnGet(int id)
        {
            var tempUser = _db.USER_DBSet.Find(id);
            UserData = tempUser;
        }        
[/code]

On the HTML side we can test if the page's model has information or not.  If nothing is there (say no id was provided or an invalid id) display one bit of text.  If found then display the data.

[code]
<h3>@Model.Message</h3>

@if (Model.UserData == null)
{
    <p>No profile to load.</p>
}
else
{
    <p><label asp-for="@Model.UserData.ID"></label> : @Model.UserData.ID</p>
    <p><label asp-for="@Model.UserData.Name"></label> : @Model.UserData.Name</p>
    <p><label asp-for="@Model.UserData.XP"></label> : @Model.UserData.XP</p>
    <p><label asp-for="@Model.UserData.DESCRIPTION"></label> : @Model.UserData.DESCRIPTION</p>


}
[/code]

You should be able to test this by navigating to your localhost/profile/1 or any number that is valid/invalid for you page.  

We will come back to this profile page later, but our work here is done for now.

[u][b]8. [/b]  Sessions[/u]
This is the major hurdle for this tutorial.  Sessions.  How to keep track of some states of data between pages while reacting to what is there.  In the context of this tutorial sessions are important to see who is an 'admin' and track when someone logs in.  It will provide some sweeping changes across the project so buckle up buttercup warp drive is about to be engaged.

Sessions are not on by default and we need to turn them on in the startup services, but we need to add the Nuget package first.

Tools -> Nuget Package Manager -> Manage Nuget Package.  

Search for "Microsoft.AspNetCore.Sessions' and install it.

From there head to 'startup.cs.  In 'ConfigureServices', and after 'services.addmvc', add the following:

[code]
            // Needed for session stuff.  Plus Nuget package.  
            // vvvvvvvvvvvvvvvvv SESSION vvvvvvvvvvvvvvvvvvvvvvvvvv  
            //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state?tabs=aspnetcore2x
            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a timeout 
                options.IdleTimeout = TimeSpan.FromSeconds(60);
                options.Cookie.HttpOnly = true;
            });

            //^^/>/>/>/>/>/>^^/>/>/>/>/>^/>^^/>/>/>/>/>^/>^^/>/>/>/>/>^/>^^/>/>/>/>/>^/>^^/>/>/>/>/>^/>^^/>/>/>/>/>^/> SESSION ^^/>/>/>/>/>/>^^/>/>/>/>/>^/>^^/>/>/>/>/>^/>^^/>/>/>/>/>^/>^^/>/>/>/>/>^/>^^/>/>/>/>/>^/>^^/>/>/>/>/>^/>^^/>/>/>/>/>^/>^^/>/>/>/>/>^/>^^/>/>/>/>/>^/>^
[/code]
.. and that's it.  Sessions can now 'be a thing' in our project.  You can tweak time outs and what not with options while adding more than what is here.

Typically the session values are a key/value pair, and added like this:

[code]
HttpContext.Session.SetString("test", "123");
[/code]

"test" is the key, and "123" is the value.

To fetch a value just ask it to get the string for a given value:

[code]
HttpContextAccessor.HttpContext.Session.GetString("test")
[/code]

A world of possibilities just opened up, and the first to tackle is how to 'log in'.

[u][b]9. [/b]  Log in[/u]
The biggest use will be tracking when a user appropriately logs into our page.  Like a normal site we need to create a 'login' page.

Start by creating the Razor page per normal (right click Pages -> new razor page -> "Login").

Per usual we are looking to pull data so DB context needs to be there as well as our USER object to fill.
[code]
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
[/code]

Since we are not dealing with passwords currently the only log in requirement is the user provides a valid name.

The HTML is pretty simple.  Form to post, a user input, and a button to kick off the post.

[code]
<h2>Login</h2>

<form method="post">

    <h3>@Model.Message</h3>


    @*<div asp-validation-summary="All"></div>*@
    <div>Name: <input asp-for="UserData.Name" value="test" /></div>
    <input type="submit" />
</form>
[/code]

The post, on the other handle, is a little bit different than usual.  

Using the entity framework in line query we can see if the DB context for USERS has a name in it.  To be tricky I think the user better be case exact.  

[code]
        //User clicks a button.
        public IActionResult OnPost()
        {
            //http://www.entityframeworktutorial.net/Querying-with-EDM.aspx

            // 1.  Using the user name, see if a record can be found.
            var user = from a in _db.USER_DBSet
                       where a.Name == UserData.Name
                       select a;
[/code]

Fill the object if one is found.

[code]            USERS tempUser = user.FirstOrDefault<USERS>();[/code]

Clearly any Session info

 [code]           HttpContext.Session.Clear();[/code]

If nothing is found tell the user and don't let them log in.

 [code]           if (tempUser == null)
            {
                Message = "No User Found";
            }
[/code]

If they are found then set the name and id in the session.  

 [code]           else
            {
                // 2.  User is found so stash the name, id, and 'is admin' flag in the session.
                HttpContext.Session.SetString("name", tempUser.Name);
                HttpContext.Session.SetString("id", tempUser.ID.ToString());
[/code]

If they are admin make a note of that too.

 [code]               if (tempUser.IS_ADMIN)
                    HttpContext.Session.SetString("is_admin", "1");
[/code]

Give a custom welcome message and send them to the index page.

 [code]               // 3.  Give a personalized welcome message.
                Message = "Hello " + tempUser.Name + "!";
                
                // 4.  To the index they go.. all logged in.
                return RedirectToPage("/Index");
            }

            return RedirectToPage();
[/code]

Not terribly bad, but now we can individualize menus and data all for that user!  

[u][b]9.1 [/b]  Log out[/u]
Again with the ying of being able to log in the yang to log out must happen.  'Log out' really means the session information becomes invalid through either timeout or actual user interaction.  

In a bid to be fairly universal, and where we will see a bit later, let's stash the single method in the index so head there now.

The method is straight forward.  Clear any session, show a message to the user, and redirect to index. 

[code]
        public IActionResult OnPostLogOut()
        {
            HttpContext.Session.Clear();
            Message = "Logged out";

            return RedirectToPage();
        }
[/code]

Keep that tucked there for a few sections lower.

[u][b]9.2 [/b]  Misc areas[/u]
With sessions the website can be tweaked in a few different ways to take that into account.

The first is the admin page.  In the HTML side add a line near the top to our HTTPContext

[code]@inject IHttpContextAccessor HttpContextAccessor[/code]

A bit below the page name create an if statement to check for a user name and admin check.  

If neither are present then simply show a message that there is no access.

[code]
@if (HttpContextAccessor.HttpContext.Session.GetString("name") == null && HttpContextAccessor.HttpContext.Session.GetString("is_admin") == null)
{
    <h3>No Access</h3>
}
else
{
}
[/code]

The rest of the code goes inside the else.. so all the News and User fun and anything in the future we may only want to the admins to see.

While in the admin HTML we should scoot down to the 'Users' area.  It occurred to me that an admin should be able to remove everyone but themselves.

Update the submit button to check the ID in the for loop to the logged in user's id.  If they are the same do not print a button!

[code]
                            @if (HttpContextAccessor.HttpContext.Session.GetString("id") != temp.ID.ToString())
                            {
                                <button type="submit" asp-page-handler="DeleteUser" asp-route-id="@temp.ID">Delete</button>
                            }
                            else
                            {
                                @Html.Raw("&nbsp;");
                            }
[/code]

Think hard when you add new pages if there needs to be some sort of session check for them.

Make sure to have a test user account with admin flag checked and one that doesn't.  See what happens when you try to navigate to the admin pages with no account, a logged into account with the flag, and one that doesn't.

[u][b]10. [/b]  Universal layout[/u]
There are quite a few files that had not needed interaction with. As you explore this wonderful new playground you may make need of them, but let's focus on one specific one now - the "_Layout.cshtml".  

This file is a universal layout that is applied to all the razor pages in the project.  Do you have a footer needed on each page?  This will have it.  Are there site wide CSS you need to load?  Put it here!  In our case this is where the menu that appears on each page is housed.

Open the file up and dig past the 'body' tag into the first 'nav', the div with the 'container' class, and find the div with the class 'navbar-collapse collapse'.  Here you should see an unordered list with our various existing menu bits  The plan is we need to think about what menu items should be shown all the time, which are for folk not logged in, which are for folk logged in, and which are for logged in admins.

The thinking is some menu options should disappear with a bit of session checking.  For example - if a user logs in do they really need to see the 'register' or 'login' buttons?  Most likely not.  The same idea with the 'admin' button.. should everyone see it?  Most certainly not.

Using the same logic as the sessions section above we can turn off and on things.

After the about page I will put the 'log in' and 'register' buttons, but wrap them in a check to see if there is a session variable.  If there is none then that means no one has logged in and show the buttons.  If there is don't render them.

[code]
                    @if (HttpContextAccessor.HttpContext.Session.GetString("name") == null)
                    {
                        //if not logged in, show these.
                        <li><a asp-page="/Login">Login</a></li>
                        <li><a asp-page="/Register">Register</a></li>
                    }[/code]

After that things get a little more fancy.  I want a drop down similar to DIC's that is shown when logged in.  This will hold a link for the admin, a quick link to the logged in profile, and a logged out button.

Only show if the session variable is set.

[code]                    @if (HttpContextAccessor.HttpContext.Session.GetString("name") != null)
                    {
[/code]

A nice drop down with a header name 'My Account'.

[code]                        <li class="dropdown">
                            <a href="#" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-expanded="false">My Account<span class="caret"></span></a>
                            <ul class="dropdown-menu" role="menu">
[/code]

If the admin flag is set show the admin else do not render it.

 [code]                               @if (HttpContextAccessor.HttpContext.Session.GetString("is_admin") != null)
                                {
                                    //if logged in, AND an admin, show this.
                                    <li><a asp-page="/Admin">Admin</a></li>
                                }
[/code]

Given the user's logged in ID in session, have a link to the profile page we created.  See above for that page.

 [code]                               <li><a asp-page="/Profile" asp-route-id="@HttpContextAccessor.HttpContext.Session.GetString("id")">Profile: @HttpContextAccessor.HttpContext.Session.GetString("name")</a></li>
[/code]

A little wiz bang trickery to have an index button that goes to the index page and looks for the 'logout' method we created above.  This works from any page!

[code]                                <li>
                                    <form method="post">
                                        @*Having a hard time making anchor tag be a href and not button*@
                                        @*The Index's code behind has the logic to log out*@
                                        <button type="submit" asp-page="/Index" asp-page-handler="logout">logout</button>
                                    </form>
                                </li>
                            </ul>
                        </li>

                    }[/code]

Not bad for a dynamic menu system based off the measlie session we setup earlier.  Take another cookie - we are on the home stretch.

[u][b]11. [/b]  Activity[/u]
The main thrust of the page is to have a user log in and do some activity to gain points and wait for the activity to 'cool down'.  This is going to stretch everything you have learned as we go through the sequences of adding a class, a DB, updating DB context, having the admin functions, and show a page to allow the user to do the activity.  Buckle in once more and see how what we learned makes this a pretty straight forward addition.

A little more on the concept.  The activity will have an XP reward to add to the user's total while a cool down time in minutes to make it more tricky for the user to accrue more points.  This means user 1 could kick off an activity and have to wait another 10 minutes while user2 already did the action and has two minutes left.  This is dynamic based on teh user who did the action.  One possible way is to temporarily log the user id and activity id.. and when the activity page loads only show actions that can be done based on this log table.

[u][b]11.1 [/b]  Activity DB.[/u]
In the Data folder create two classes 'ACTIVITY' and 'ACTIVITY_LOG'.

Let's throw a curve ball in here and have an 'is active' flag.  This means the admin can disable, but not delete, an activity.. say for a special occasion like a holiday or commercial event.

[code] 
    public class ACTIVITY
    {
        public int ID { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "XP")]
        public int XP { get; set; }

        [Display(Name = "Cool Down Time")]
        public int? COOL_OFF_MINUTES { get; set; } //in case anyone puts nothing in there.

        public bool IS_ACTIVE { get; set; }

        [Display(Name = "Created Date")]
        public DateTime DATE_ENTERED { get; set; }

        [NotMapped]
        public int? TIME_LEFT { get; set; }

        public ACTIVITY()
        {
            COOL_OFF_MINUTES = 0;
        }
    }
[/code] 

[code] 
    public class ACTIVITY_LOG
    {
        public int ID { get; set; }

        public int ID_USER { get; set; }

        public int ID_ACTIVITY { get; set; }

        public DateTime DATE_ENTERED { get; set; }

    }
[/code] 

Then replicate the tables in the database.  

Open up the DB Context and add our DBSets:

[code] 
        public DbSet<ACTIVITY> ACTIVITY_DBSet { get; set; }

        public DbSet<ACTIVITY_LOG> ACTIVITY_LOG_DBSet { get; set; }
[/code]

.. and in 'OnModelCreating' add the mapping from the classes to the DB tables.

[code] 
            modelBuilder.Entity<ACTIVITY>().ToTable("ACTIVITY");
            modelBuilder.Entity<ACTIVITY_LOG>().ToTable("ACTIVITY_LOG");
[/code]

Great now let's integrate it into our page.

[u][b]11.2 [/b]  Activity Admin.[/u]
First we should be able to add/remove data from the table.  This means head to the 'admin' page we created earlier.

Much like the users and news add a list of activities to be displayed and a single activity instance for the admin to add new ones.

[code] 
        public IList<ACTIVITY> ActivityList { get; private set; }// private set so don't need to have data back.. just to show.

        [BindProperty]// survive the journey _BACK_ from the user.
        public ACTIVITY ActivityAdd { get; set; }
[/code]

With the page's model updated head to the HTML side of admin.

Similar to the rest of the display there is a form post method with a table that a for-each fills. The enable/disable button is there to turn off and on activities.

[code]     <form method="post">
        <table class="table">
            <thead>
                <tr>
                    @*<th>ID </th>*@
                    <th>Title</th>
                    <th>XP</th>
                    <th>COOL_OFF_MINUTES</th>
                    <th>IS_ACTIVE</th>
                    <th>DATE_ENTERED</th>
                </tr>
            </thead>

            <tbody>
                @foreach (var temp in Model.ActivityList)
                {
                    <tr>
                        @*<td>@temp.ID </td>*@
                        <td>@temp.Title</td>
                        <td>@temp.XP</td>
                        <td>@temp.COOL_OFF_MINUTES</td>
                        <td>@temp.IS_ACTIVE</td>
                        <td>@temp.DATE_ENTERED</td>
                        <td>
                            <button type="submit" asp-page-handler="ActivityDisable" asp-route-id="@temp.ID">Enable/Disable</button>
                        </td>
                    </tr>
                }

            </tbody>

        </table>
    </form>
[/code]

Adding activities is a mirror of adding news.  Another form post with labels and text boxes to fill out.

[code]     <form method="post">
        <div>
            <label asp-for="ActivityAdd.Title"></label>
            <input asp-for="ActivityAdd.Title" />

            <label asp-for="ActivityAdd.Description"></label>
            <input asp-for="ActivityAdd.Description" />

            <label asp-for="ActivityAdd.XP"></label>
            <input asp-for="ActivityAdd.XP" />

            <label asp-for="ActivityAdd.COOL_OFF_MINUTES"></label>
            <input asp-for="ActivityAdd.COOL_OFF_MINUTES" />

            <label asp-for="ActivityAdd.IS_ACTIVE"></label>
            <input asp-for="ActivityAdd.IS_ACTIVE" />
        </div>
        <input asp-page-handler="ActivityAdd" type="submit" value="Add Activity" />
    </form>
[/code]

Flip to the code behind to setup the OnPost Async functions needed.

The disable/enable is a quick matter of finding the activity in the DB, getting a handle on it, and flipping the existing 'isActive', and then saving it all back to the DB.

The wrinkle here is after we manually change the row we need to flag it as 'modified' so the DBContext knows to update it.

[code] 
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
[/code]

Adding an activity looks about on par with adding news. Nothing out of place here.

 [code]        //The user signals there is an action to do.  Notice how the words between "OnPost" and "Async" match up with the CHTML's "asp-page-handler"
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
[/code]

You can go and test the Admin page's functionality here and see the data show up and/or be disabled.

[u][b]11.3 [/b]  Activity Page.[/u]
Admin functions out of the way now we focus on how the regular logged in users would interact with the activities.  Create a new Razor page called 'activity'.

Per usual we needed our data pipe so add a dbcontext, a list of activities, and set the context in the constructor.

[code]         private readonly AppDbContext _db;

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
[/code]

When the page loads things get a little tricky as we need some moderately complex data back.  The page should get any entries in the log table for a given user ID and show how much time each activity has left to 'cool down' while also getting any actvities that the user has not engaged in.. ie not in the log table.  

Get the user id from the session, and get a connection from our db context.

[code]         public async Task OnGetAsync()
        {
            int userID = 0;
            int.TryParse(HttpContext.Session.GetString("id"), out userID);

            var con = _db.Database.GetDbConnection();
            try
            {
[/code]

Open the connection and get a command object from the connection given to us by the db context.

[code]                 await con.OpenAsync();
                using (var command = con.CreateCommand())
                {
[/code]

Here's the chunk that only gets active activities where there are entries logged in the table.  FYI there is a bit of SQL magic happening here, but that is outside the scope of this lengthy tutorial.  Nothing a bit of MSDN searching won't clear up.

[code]                     string q = @" SELECT a.ID,
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
[/code]

Mary that data with all the activities _NOT_ above with a union.

[code]                         UNION

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
[/code]

The only parameter needed is the user id.

[code]                     DbParameter tempParameter = command.CreateParameter();
                    tempParameter.ParameterName = "@userid";
                    tempParameter.Value = userID;
                    command.Parameters.Add(tempParameter);[/code]

For a fun alternative use a data reader to collect our rows.. 

[code]                     System.Data.Common.DbDataReader reader = await command.ExecuteReaderAsync();[/code]

Assuming the reader has rows, instantiate our list of activitiesi and read each one.

[code]                    if (reader.HasRows)
                    {
                        ActivityList = new List<ACTIVITY>();

                        while (await reader.ReadAsync())
                        {
[/code]

Create a new activity instance from the reader's data and add it to the list.

[code]                             var row = new ACTIVITY
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
[/code]

Wrap up the try's catch and now when the page loads the only activities in our page's models are ones that have time left that can't be activated (but we show them this), and ones the user can click on.

[code]             }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                con.Dispose();
            }
        }
[/code]

Crazy, right?

Head to the HTML for a bit more familiar waters.  Activities are deeply tied to a user id so only show data on this page if the user has logged in.  Don't forget the @inject!

[code] @inject IHttpContextAccessor HttpContextAccessor

@{
    ViewData["Title"] = "Activity";
}

<h2>Activity</h2>
<h3><font color="lime"> @Model.Message</font></h3>

@* Quick check to see which part of the code is being rendered.*@
@if (HttpContextAccessor.HttpContext.Session.GetString("name") == null)
{
    <h3>No Access</h3>
}
else
{[/code]

The table looks fairly normal until we get up to the per-row-action button.

[code]    <form method="post">
        <table class="table">
            <thead>
                <tr>
                    @*<th>ID </th>*@
                    <th>Title</th>
                    <th>XP</th>
                    <th>COOL_OFF_MINUTES</th>
                    <th>IS_ACTIVE</th>
                    <th>DATE_ENTERED</th>
                </tr>
            </thead>

            <tbody>
                @foreach (var temp in Model.ActivityList)
                {
                    <tr>
                        @*<td>@temp.ID </td>*@
                        <td>@temp.Title</td>
                        <td>@temp.XP</td>
                        <td>@temp.COOL_OFF_MINUTES</td>
                        <td>@temp.IS_ACTIVE</td>
                        <td>@temp.DATE_ENTERED</td>
                        <td>
[/code]

Here there are two possible views.. one if the time left is 0 that means the user can kick off the activity so show an action button with the right page handler and route id of the activity.

[code]                             @if (temp.TIME_LEFT == 0)
                            {
                                //Matches up with the OnPostActivityDoAsync in the cs page.
                                <button type="submit" asp-page-handler="ActivityDo" asp-route-id="@temp.ID">Do</button>
                            }
[/code]

If the activity still has time left be a gent and show the user how much time is left, but do not have an action button available.  Using a bit of C# magic with timespans we can convert the time to minutes and seconds remaining.  Refreshing the page should show a lower count each time.

[code]                             else
                            {
                                //Utilizing some .NET ability here.
                                TimeSpan t = TimeSpan.FromSeconds((double)temp.TIME_LEFT);

                                <label>Left: mm: @Math.Floor(@t.TotalMinutes).ToString()   ss: @t.Seconds.ToString() </label>
                            }
                        </td>
                    </tr>
                }

            </tbody>

        </table>
    </form>

[/code]

This has certainly taken us to the edge of what we have learned and a smidge over.  Just one more step on what to do when a user clicks on the action button for an available activity... so back to the code behind.

From the sesion get the user ID.

[code]         public async Task<IActionResult> OnPostActivityDoAsync(int id)
        {
            int userID = 0;

            if (HttpContext.Session.GetString("id") != null)
            {
                int.TryParse(HttpContext.Session.GetString("id"), out userID);
[/code]

Get the associated XP from the activity's entry in the database and the user's row.

[code] 
                // 1.  Update XP on user profile
                ACTIVITY tempActivity = await _db.ACTIVITY_DBSet.FindAsync(id);
                USERS tempUser = await _db.USER_DBSet.FindAsync(userID);
[/code]

Add the activityu's XP to the user's total XP, and save it.

[code]                 tempUser.XP += tempActivity.XP;
                _db.USER_DBSet.Update(tempUser);
                _db.SaveChanges();
[/code]

Next I need to clear out any previous entries of this user id and activity id.  For fun this time around we can use LINQ to get a handle to a row in the activity log that has matching keys.  (there only should be one at most).

[code]                 // 2.  Upate log.
                ACTIVITY_LOG temp = new ACTIVITY_LOG();

                // If you got this far then the button is enabled so remove all instances of the old.
                var temp2 = _db.ACTIVITY_LOG_DBSet.Where(s => s.ID_ACTIVITY == id && s.ID_USER == userID);
[/code]

Remove what was found, and save.

[code]                 _db.ACTIVITY_LOG_DBSet.RemoveRange(temp2);
                _db.SaveChanges();[/code]

Create a new entry for the table and save that.

[code]                 temp.ID_ACTIVITY = id;

                temp.ID_USER = userID;
                temp.DATE_ENTERED = DateTime.Now;

                _db.ACTIVITY_LOG_DBSet.Add(temp);
                await _db.SaveChangesAsync();
[/code]

Assuming it all goes well let the player know and head back to the activity table.

[code]             }

            Message = $"Activity started; XP added.";

            return RedirectToPage("/Activity");
        }[/code]

Whew!  What a load of work.  Pat yourself on the back.. that was the final exam!  All that is needed now is to update the menu and we can put a bow on activities.


[u][b]11.4 [/b]  Activity Menu.[/u]
Head to the _layout.cshtml.  Right after the 'admin' session check add a link to the activities page.
[code]                                 <li><a asp-page="/Activity">Activity</a></li>[/code]

Save it and rejoice!  Test your heart out.  There are just a few perfuncatory things to mop up and the tutorial should be ready to put in the can.

[u][b]12. [/b]  Leader Board[/u]

Having XP is great to rack up, but in games like this showing who is top dog is part of the fun.  The idea is to have a read only page, open to everyone, to show the rankings.  Well within work we have already seen and uses information we already have!

There are no new classes as we will be recyling the user class.

Create a new razor page called 'leaderboard'.

Per usual data means data context and listing things out means a list of users.

[code]         private readonly AppDbContext _db;

        public IList<USERS> UserList { get; private set; }

        public LeaderboardModel(AppDbContext db)
        {
            _db = db;
        }[/code]

For a little flair the get will use LINQ to arrange the data by XP and then by name.

[code]         public async Task OnGetAsync()
        {
            UserList = await (from a in _db.USER_DBSet
                       orderby a.XP descending, a.Name
                       select a).ToListAsync();
        }[/code]

Boom - the data is ready to load!

Head to the HTML side of life and make a table (no form needed as no user actions can be had) and setup a table per usual.  The only curve here is I: opted to use a plain for loop to give each row a numerical count.

[code] <table class="table">
    <thead>
        <tr>
            <th>#</th>
            <th>Name</th>
            <th>XP</th>
        </tr>
    </thead>

    <tbody>
        @*Make use of a basic for loop to show rankings*@
        @for (int i = 0; i < Model.UserList.Count - 1; i++)
        {
            <tr>
                <td>@(i + 1)</td>
                @*Link to the profiles with the right routing ids *@
                <td><a asp-page="/Profile" asp-route-id="@Model.UserList[i].ID">@Model.UserList[i].Name</a></td>
                <td>@Model.UserList[i].XP</td>
            </tr>
        }

    </tbody>

</table>[/code]

Easy peasy.  

Since  this is page open to everyone go to the _layout.cshtml and add this line right after the 'about'.

[code]                     <li><a asp-page="/Leaderboard">Leader Board</a></li>[/code]

[u][b]13. [/b]  Miscellaneous[/u]
That's it.. we took a voyage and explored the crazy ways to get data, display it, edit it, manipulate it, and organized it into a semi logical page.  There are a few last things to clean up that didn't quite fit as neat but nothing out of the norm.

[u][b]13.1 [/b] About page[/u]
The about page was left to show a plain jane HTML page.  No fancy data loads, saves, or in code curve balls.  Yup. you can style this like any HTML page.

[u][b]13.2 [/b] Test page[/u]
The test page was created to show case a few other UI elements I couldn't make use of with the expansive, but limited, tutorial.

Here drop down lists, radio buttons, and checkboxes are made use with do nothing data.

[u][b]13.2.1 [/b] Drop Down Lists[/u]
Drop down lists have two major componets to their model.  A enumerated list of select list items, and a single 'bind property' to get the user's input.

Code behind:

[code]         public IEnumerable<SelectListItem> testDD { get; set; }// DropDown Testing

        [BindProperty]
        public string testDDSelected { get; set; }//DropDown Testing.. gets the selected value on post
[/code]

The constructor is just there to show a list of three strings.

[code]         public TestModel(AppDbContext db)
        {
            //DropDown Testing - fill values for hte list.
            IList<string> foo = new List<string>();
            foo.Add("aa");
            foo.Add("bb");
            foo.Add("cc");

            //convert that to a 'select list item'.
            testDD = from a in foo
                     select new SelectListItem
                     {
                         Text = a,
                         Value = a
                     };

        }
[/code]

The action verifies what hte user sent back survived the trip and displays it.

[code]         //DropDown Testing - display selected item to show it was picked.
        public IActionResult OnPostDropDownList()
        {
            Message = "Dropdown: " + testDDSelected;
            return RedirectToPage();
        }
[/code]

HTML

[code] <form method="post">
    @* templated helper *@
    @* 1st param - the selected value to return on post, 2nd param the list from the model, 3rd parameter - some initial filler text *@
    @Html.DropDownListFor(a => a.testDDSelected, Model.testDD, "--Select a Value--")
    <button type="submit" asp-page-handler="DropDownList">Test</button>
</form>
[/code]

[u][b]13.2.2 [/b] Radio buttons[/u]
Radio buttons just make use of a variable to hold the value that the user selected.

Code behind:
[code]         [BindProperty]
        public string testRadioButtonSelected { get; set; }//Radiobutton Testing.. gets the selected value on post
[/code]

HTML: 

[code] <form method="post">
    @Html.RadioButtonFor(a => a.testRadioButtonSelected, "A")@Html.Label("A")
    @Html.RadioButtonFor(a => a.testRadioButtonSelected, "B")@Html.Label("B")
    <button type="submit" asp-page-handler="RadioButton">Test</button>
</form>
[/code]

[u][b]13.2.3 [/b] Checkboxes[/u]
Checkboxes are similar to radio buttons in there is only one object for the page's model which holds the value on return.

[code]         [BindProperty]
        public bool isChecked { get; set; }//Checkbox Testing.. gets the selected value on post
[/code]

HTML:

[code] <form method="post">
    @Html.CheckBoxFor(a => a.isChecked)@Html.LabelFor(a => a.isChecked)
    <button type="submit" asp-page-handler="CheckBox">Test</button>
</form>
[/code]

[b][u]Wrap up[/b][/u]
That took a broad, but sufficiently deep, look at the bulk of functionality you may need in a site while showing varrying ways of doing the same thing.  This should get you up on your feet enough to know where to look and what to ask for.  Check the github for the complete project.


[u]Areas to expand on:[/u]
- text box validation
- actual password salt/hash
- password recovery
- nifty security features and services
