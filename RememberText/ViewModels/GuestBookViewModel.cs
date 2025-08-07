using System;
using System.ComponentModel.DataAnnotations;

namespace RememberText.ViewModels
{
    public class SendGuestBookViewModel
    {
        [Required]
        [Display(Name = "Sender")]
        public string Nickname { get; set; }
        [Display(Name = "Title")]
        public string Title { get; set; }
        [Required]
        [StringLength(1000)]
        [Display(Name = "Message")]
        public string Message { get; set; }
    }

    public class GuestBookDetailsViewModel
    {
        public int MessageId { get; set; }
        [Display(Name = "Title")]
        public string MessageTitle { get; set; }
        [Display(Name = "Message")]
        public string Message { get; set; }
        [Display(Name = "Sender")]
        public string SenderNickname { get; set; }
        [Display(Name = "Created")]
        public DateTime CreatedDateTime { get; set; }
    }
}
