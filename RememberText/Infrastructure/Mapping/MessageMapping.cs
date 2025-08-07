using RememberText.Domain.Entities;
using RememberText.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Mapping
{
    public static class MessageMapping
    {
        public static GuestBookDetailsViewModel ToMessageDetails(GuestBookEntry message) => new GuestBookDetailsViewModel
        {
            MessageId = message.Id,
            MessageTitle = message.MessageTitle,
            Message = message.Message,
            SenderNickname = message.User.Nickname,
            CreatedDateTime = message.CreatedDateTime
        };

        public static List<GuestBookDetailsViewModel> ToMessageTable(IQueryable<GuestBookEntry> messages)
        {
            var messageTable = new List<GuestBookDetailsViewModel>();
            int tempMessLength = 30;
            foreach(var m in messages)
            {
                var message = new GuestBookDetailsViewModel
                {
                    MessageId = m.Id,
                    MessageTitle = m.MessageTitle,
                    Message = m.Message.Length > tempMessLength ? m.Message.Substring(0, 30) + "..." : m.Message,
                    SenderNickname = m.User.Nickname,
                    CreatedDateTime = m.CreatedDateTime
                };
                messageTable.Add(message);
            }
            return messageTable.OrderBy(x => x.CreatedDateTime).ToList();
        }
    }
}
