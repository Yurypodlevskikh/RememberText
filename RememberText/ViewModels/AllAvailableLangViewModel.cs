using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.ViewModels
{
    public class AllAvailableLangViewModel
    {
        public int Id { get; set; }
        public string LangCode { get; set; }
        public string LangName { get; set; }
        public int Order { get; set; }
        public bool Selected { get; set; }
    }
}
