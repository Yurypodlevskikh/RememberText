using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.ViewModels
{
    public class SpaceProgressBarViewModel
    {
        public int AmountOfText { get; set; }
        public int LimitationOfChars { get; set; }
        public int Percent { get; set; }
        public int SuccessPercent { get; set; }
        public int WarningPercent { get; set; }
        public int DangerPercent { get; set; }
        public int SuccessMax { get; set; }
        public int WarningMax { get; set; }
        public int DangerMax { get; set; }
    }
}
