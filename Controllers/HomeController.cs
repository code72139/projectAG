using Microsoft.AspNetCore.Mvc;
using Project_AG.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Project_AG.Filters;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;

namespace Project_AG.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UsuarioLogeado") != null)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            var user = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == username);
            if (user != null)
            {
                var hashedInput = HashPassword(password, username);
                if (hashedInput == user.Contrasena)
                {
                    HttpContext.Session.SetString("UsuarioLogeado", username);
                    return RedirectToAction("Dashboard");
                }
            }
            ViewData["Error"] = "Credenciales inválidas";
            return View();
        }

        [Autenticado]
        public async Task<IActionResult> Dashboard()
        {
            var now = DateTime.Now;
            var primerDiaMes = new DateTime(now.Year, now.Month, 1);
            var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);

            var gastosMes = await _context.RegistrosGasto
                .Where(r => r.Fecha >= primerDiaMes && r.Fecha <= ultimoDiaMes)
                .SelectMany(r => r.DetallesGasto)
                .SumAsync(d => d.Monto);

            var presupuestoTotal = await _context.PresupuestosTipoGasto
                .Where(p => p.Mes == now.Month && p.Anio == now.Year)
                .SumAsync(p => p.MontoPresupuestado);
            var presupuestoDisponible = presupuestoTotal - gastosMes;

            var fondosConDetalles = await _context.FondosMonetarios
                .Include(f => f.Depositos)
                .Include(f => f.RegistrosGasto)
                    .ThenInclude(rg => rg.DetallesGasto)
                .ToListAsync();

            decimal totalFondos = fondosConDetalles.Sum(fondo =>
            {
                decimal totalDepositosFondo = fondo.Depositos?.Sum(d => d.Monto) ?? 0;
                decimal totalGastosFondo = fondo.RegistrosGasto?.SelectMany(rg => rg.DetallesGasto).Sum(dg => dg.Monto) ?? 0;

                return totalDepositosFondo - totalGastosFondo;
            });

            var registrosPendientes = await _context.RegistrosGasto
                .CountAsync(r => r.Fecha >= now.AddDays(-5));

            ViewBag.GastosMes = gastosMes;
            ViewBag.PresupuestoDisponible = presupuestoDisponible;
            ViewBag.TotalFondos = totalFondos;
            ViewBag.RegistrosPendientes = registrosPendientes;

            return View("~/Views/Shared/Dashboard.cshtml");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        private string HashPassword(string password, string salt)
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

            byte[] hashed = KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(hashed);
        }
    }
}