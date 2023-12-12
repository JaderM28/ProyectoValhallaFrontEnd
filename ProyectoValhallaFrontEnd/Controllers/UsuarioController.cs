using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProyectoValhallaFrontEnd.Models;
using System.Net.Http.Headers;
using System.Text;
using X.PagedList;

namespace ProyectoValhallaFrontEnd.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {


        public ActionResult Home()
        {
            return View();
        }
        public async Task<IActionResult> Index(string buscar, string filtro, int? pagina)
        {
            int tamañoPagina = 7;
            int numeroPagina = pagina ?? 1;

            string CodigoAsc = "CodigoAscendente";
            string CodigoDes = "CodigoDescendente";
            string NombreAsc = "NombreAscendente";
            string NombreDes = "NombreDescendente";

            List<UsuarioDTO> lista = new List<UsuarioDTO>();

            var accessToken = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            var usuario = new HttpClient();

            usuario.BaseAddress = new Uri("http://localhost:5188/api/Usuarios");
            usuario.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await usuario.GetAsync(usuario.BaseAddress);

            if (response.IsSuccessStatusCode)
            {
                var json_respuesta = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<resultUsuario>(json_respuesta);
                lista = resultado.Response;

                if (!String.IsNullOrEmpty(buscar))
                {
                    lista = lista.Where(usuario => usuario.Username.Contains(buscar)).ToList();
                }

                ViewData["FiltroCodigo"] = filtro == CodigoAsc ? CodigoDes : CodigoAsc;
                ViewData["FiltroNombre"] = filtro == NombreAsc ? NombreDes : NombreAsc;

                if (filtro == NombreDes)
                {
                    lista = lista.OrderByDescending(s => s.Nombres).ToList();
                }
                else if (filtro == CodigoDes)
                {
                    lista = lista.OrderByDescending(s => s.IdUsuario).ToList();
                }
                else if (filtro == NombreAsc)
                {
                    lista = lista.OrderBy(s => s.Nombres).ToList();
                }
                else if (filtro == CodigoAsc)
                {
                    lista = lista.OrderBy(s => s.IdUsuario).ToList();
                }
                
                return View(await lista.ToPagedListAsync(numeroPagina, tamañoPagina));
            }

            return View(new List<UsuarioDTO>());
        }

        public async Task<IActionResult> Obtener(int idUsuario)
        {
            UsuarioDTO objeto = new UsuarioDTO();

            var accessToken = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            var usuario = new HttpClient();

            usuario.BaseAddress = new Uri($"http://localhost:5188/api/Usuarios/{idUsuario}");
            usuario.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await usuario.GetAsync(usuario.BaseAddress);

            if (response.IsSuccessStatusCode)
            {
                var json_respuesta = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<UsuarioDTO>(json_respuesta);
                objeto = resultado;
            }

            return View(objeto);
        }

        public IActionResult Guardar()
        {
            return View();
        }

        [HttpPost ]
        public async Task<IActionResult> Guardar(UsuarioDTO objeto)
        {
            bool respuesta = false;

            var accessToken = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            var usuario = new HttpClient();

            var content = new StringContent(JsonConvert.SerializeObject(objeto), Encoding.UTF8, "application/json");

            usuario.BaseAddress = new Uri($"http://localhost:5188/api/Usuarios");
            usuario.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await usuario.PostAsync(usuario.BaseAddress, content);

            if (response.IsSuccessStatusCode)
            {
                respuesta = true;
                return RedirectToAction("Index");
            }

            return View();
        }

        public async Task<IActionResult> Editar(int id)
        {
            UsuarioDTO objeto = new UsuarioDTO();

            var accessToken = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            var usuario = new HttpClient();

            usuario.BaseAddress = new Uri($"http://localhost:5188/api/Usuarios/{id}");
            usuario.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await usuario.GetAsync(usuario.BaseAddress);

            if (response.IsSuccessStatusCode)
            {
                var json_respuesta = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<resultUsuario>(json_respuesta);
                objeto = resultado.Objeto; // Cambia a Response
                return View(objeto);
            }

            return View(objeto);

        }
        
        [HttpPost]
        public async Task<IActionResult> Editar(UsuarioDTO objeto, int id)
        {
            bool respuesta = false;

            var accessToken = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            var usuario = new HttpClient();

            var content = new StringContent(JsonConvert.SerializeObject(objeto), Encoding.UTF8, "application/json");

            usuario.BaseAddress = new Uri($"http://localhost:5188/api/Usuarios/{id}");
            usuario.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await usuario.PutAsync(usuario.BaseAddress, content);

            if (response.IsSuccessStatusCode)
            {
                respuesta = true;
                return RedirectToAction("Index");
            }

            return View();
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            UsuarioDTO objeto = new UsuarioDTO();

            var accessToken = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            var usuario = new HttpClient();

            usuario.BaseAddress = new Uri($"http://localhost:5188/api/Usuarios/{id}");
            usuario.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await usuario.GetAsync(usuario.BaseAddress);

            if (response.IsSuccessStatusCode)
            {
                var json_respuesta = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<resultUsuario>(json_respuesta);
                objeto = resultado.Objeto;
                return View(objeto);
            }

            return View(objeto);
        }


        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            bool respuesta = false;

            var accessToken = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            var usuario = new HttpClient();

            usuario.BaseAddress = new Uri($"http://localhost:5188/api/Usuarios/{id}");
            usuario.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await usuario.DeleteAsync(usuario.BaseAddress);

            if (response.IsSuccessStatusCode)
            {
                respuesta = true;
            }

            return RedirectToAction("Index");
        }


    }
}
