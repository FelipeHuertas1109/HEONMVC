using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;           // ← para ILogger
using MiTiendaMVC.Data;
using MiTiendaMVC.Models;
using MiTiendaMVC.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MiTiendaMVC.Controllers
{
    public class CanastasController : Controller
    {
        private readonly MiTiendaContext _context;
        private readonly ILogger<CanastasController> _logger;   // ← logger

        public CanastasController(MiTiendaContext context, ILogger<CanastasController> logger)
        {
            _context = context;
            _logger = logger;                                 // ← inyectado
        }

        // GET: Canastas
        public async Task<IActionResult> Index()
        {
            var lista = await _context.Canastas
                                      .Include(c => c.Cliente)
                                      .Include(c => c.Productos)
                                      .ToListAsync();

            _logger.LogInformation("INDEX ► {Count} canastas cargadas", lista.Count);
            return View(lista);
        }

        // GET: Canastas/Create
        public IActionResult Create()
        {
            var vm = new CanastaViewModel
            {
                ProductosList = new MultiSelectList(_context.Productos, "ProductoId", "Nombre")
            };

            ViewBag.ClienteId = new SelectList(_context.Clientes, "ClienteId", "Nombre");

            _logger.LogInformation("CREATE GET ► Clientes={Cli}, Productos={Prod}",
                                   _context.Clientes.Count(),
                                   _context.Productos.Count());

            return View(vm);
        }

        // POST: Canastas/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CanastaViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                /* ←––– AQUI MISMO ––––––––––––––––––––––– */
                var errores = string.Join(" | ",
                              ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage));
                _logger.LogWarning("CREATE ModelState INVALID ► {Errores}", errores);

                // recarga listas antes de volver a la vista
                vm.ProductosList = new MultiSelectList(_context.Productos,
                                                       "ProductoId", "Nombre",
                                                       vm.ProductosSeleccionados);
                ViewBag.ClienteId = new SelectList(_context.Clientes,
                                                   "ClienteId", "Nombre",
                                                   vm.ClienteId);
                return View(vm);
            }
            _logger.LogInformation("CREATE POST ► ClienteId={Cli}, Seleccionados=[{Sel}]",
                                   vm.ClienteId,
                                   string.Join(",", vm.ProductosSeleccionados));

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Modelo inválido. Revisa los campos.";
                vm.ProductosList = new MultiSelectList(_context.Productos, "ProductoId", "Nombre", vm.ProductosSeleccionados);
                ViewBag.ClienteId = new SelectList(_context.Clientes, "ClienteId", "Nombre", vm.ClienteId);
                return View(vm);
            }

            var productos = await _context.Productos
                                    .Where(p => vm.ProductosSeleccionados.Contains(p.ProductoId))
                                    .ToListAsync();

            var canasta = new Canasta
            {
                ClienteId = vm.ClienteId,
                Productos = productos,
                Total = productos.Sum(p => p.Precio)
            };

            _context.Add(canasta);
            await _context.SaveChangesAsync();

            TempData["Ok"] = $"Canasta #{canasta.CanastaId} creada con {productos.Count} productos (Total: {canasta.Total:C}).";
            _logger.LogInformation("CREATE OK ► Id={Id}, Total={Total:C}", canasta.CanastaId, canasta.Total);

            return RedirectToAction(nameof(Index));
        }

        // GET: Canastas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();

            var canasta = await _context.Canastas
                                        .Include(c => c.Productos)
                                        .FirstOrDefaultAsync(c => c.CanastaId == id);

            if (canasta is null) return NotFound();

            var vm = new CanastaViewModel
            {
                CanastaId = canasta.CanastaId,
                ClienteId = canasta.ClienteId,
                ProductosSeleccionados = canasta.Productos.Select(p => p.ProductoId).ToArray(),
                ProductosList = new MultiSelectList(_context.Productos, "ProductoId", "Nombre",
                                                            canasta.Productos.Select(p => p.ProductoId))
            };

            ViewBag.ClienteId = new SelectList(_context.Clientes, "ClienteId", "Nombre", vm.ClienteId);
            _logger.LogInformation("EDIT GET ► Id={Id}, ProductosSel=[{Sel}]", vm.CanastaId, string.Join(",", vm.ProductosSeleccionados));
            return View(vm);
        }

        // POST: Canastas/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CanastaViewModel vm)
        {
            if (id != vm.CanastaId) return NotFound();

            _logger.LogInformation("EDIT POST ► Id={Id}, Cliente={Cli}, Seleccionados=[{Sel}]",
                                   id, vm.ClienteId, string.Join(",", vm.ProductosSeleccionados));

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Modelo inválido.";
                vm.ProductosList = new MultiSelectList(_context.Productos, "ProductoId", "Nombre", vm.ProductosSeleccionados);
                ViewBag.ClienteId = new SelectList(_context.Clientes, "ClienteId", "Nombre", vm.ClienteId);
                return View(vm);
            }

            var canastaDb = await _context.Canastas
                                          .Include(c => c.Productos)
                                          .FirstAsync(c => c.CanastaId == id);

            canastaDb.ClienteId = vm.ClienteId;

            // Reemplazar productos
            canastaDb.Productos.Clear();
            var nuevos = await _context.Productos
                                       .Where(p => vm.ProductosSeleccionados.Contains(p.ProductoId))
                                       .ToListAsync();
            foreach (var p in nuevos) canastaDb.Productos.Add(p);

            canastaDb.Total = nuevos.Sum(p => p.Precio);
            await _context.SaveChangesAsync();

            TempData["Ok"] = $"Canasta #{id} actualizada. Nuevo total: {canastaDb.Total:C}.";
            _logger.LogInformation("EDIT OK ► Id={Id}, Total={Total:C}", id, canastaDb.Total);

            return RedirectToAction(nameof(Index));
        }

        // GET: Canastas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();

            var canasta = await _context.Canastas
                                        .Include(c => c.Cliente)
                                        .FirstOrDefaultAsync(c => c.CanastaId == id);
            if (canasta is null) return NotFound();

            return View(canasta);
        }

        // POST: Canastas/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var canasta = await _context.Canastas.FindAsync(id);
            if (canasta != null) _context.Canastas.Remove(canasta);

            await _context.SaveChangesAsync();
            TempData["Ok"] = $"Canasta #{id} eliminada.";
            _logger.LogInformation("DELETE OK ► Id={Id}", id);

            return RedirectToAction(nameof(Index));
        }
    }
}
