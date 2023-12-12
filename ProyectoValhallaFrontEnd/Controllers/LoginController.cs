using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProyectoValhallaFrontEnd.Models;
using System.Security.Claims;
using System.Text;

namespace ProyectoValhallaFrontEnd.Controllers
{
    
    public class LoginController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(Credenciales model)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = "http://localhost:5188/api/Autenticacion/Autenticar";

                string jsonContent = JsonConvert.SerializeObject(model);

                StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var json_respuesta = await response.Content.ReadAsStringAsync();
                    var resultado = JsonConvert.DeserializeObject<ResultadoToken>(json_respuesta);

                    

                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim("Token", resultado.Token),
                        new Claim("Refresh", resultado.RefreshToken), 
                    };

                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    AuthenticationProperties properties = new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        properties
                        );
                    

                    return RedirectToAction("Index", "Usuario");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error de autenticación");
                }
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
