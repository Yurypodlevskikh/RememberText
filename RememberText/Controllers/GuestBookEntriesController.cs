using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RememberText.DAL.Context;
using RememberText.Infrastructure.Helpers;
using RememberText.Infrastructure.Interfaces;
using RememberText.Infrastructure.Mapping;
using RememberText.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Controllers
{
    [Authorize(Roles = "GenAdministrator")]
    [Route("GuestBookEntries")]
    public class GuestBookEntriesController : Controller
    {
        private readonly RememberTextDbContext _context;
        private readonly IRTGuestBookService _guestBookService;

        public GuestBookEntriesController(RememberTextDbContext context, IRTGuestBookService guestBookService)
        {
            _context = context;
            _guestBookService = guestBookService;
        }

        [Route("{PageNumber:int?}")]
        public IActionResult Index(int? PageNumber)
        {
            int pageSize = 10;
            var messagesToView = MessageMapping.ToMessageTable(_guestBookService.GetAllMessanges());
            int totalPages = (int)Math.Ceiling(messagesToView.Count() / (double)pageSize);
            int page = PageNumber != null ? PageNumber > totalPages ? totalPages : (int)PageNumber : 1;

            var paginatedMessages = PaginatedList<GuestBookDetailsViewModel>.Create(messagesToView, page, pageSize);
            
            return View(paginatedMessages);
        }

        // GET: GuestBookEntries/Details/5
        [Route("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guestBookEntry = await _guestBookService.GetMessageByIdAsync((int)id);
            if (guestBookEntry == null)
            {
                return NotFound();
            }

            var message = MessageMapping.ToMessageDetails(guestBookEntry);

            return View(message);
        }

        // GET: GuestBookEntries/Edit/5
        //[Route("Edit")]
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var guestBookEntry = await _context.GuestBookEntries.FindAsync(id);
        //    if (guestBookEntry == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", guestBookEntry.UserId);
        //    return View(guestBookEntry);
        //}

        //// POST: GuestBookEntries/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Route("Edit")]
        //public async Task<IActionResult> Edit(int id, [Bind("MessageTitle,Message,UserId,CreatedDateTime,Id")] GuestBookEntry guestBookEntry)
        //{
        //    if (id != guestBookEntry.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(guestBookEntry);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!GuestBookEntryExists(guestBookEntry.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", guestBookEntry.UserId);
        //    return View(guestBookEntry);
        //}

        // GET: GuestBookEntries/Delete/5
        [Route("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guestBookEntry = await _guestBookService.GetMessageByIdAsync((int)id);
            if (guestBookEntry == null)
            {
                return NotFound();
            }

            var message = MessageMapping.ToMessageDetails(guestBookEntry);

            return View(message);
        }

        // POST: GuestBookEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var guestBookEntry = await _context.GuestBookEntries.FindAsync(id);
            _context.GuestBookEntries.Remove(guestBookEntry);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GuestBookEntryExists(int id)
        {
            return _context.GuestBookEntries.Any(e => e.Id == id);
        }
    }
}
