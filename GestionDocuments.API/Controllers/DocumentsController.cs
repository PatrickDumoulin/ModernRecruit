
using DocuSign.eSign.Model;
using ModernRecrut.Documents.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ModernRecrut.Documents.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public DocumentsController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet("{userId}")]
        public IActionResult GetDocuments(string userId)
        {
            var documentsFolderPath = Path.Combine(_env.WebRootPath, "documents");
            var userDocuments = Directory.EnumerateFiles(documentsFolderPath)
                                         .Where(file => Path.GetFileName(file).StartsWith(userId))
                                         .Select(file => Path.GetFileName(file))
                                         .ToList();

            List<DocumentModel> documents = new List<DocumentModel>();

            foreach (var document in userDocuments)
            {
                var parts = document.Split('_');
                var newDocument = new DocumentModel
                {
                    Id = Path.GetFileNameWithoutExtension(parts[2]),
                    Type = parts[1],
                    Chemin = document
                };
                documents.Add(newDocument);
            }

            return Ok(documents);
        }

        [HttpGet("{userId}/{chemin}")]
        public IActionResult GetDocumentById(string userId, string chemin)
        {
            var documentsFolderPath = Path.Combine(_env.WebRootPath, "documents");
            var filePath = Directory.EnumerateFiles(documentsFolderPath)
                            .FirstOrDefault(file => Path.GetFileName(file) == chemin);


            if (string.IsNullOrEmpty(filePath))
            {
                return NotFound();
            }

            var fileName = Path.GetFileName(filePath);
            var parts = fileName.Split('_');

            if (parts.Length >= 3)
            {
                var document = new DocumentModel
                {
                    Id = Path.GetFileNameWithoutExtension(parts[2]),
                    Type = parts[1],
                    Chemin = fileName
                };

                return Ok(document);
            }

            return NotFound();
        }

        // POST api/<GestionDocumentsController>
        [HttpPost("{userId}/{documentType}")]
        public async Task<IActionResult> PostDocument(string userId, string documentType, [FromForm] IFormFile file)
        {
            if (file != null)
            {
                // Vérifier que le fichier est bien un document Word ou PDF
                var allowedExtensions = new[] { ".doc", ".docx", ".pdf" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest("Seuls les fichiers Word et PDF sont autorisés.");
                }

                // Générer un numéro aléatoire
                Random random = new Random();
                int randomNumber = random.Next(1, 101);
                string randomNumberString = randomNumber.ToString();

                // Créer le nom du fichier
                string fileName = $"{userId}_{documentType}_{randomNumberString}{extension}";
                string filePath = Path.Combine(_env.WebRootPath, "documents", fileName);

                // Sauvegarder le fichier dans le système de fichiers
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return Ok(new { FileName = fileName, FilePath = filePath });
            }

            return BadRequest("Aucun fichier fourni");
        }

        // DELETE api/<GestionDocumentsController>/5
        [HttpDelete("{chemin}")]
        public IActionResult DeleteDocument(string chemin)
        {
            var documentsFolderPath = Path.Combine(_env.WebRootPath, "documents");
            var filePath = Path.Combine(documentsFolderPath, chemin);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                return Ok($"Le document {chemin} a été supprimé avec succès.");
            }

            return NotFound($"Le document {chemin} n'a pas été trouvé.");
        }

    }
}
