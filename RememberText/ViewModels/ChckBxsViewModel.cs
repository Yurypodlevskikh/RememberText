using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace RememberText.ViewModels
{
    public class ChckBxsViewModel
    {
        public string FieldsName { get; set; }
        public List<SelectListItem> Checks { get; set; }
    }
}
