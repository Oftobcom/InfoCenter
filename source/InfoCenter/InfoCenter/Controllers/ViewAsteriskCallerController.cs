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
    public class ViewAsteriskCallerController : Controller
    {
        private readonly InfoCenterContext _context;

        public ViewAsteriskCallerController(InfoCenterContext context)
        {
            _context = context;
        }

        // GET: AsteriskCallerViews
        public async Task<IActionResult> Index()
        {
            return View(await _context.ViewAsteriskCaller.ToListAsync());
        }

        // GET: AsteriskCallerViews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asteriskCallerView = await _context.ViewAsteriskCaller
                .FirstOrDefaultAsync(m => m.Id == id);
            if (asteriskCallerView == null)
            {
                return NotFound();
            }

            return View(asteriskCallerView);
        }

    }
}
