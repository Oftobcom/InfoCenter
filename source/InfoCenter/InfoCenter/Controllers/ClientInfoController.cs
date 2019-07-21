using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InfoCenter.Models;

namespace InfoCenter.Controllers
{
    public class ClientInfoController : Controller
    {
        private readonly InfoCenterContext _context;

        public ClientInfoController(InfoCenterContext context)
        {
            _context = context;
        }

        // GET: ClientInfo
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.ClientInfo.ToListAsync());
        //}

        public async Task<IActionResult> Index(string searchPhone)
        {
            var clientInfo = from m in _context.ClientInfo
                         select m;

            if (!String.IsNullOrEmpty(searchPhone))
            {
                clientInfo = clientInfo.Where(s => s.Phone.Contains(searchPhone));
            }

            var clientInfoSearchVM = new ClientInfoSearchViewModel
            {
                ClientInfo = await clientInfo.ToListAsync()
            };

            return View(clientInfoSearchVM);

            //return View(await clientInfo.ToListAsync());
        }

        // GET: ClientInfo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientInfo = await _context.ClientInfo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clientInfo == null)
            {
                return NotFound();
            }

            return View(clientInfo);
        }

        // GET: ClientInfo/ClientInfoByPhone/2100
        public async Task<IActionResult> InfoByPhone(string id)
        {
            //caller = "992927052100";
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var clientInfo = await _context.ClientInfo
                .FirstOrDefaultAsync(m => m.Phone == id);
            if (clientInfo == null)
            {
                return NotFound();
            }

            return View(clientInfo);
        }

        // GET: ClientInfo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClientInfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Surname,Firstname,Patronymic,Address,Phone,NumberTotalCredits,NumberActiveCredits,NumberTotalDeposits,NumberActiveDeposits,NumberRemittances")] ClientInfo clientInfo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clientInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(clientInfo);
        }

        // GET: ClientInfoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientInfo = await _context.ClientInfo.FindAsync(id);
            if (clientInfo == null)
            {
                return NotFound();
            }
            return View(clientInfo);
        }

        // POST: ClientInfoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Surname,Firstname,Patronymic,Address,Phone,NumberTotalCredits,NumberActiveCredits,NumberTotalDeposits,NumberActiveDeposits,NumberRemittances")] ClientInfo clientInfo)
        {
            if (id != clientInfo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clientInfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientInfoExists(clientInfo.Id))
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
            return View(clientInfo);
        }

        // GET: ClientInfoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientInfo = await _context.ClientInfo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clientInfo == null)
            {
                return NotFound();
            }

            return View(clientInfo);
        }

        // POST: ClientInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clientInfo = await _context.ClientInfo.FindAsync(id);
            _context.ClientInfo.Remove(clientInfo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientInfoExists(int id)
        {
            return _context.ClientInfo.Any(e => e.Id == id);
        }
    }
}
