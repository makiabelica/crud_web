using crudangieespinoza.Models;
using crudangieespinoza.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace crudangieespinoza.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbcrudcoreContext _DBContext;

        public HomeController(DbcrudcoreContext context)
        {
            _DBContext = context;
        }

        public IActionResult Index()
        {
            List<Empleado> lista = _DBContext.Empleados.Include(c => c.oCargo).ToList();
            return View(lista);
        }

        [HttpGet]
        //CREATE
        public IActionResult Empleado_Detalle(int? idEmpleado)
        {
            EmpleadoVM oEmpleadoVM = new EmpleadoVM();

            if (idEmpleado.HasValue && idEmpleado.Value > 0)
            {
                // Editing an existing employee
                var empleado = _DBContext.Empleados.Find(idEmpleado.Value);
                if (empleado != null)
                {
                    oEmpleadoVM.oEmpleado = empleado;
                }
            }
            else
            {
                // Creating a new employee
                oEmpleadoVM.oEmpleado = new Empleado();
            }

            oEmpleadoVM.oListaCargo = _DBContext.Cargos.Select(cargo => new SelectListItem()
            {
                Text = cargo.Descripcion,
                Value = cargo.IdCargo.ToString()
            }).ToList();

            return View(oEmpleadoVM);
        }

        [HttpPost]
        //UPDATE
        public IActionResult Empleado_Detalle(EmpleadoVM oEmpleadoVM)
        {
            if (oEmpleadoVM.oEmpleado.IdEmpleado == 0)
            {
                // Create a new employee
                _DBContext.Empleados.Add(oEmpleadoVM.oEmpleado);
            }
            else
            {
                // Update existing employee
                var empleadoExistente = _DBContext.Empleados.Find(oEmpleadoVM.oEmpleado.IdEmpleado);
                if (empleadoExistente != null)
                {
                    empleadoExistente.NombreCompleto = oEmpleadoVM.oEmpleado.NombreCompleto;
                    empleadoExistente.Correo = oEmpleadoVM.oEmpleado.Correo;
                    empleadoExistente.Telefono = oEmpleadoVM.oEmpleado.Telefono;
                    empleadoExistente.IdCargo = oEmpleadoVM.oEmpleado.IdCargo;

                    _DBContext.Empleados.Update(empleadoExistente);
                }
            }

            _DBContext.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        //DELETE
        public IActionResult Eliminar(int idEmpleado)
        {
            var empleado = _DBContext.Empleados.Find(idEmpleado);
            if (empleado != null)
            {
                _DBContext.Empleados.Remove(empleado);
                _DBContext.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
