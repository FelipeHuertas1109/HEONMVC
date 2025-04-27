using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiTiendaMVC.Data;
using MiTiendaMVC.Models;
using MiTiendaMVC.Models.ViewModels;


namespace MiTiendaMVC.Controllers
{
    public class CanastasController : Controller
    {
        private readonly MiTiendaContext _context;

        public CanastasController(MiTiendaContext context)
        {
            _context = context;
        }

        // GET: Canastas
        public async Task<IActionResult> Index()
        {
            var miTiendaContext = _context.Canastas.Include(c => c.Cliente);
            return View(await miTiendaContext.ToListAsync());
        }

        // GET: Canastas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var canasta = await _context.Canastas
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(m => m.CanastaId == id);
            if (canasta == null)
            {
                return NotFound();
            }

            return View(canasta);
        }

        public IActionResult Create()
        {
            var vm = new CanastaViewModel
            {
                ProductosList = new MultiSelectList(_context.Productos, "ProductoId", "Nombre")
            };
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nombre");
            return View(vm);
        }
        // POST: Canastas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CanastaViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // recarga listas antes de volver a la vista
                vm.ProductosList = new MultiSelectList(_context.Productos, "ProductoId", "Nombre", vm.ProductosSeleccionados);
                ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nombre", vm.ClienteId);
                return View(vm);
            }

            var canasta = new Canasta
            {
                ClienteId = vm.ClienteId,
                Productos = await _context.Productos
                    .Where(p => vm.ProductosSeleccionados.Contains(p.ProductoId))
                    .ToListAsync()
            };
            canasta.Total = canasta.Productos.Sum(p => p.Precio);

            _context.Add(canasta);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var canasta = await _context.Canastas
                                 .Include(c => c.Productos)
                                 .FirstOrDefaultAsync(c => c.CanastaId == id);
            if (canasta == null) return NotFound();

            var vm = new CanastaViewModel
            {
                CanastaId = canasta.CanastaId,
                ClienteId = canasta.ClienteId,
                ProductosSeleccionados = canasta.Productos.Select(p => p.ProductoId).ToArray(),
                ProductosList = new MultiSelectList(_context.Productos, "ProductoId", "Nombre",
                                                    canasta.Productos.Select(p => p.ProductoId))
            };
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nombre", vm.ClienteId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CanastaViewModel vm)
        {
            if (id != vm.CanastaId) return NotFound();
            if (ModelState.IsValid)
            {
                var canastaDb = await _context.Canastas
                                       .Include(c => c.Productos)
                                       .FirstAsync(c => c.CanastaId == id);
                canastaDb.ClienteId = vm.ClienteId;
                canastaDb.Productos.Clear();
                var productos = await _context.Productos
                    .Where(p => vm.ProductosSeleccionados.Contains(p.ProductoId))
                    .ToListAsync();
                foreach (var p in productos) canastaDb.Productos.Add(p);
                canastaDb.Total = productos.Sum(p => p.Precio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // En caso de error
            vm.ProductosList = new MultiSelectList(_context.Productos, "ProductoId", "Nombre", vm.ProductosSeleccionados);
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nombre", vm.ClienteId);
            return View(vm);
        }


        // GET: Canastas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var canasta = await _context.Canastas
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(m => m.CanastaId == id);
            if (canasta == null)
            {
                return NotFound();
            }

            return View(canasta);
        }

        // POST: Canastas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var canasta = await _context.Canastas.FindAsync(id);
            if (canasta != null)
            {
                _context.Canastas.Remove(canasta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CanastaExists(int id)
        {
            return _context.Canastas.Any(e => e.CanastaId == id);
        }
    }
}
