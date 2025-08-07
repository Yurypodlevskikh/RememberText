using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using RememberText.DAL.Context;
using RememberText.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Controllers
{
    [Authorize]
    [RequireHttps]
    public class NormalizedTagsController : Controller
    {
        private readonly RememberTextDbContext _context;
        private readonly IActionDescriptorCollectionProvider _provider;

        public NormalizedTagsController(RememberTextDbContext context, IActionDescriptorCollectionProvider provider)
        {
            _context = context;
            _provider = provider;
        }

        // GET: NormalizedTags
        public async Task<IActionResult> Index()
        {
            var routes = _provider.ActionDescriptors.Items.Where(x => x.AttributeRouteInfo != null);
            List<string> temps = new List<string>();
            foreach (var route in routes)
            {
                temps.Add(route.AttributeRouteInfo.Template);
            }

            ViewData["routesInfo"] = temps;

            var normTagsLinq = await _context.NormalizedTags.ToListAsync();

            //string dataTable = "dbo.NormalizedTags";
            //string sqlCommand = $"SELECT * FROM {dataTable}";
            //var normTagsSomeCulture = RawSqlQueryHelper.RawSqlReader(_context, sqlCommand, x => new NormalizedTag { Id = (int)x[0], NormalizedTagName = (string)x[2] });

            return View(normTagsLinq);
        }

        // GET: NormalizedTags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var normalizedTag = await _context.NormalizedTags
                .FirstOrDefaultAsync(m => m.Id == id);
            if (normalizedTag == null)
            {
                return NotFound();
            }

            return View(normalizedTag);
        }

        // GET: NormalizedTags/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NormalizedTags/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NormalizedTagName,RowVersion,Id")] NormalizedTag normalizedTag)
        {
            if (ModelState.IsValid)
            {
                _context.Add(normalizedTag);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(normalizedTag);
        }

        // GET: NormalizedTags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var normalizedTag = await _context.NormalizedTags.FindAsync(id);
            if (normalizedTag == null)
            {
                return NotFound();
            }
            return View(normalizedTag);
        }

        // POST: NormalizedTags/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NormalizedTagName,RowVersion,Id")] NormalizedTag normalizedTag)
        {
            if (id != normalizedTag.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(normalizedTag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NormalizedTagExists(normalizedTag.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(normalizedTag);
        }

        // GET: NormalizedTags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var normalizedTag = await _context.NormalizedTags
                .FirstOrDefaultAsync(m => m.Id == id);
            if (normalizedTag == null)
            {
                return NotFound();
            }

            return View(normalizedTag);
        }

        // POST: NormalizedTags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var normalizedTag = await _context.NormalizedTags.FindAsync(id);
            _context.NormalizedTags.Remove(normalizedTag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NormalizedTagExists(int id)
        {
            return _context.NormalizedTags.Any(e => e.Id == id);
        }
    }
}
