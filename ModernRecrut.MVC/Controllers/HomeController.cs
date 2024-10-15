using Microsoft.AspNetCore.Mvc;
using ModernRecrut.MVC.Models;
using System.Diagnostics;

namespace ModernRecrut.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
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

        public IActionResult CodeStatus(int code)
        {
            CodeStatusViewModel codeStatusViewModel = new CodeStatusViewModel();

            codeStatusViewModel.IdRequete = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            codeStatusViewModel.CodeStatus = code;

            if (code == 404)
            {
                codeStatusViewModel.MessageErreur = "\"Oops ! La page que vous recherchez semble avoir pris des vacances. Nous ne l'avons pas trouvée sur notre serveur. Assurez-vous d'avoir saisi l'URL correcte, " +
                                                    "ou explorez nos autres pages pour trouver ce que vous cherchez. Si le problème persiste, " +
                                                    "n'hésitez pas à contacter notre équipe d'assistance pour obtenir de l'aide.\"";
                
                _logger.LogWarning("Erreur {CodeStatus} survenue avec la requête {IdRequete}", code, codeStatusViewModel.IdRequete);
                return View("NotFound", codeStatusViewModel);
            }


            else if (code >= 400 && code < 500)
            {
                codeStatusViewModel.MessageErreur = "Désolée, une erreur est survenue";
                _logger.LogWarning("Erreur {CodeStatus} survenue avec la requête {IdRequete}", code, codeStatusViewModel.IdRequete);
                return View(codeStatusViewModel);
            }

            else if (code >= 500 && code < 600)
            {
                codeStatusViewModel.MessageErreur = "Désolée, une erreur critique est survenue";
                _logger.LogCritical("Erreur {CodeStatus} survenue avec la requête {IdRequete}", code, codeStatusViewModel.IdRequete);
                return View(codeStatusViewModel);
            }
            
            else
            {
                codeStatusViewModel.MessageErreur = "Désolée, une erreur est survenue";
            }

            return View(codeStatusViewModel);
        }
    }
}