using RememberText.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Interfaces
{
    public interface IRTGuestBookService
    {
        /// <summary>Get all Messages</summary>
        /// <returns>All Messages</returns>
        IQueryable<GuestBookEntry> GetAllMessanges();
        /// <summary>Get a selected Message</summary>
        /// <param name="id">Message id</param>
        /// <returns>Message</returns>
        Task<GuestBookEntry> GetMessageByIdAsync(int id);
        /// <summary>Add a Message</summary>
        /// <param name="entry">Message</param>
        Task AddMessage(GuestBookEntry entry);
        /// <summary>Remove a Message from database</summary>
        /// <param name="id">Message id</param>
        Task RemoveMessage(int id);
    }
}
