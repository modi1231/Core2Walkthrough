using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

/*
 *  About: Basic page showing setup for other various controls that did not fit in the original tutorials
 *  -- drop down lists
 */
namespace Core2Walkthrough.Pages
{

    public class TestModel : PageModel
    {
        [TempData]
        public string Message { get; set; }// no private set b/c we need data back


        public IEnumerable<SelectListItem> testDD { get; set; }// DropDown Testing

        [BindProperty]
        public string testDDSelected { get; set; }//DropDown Testing.. gets the selected value on post

        [BindProperty]
        public string testRadioButtonSelected { get; set; }//Radiobutton Testing.. gets the selected value on post

        [BindProperty]
        public bool isChecked { get; set; }//Checkbox Testing.. gets the selected value on post

        public TestModel(AppDbContext db)
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

        //DropDown Testing - display selected item to show it was picked.
        public IActionResult OnPostDropDownList()
        {
            Message = "Dropdown: " + testDDSelected;
            return RedirectToPage();
        }

        //RadioButton Testing - display selected item to show it was picked.
        public IActionResult OnPostRadioButton()
        {
            Message = "Radiobutton: " + testRadioButtonSelected.ToString();
            return RedirectToPage();
        }

        //Checkbox Testing - display selected item to show it was picked.
        public IActionResult OnPostCheckBox()
        {
            Message = "Checkbox: " + isChecked.ToString();
            return RedirectToPage();
        }
    }
}