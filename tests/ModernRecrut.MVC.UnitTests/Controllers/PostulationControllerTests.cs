using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModernRecrut.MVC.Areas.Identity.Data;
using ModernRecrut.MVC.Controllers;
using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Interface;
using ModernRecrut.MVC.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace ModernRecrut.MVC.UnitTests.Controllers
{
    public class PostulationControllerTests
    {
        #region Postuler HTTPPost
        [Fact]
        public async Task Postuler_QuandAucunCV_RetourneVueAvecErreur()
        {
            // ETANT DONNEE
            var fixture = new Fixture();
            var mockDocumentsService = new Mock<IDocumentsService>();
            var mockPostulationsService = new Mock<IPostulationsService>();
            var mockLogger = new Mock<ILogger<PostulationsController>>();
            var mockNoteService = new Mock<INoteService>();

            // Créer un objet RequetePostulation avec un ID candidat prédéfini
            var requetePostulation = fixture.Build<RequetePostulation>()
                                            .With(r => r.IdCandidat, "idCandidat")
                                            .With(r => r.OffreDEmploiId, 1)
                                            .With(r => r.DateDisponibilite, DateTime.Now.AddDays(1))
                                            .With(r => r.PretentionSalariale, 100000)
                                            .Create();
 
            var documentsSansCV = new List<DocumentModel>()
            {
                
                new DocumentModel()
                {
                    Id = "idLettreMotivation",
                    Type = "LettreDeMotivation",
                    Chemin = "cheminLettreMotivation"
                }   
            };

            // Simuler l'absence de CV
            mockDocumentsService.Setup(ds => ds.ObtenirTout(requetePostulation.IdCandidat))
                                .ReturnsAsync(documentsSansCV);

            var controller = new PostulationsController(
                null, 
                null, 
                mockLogger.Object,
                mockDocumentsService.Object,
                mockPostulationsService.Object,
                mockNoteService.Object
                );


            // LORSQUE
            var viewResult = await controller.Postuler(requetePostulation) as ViewResult;

            // ALORS
            viewResult.Should().NotBeNull();
            var requetePostulationResult = viewResult.Model as RequetePostulation;
            controller.ModelState.Count.Should().Be(1);
            controller.ModelState.Should().ContainKey("Error");
            controller.ModelState["Error"].Errors.Should().NotBeEmpty();
            controller.ModelState["Error"].Errors.FirstOrDefault()
                .ErrorMessage.Should().Be("Un CV est requis pour postuler. Veuillez télécharger d'abord un CV dans votre espace Documents.");

        }

        [Fact]
        public async Task Postuler_QuandAucuneLettreMotivation_RetourneVueAvecErreur()
        {
            // ETANT DONNEE
            var fixture = new Fixture();
            var mockDocumentsService = new Mock<IDocumentsService>();
            var mockPostulationsService = new Mock<IPostulationsService>();
            var mockLogger = new Mock<ILogger<PostulationsController>>();
            var mockNoteService = new Mock<INoteService>();

            // Créer un objet RequetePostulation avec un ID candidat prédéfini
            var requetePostulation = fixture.Build<RequetePostulation>()
                                            .With(r => r.IdCandidat, "idCandidat")
                                            .With(r => r.OffreDEmploiId, 1)
                                            .With(r => r.DateDisponibilite, DateTime.Now.AddDays(1))
                                            .With(r => r.PretentionSalariale, 100000)
                                            .Create();

            var documentsSansLettreMotivation = new List<DocumentModel>()
            {

                new DocumentModel()
                {
                    Id = "idCV",
                    Type = "CV",
                    Chemin = "cheminCV"
                }
            };

            // Simuler l'absence de Lettre de motivation
            mockDocumentsService.Setup(ds => ds.ObtenirTout(requetePostulation.IdCandidat))
                                .ReturnsAsync(documentsSansLettreMotivation);

            var controller = new PostulationsController(
                null,
                null,
                mockLogger.Object,
                mockDocumentsService.Object,
                mockPostulationsService.Object,
                mockNoteService.Object

                );


            // LORSQUE
            var viewResult = await controller.Postuler(requetePostulation) as ViewResult;

            // ALORS
            viewResult.Should().NotBeNull();
            var requetePostulationResult = viewResult.Model as RequetePostulation;
            controller.ModelState.Count.Should().Be(1);
            controller.ModelState.Should().ContainKey("Error");
            controller.ModelState["Error"].Errors.Should().NotBeEmpty();
            controller.ModelState["Error"].Errors.FirstOrDefault()
                .ErrorMessage.Should().Be("Une lettre de motivation est requise pour postuler. Veuillez télécharger une lettre de motivation dans votre espace Documents.");

        }

        [Fact]
        public async Task Postuler_QuandDateDispoInfDateDuJour_RetourneVueAvecErreur()
        {
            // ETANT DONNEE
            var fixture = new Fixture();
            var mockDocumentsService = new Mock<IDocumentsService>();
            var mockPostulationsService = new Mock<IPostulationsService>();
            var mockLogger = new Mock<ILogger<PostulationsController>>();
            var mockNoteService = new Mock<INoteService>();

            // Créer un objet RequetePostulation
            var requetePostulation = fixture.Build<RequetePostulation>()
                                            .With(r => r.IdCandidat, "idCandidat")
                                            .With(r => r.OffreDEmploiId, 1)
                                            .With(r => r.DateDisponibilite, DateTime.Now.AddDays(-2))
                                            .With(r => r.PretentionSalariale, 100000)
                                            .Create();

            var documentsAvecCVetLettre = new List<DocumentModel>()

            {
                new DocumentModel() { Id = "idCV", Type = "CV", Chemin = "cheminCV" },
                new DocumentModel() { Id = "idLettreMotivation", Type = "LettreDeMotivation", Chemin = "cheminLettreMotivation" }
            };

            
            mockDocumentsService.Setup(ds => ds.ObtenirTout(requetePostulation.IdCandidat))
                                .ReturnsAsync(documentsAvecCVetLettre);

            var controller = new PostulationsController(
                null,
                null,
                mockLogger.Object,
                mockDocumentsService.Object,
                mockPostulationsService.Object,
                mockNoteService.Object
                );


            // LORSQUE
            var viewResult = await controller.Postuler(requetePostulation) as ViewResult;

            // ALORS
            viewResult.Should().NotBeNull();
            var requetePostulationResult = viewResult.Model as RequetePostulation;
            controller.ModelState.Count.Should().Be(1);
            controller.ModelState.Should().ContainKey("DateDisponibilite");
            controller.ModelState["DateDisponibilite"].Errors.Should().NotBeEmpty();
            controller.ModelState["DateDisponibilite"].Errors.FirstOrDefault()
                .ErrorMessage.Should().Be("La date de disponibilité doit être supérieur ou égale à la date du jour");

        }

        [Fact]
        public async Task Postuler_QuandDateDispoSupDateDuJourPlus45Jours_RetourneVueAvecErreur()
        {
            // ETANT DONNEE
            var fixture = new Fixture();
            var mockDocumentsService = new Mock<IDocumentsService>();
            var mockPostulationsService = new Mock<IPostulationsService>();
            var mockLogger = new Mock<ILogger<PostulationsController>>();
            var mockNoteService = new Mock<INoteService>();

            // Créer un objet RequetePostulation
            var requetePostulation = fixture.Build<RequetePostulation>()
                                            .With(r => r.IdCandidat, "idCandidat")
                                            .With(r => r.OffreDEmploiId, 1)
                                            .With(r => r.DateDisponibilite, DateTime.Now.AddDays(100))
                                            .With(r => r.PretentionSalariale, 100000)
                                            .Create();

            var documentsAvecCVetLettre = new List<DocumentModel>()

            {
                new DocumentModel() { Id = "idCV", Type = "CV", Chemin = "cheminCV" },
                new DocumentModel() { Id = "idLettreMotivation", Type = "LettreDeMotivation", Chemin = "cheminLettreMotivation" }
            };

            
            mockDocumentsService.Setup(ds => ds.ObtenirTout(requetePostulation.IdCandidat))
                                .ReturnsAsync(documentsAvecCVetLettre);

            var controller = new PostulationsController(
                null,
                null,
                mockLogger.Object,
                mockDocumentsService.Object,
                mockPostulationsService.Object,
                mockNoteService.Object
                );


            // LORSQUE
            var viewResult = await controller.Postuler(requetePostulation) as ViewResult;

            // ALORS
            viewResult.Should().NotBeNull();
            var requetePostulationResult = viewResult.Model as RequetePostulation;
            controller.ModelState.Count.Should().Be(1);
            controller.ModelState.Should().ContainKey("DateDisponibilite");
            controller.ModelState["DateDisponibilite"].Errors.Should().NotBeEmpty();
            controller.ModelState["DateDisponibilite"].Errors.FirstOrDefault()
                .ErrorMessage.Should().Be("La date de disponibilité doit être inférieur ou égale à la date du jour + 45 jours");

        }

        [Fact]
        public async Task Postuler_QuandPretentionSalarialeSup150000_RetourneVueAvecErreur()
        {
            // ETANT DONNEE
            var fixture = new Fixture();
            var mockDocumentsService = new Mock<IDocumentsService>();
            var mockPostulationsService = new Mock<IPostulationsService>();
            var mockLogger = new Mock<ILogger<PostulationsController>>();
            var mockNoteService = new Mock<INoteService>();

            // Créer un objet RequetePostulation
            var requetePostulation = fixture.Build<RequetePostulation>()
                                            .With(r => r.IdCandidat, "idCandidat")
                                            .With(r => r.OffreDEmploiId, 1)
                                            .With(r => r.DateDisponibilite, DateTime.Now.AddDays(20))
                                            .With(r => r.PretentionSalariale, 160000)
                                            .Create();

            var documentsAvecCVetLettre = new List<DocumentModel>()

            {
                new DocumentModel() { Id = "idCV", Type = "CV", Chemin = "cheminCV" },
                new DocumentModel() { Id = "idLettreMotivation", Type = "LettreDeMotivation", Chemin = "cheminLettreMotivation" }
            };

            
            mockDocumentsService.Setup(ds => ds.ObtenirTout(requetePostulation.IdCandidat))
                                .ReturnsAsync(documentsAvecCVetLettre);

            var controller = new PostulationsController(
                null,
                null,
                mockLogger.Object,
                mockDocumentsService.Object,
                mockPostulationsService.Object,
                mockNoteService.Object
                );


            // LORSQUE
            var viewResult = await controller.Postuler(requetePostulation) as ViewResult;

            // ALORS
            viewResult.Should().NotBeNull();
            var requetePostulationResult = viewResult.Model as RequetePostulation;
            controller.ModelState.Count.Should().Be(1);
            controller.ModelState.Should().ContainKey("PretentionSalariale");
            controller.ModelState["PretentionSalariale"].Errors.Should().NotBeEmpty();
            controller.ModelState["PretentionSalariale"].Errors.FirstOrDefault()
                .ErrorMessage.Should().Be("Votre prétention salariale est au-delà de nos limites");

        }

        [Fact]
        public async Task Postuler_QuandIdCandidatNullOrEmpty_RetourneVueAvecErreur()
        {
            // ETANT DONNEE
            var fixture = new Fixture();
            var mockDocumentsService = new Mock<IDocumentsService>();
            var mockPostulationsService = new Mock<IPostulationsService>();
            var mockLogger = new Mock<ILogger<PostulationsController>>();
            var mockNoteService = new Mock<INoteService>();

            // Créer un objet RequetePostulation
            var requetePostulation = fixture.Build<RequetePostulation>()
                                            .With(r => r.IdCandidat, "")
                                            .With(r => r.OffreDEmploiId, 1)
                                            .With(r => r.DateDisponibilite, DateTime.Now.AddDays(20))
                                            .With(r => r.PretentionSalariale, 120000)
                                            .Create();

            var documentsAvecCVetLettre = new List<DocumentModel>()

            {
                new DocumentModel() { Id = "idCV", Type = "CV", Chemin = "cheminCV" },
                new DocumentModel() { Id = "idLettreMotivation", Type = "LettreDeMotivation", Chemin = "cheminLettreMotivation" }
            };

            
            mockDocumentsService.Setup(ds => ds.ObtenirTout(requetePostulation.IdCandidat))
                                .ReturnsAsync(documentsAvecCVetLettre);

            var controller = new PostulationsController(
                null,
                null,
                mockLogger.Object,
                mockDocumentsService.Object,
                mockPostulationsService.Object,
                mockNoteService.Object
                );


            // LORSQUE
            var viewResult = await controller.Postuler(requetePostulation) as ViewResult;

            // ALORS
            viewResult.Should().NotBeNull();
            var requetePostulationResult = viewResult.Model as RequetePostulation;
            controller.ModelState.Count.Should().Be(1);
            controller.ModelState.Should().ContainKey("all");
            controller.ModelState["all"].Errors.Should().NotBeEmpty();
            controller.ModelState["all"].Errors.FirstOrDefault()
                .ErrorMessage.Should().Be("Problème lors de l'ajout de la postulation, veuillez reessayer!");

        }

        [Fact]
        public async Task Postuler_QuandPostulationNull_RetourneVueAvecErreur()
        {
            // ETANT DONNEE
            var fixture = new Fixture();
            var mockDocumentsService = new Mock<IDocumentsService>();
            var mockPostulationsService = new Mock<IPostulationsService>();
            var mockLogger = new Mock<ILogger<PostulationsController>>();
            var mockNoteService = new Mock<INoteService>();

            // Créer un objet RequetePostulation
            var requetePostulation = fixture.Build<RequetePostulation>()
                                            .With(r => r.IdCandidat, "idCandidat")
                                            .With(r => r.OffreDEmploiId, 1)
                                            .With(r => r.DateDisponibilite, DateTime.Now.AddDays(20))
                                            .With(r => r.PretentionSalariale, 120000)
                                            .Create();

            var documentsAvecCVetLettre = new List<DocumentModel>()

            {
                new DocumentModel() { Id = "idCV", Type = "CV", Chemin = "cheminCV" },
                new DocumentModel() { Id = "idLettreMotivation", Type = "LettreDeMotivation", Chemin = "cheminLettreMotivation" }
            };

            
            mockDocumentsService.Setup(ds => ds.ObtenirTout(requetePostulation.IdCandidat))
                                .ReturnsAsync(documentsAvecCVetLettre);

            // Simuler postulation null
            mockPostulationsService.Setup(service => service.Ajouter(It.IsAny<RequetePostulation>()))
                           .ReturnsAsync((Postulation)null);

            var controller = new PostulationsController(
                null,
                null,
                mockLogger.Object,
                mockDocumentsService.Object,
                mockPostulationsService.Object,
                mockNoteService.Object
                );


            // LORSQUE
            var viewResult = await controller.Postuler(requetePostulation) as ViewResult;

            // ALORS
            viewResult.Should().NotBeNull();
            var requetePostulationResult = viewResult.Model as RequetePostulation;
            controller.ModelState.Count.Should().Be(1);
            controller.ModelState.Should().ContainKey("all");
            controller.ModelState["all"].Errors.Should().NotBeEmpty();
            controller.ModelState["all"].Errors.FirstOrDefault()
                .ErrorMessage.Should().Be("Problème lors de l'ajout de la postulation, veuillez reessayer!");

        }

        [Fact]
        public async Task Postuler_QuandModelStateValide_RetourneRedirectToAction()
        {
            // ETANT DONNEE
            var fixture = new Fixture();
            var mockDocumentsService = new Mock<IDocumentsService>();
            var mockPostulationsService = new Mock<IPostulationsService>();
            var mockLogger = new Mock<ILogger<PostulationsController>>();
            var mockNoteService = new Mock<INoteService>();

            // Créer un objet RequetePostulation
            var requetePostulation = fixture.Build<RequetePostulation>()
                                            .With(r => r.IdCandidat, "idCandidat")
                                            .With(r => r.OffreDEmploiId, 1)
                                            .With(r => r.DateDisponibilite, DateTime.Now.AddDays(1))
                                            .With(r => r.PretentionSalariale, 100000)
                                            .Create();

            var documentsAvecCVetLettre = new List<DocumentModel>()

            {
                new DocumentModel() { Id = "idCV", Type = "CV", Chemin = "cheminCV" },
                new DocumentModel() { Id = "idLettreMotivation", Type = "LettreDeMotivation", Chemin = "cheminLettreMotivation" }
            };


            mockDocumentsService.Setup(ds => ds.ObtenirTout(requetePostulation.IdCandidat))
                                .ReturnsAsync(documentsAvecCVetLettre);

            var controller = new PostulationsController(
                null,
                null,
                mockLogger.Object,
                mockDocumentsService.Object,
                mockPostulationsService.Object,
                mockNoteService.Object);

            // Simuler postulation
            var postulation = new Postulation
            {
                Id = 1,
                IdCandidat = requetePostulation.IdCandidat,
                OffreDEmploiId = requetePostulation.OffreDEmploiId,
                DateDisponibilite = requetePostulation.DateDisponibilite,
                PretentionSalariale = requetePostulation.PretentionSalariale
            };
            mockPostulationsService.Setup(service => service.Ajouter(requetePostulation))
                                   .ReturnsAsync(postulation);


            // LORSQUE
            var RedirectToActionResult = await controller.Postuler(requetePostulation) as RedirectToActionResult;

            // ALORS
            RedirectToActionResult.Should().NotBeNull();
            RedirectToActionResult.ActionName.Should().Be("Index");
            mockPostulationsService.Verify(ps => ps.Ajouter(requetePostulation), Times.Once);
            controller.ModelState.ErrorCount.Should().Be(0);
            controller.ModelState.IsValid.Should().BeTrue();
        }
        #endregion

        #region Edit HTTPGet

        [Fact]
        public async Task Edit_IdNonExistant_Retourne_NotFound()
        {
            // Etant donne
            // Init postulation
            var fixture = new Fixture();
            var postulation = fixture.Create<RequetePostulation>();

            // Init Mock
            var mockLogger = new Mock<ILogger<PostulationsController>>();
            var mockPostulationsService = new Mock<IPostulationsService>();
            mockPostulationsService.Setup(e => e.ObtenirSelonId(It.IsAny<int>())).Returns(Task.FromResult<Postulation>(null));

            var postulationsController = new PostulationsController
            (
                null,
                null,
                mockLogger.Object,
                null,
                mockPostulationsService.Object,
                null
            );

            // Lorsque
            var actionResult = await postulationsController.Edit(3);

            // Alors
            actionResult.Should().BeOfType(typeof(NotFoundResult));

        }

        [Fact]
        public async Task Edit_DateDisponibiliteSuperieurDe4JourALaDateDAujourdhui_Retourne_BadRequest()
        {
            // Etant donne

            // Init postulation
            var fixture = new Fixture();
            var postulation = fixture.Build<Postulation>()
                                        .With(p => p.Id, 5)
                                        .With(p => p.IdCandidat, "IdTest")
                                        .With(p => p.OffreDEmploiId, 5)
                                        .With(p => p.PretentionSalariale, 20000)
                                        .With(p => p.DateDisponibilite, DateTime.Now.AddDays(4))
                                        .Create();

            // Init Mock
            var mockLogger = new Mock<ILogger<PostulationsController>>();
            var mockPostulationsService = new Mock<IPostulationsService>();
            mockPostulationsService.Setup(e => e.ObtenirSelonId(It.IsAny<int>())).Returns(Task.FromResult<Postulation>(postulation));

            var postulationsController = new PostulationsController
            (
                null,
                null,
                mockLogger.Object,
                null,
                mockPostulationsService.Object,
                null
            );

            // Lorsque
            var actionResult = await postulationsController.Edit(5);

            // Alors
            actionResult.Should().BeOfType(typeof(BadRequestObjectResult));

        }

        [Fact]
        public async Task Edit_DateDisponibiliteInferieurDe4JourALaDateDAujourdhui_Retourne_BadRequest()
        {
            // Etant donne

            // Init postulation
            var fixture = new Fixture();
            var postulation = fixture.Build<Postulation>()
                                        .With(p => p.Id, 5)
                                        .With(p => p.IdCandidat, "IdTest")
                                        .With(p => p.OffreDEmploiId, 5)
                                        .With(p => p.PretentionSalariale, 20000)
                                        .With(p => p.DateDisponibilite, DateTime.Now.AddDays(-4))
                                        .Create();

            // Init Mock
            var mockLogger = new Mock<ILogger<PostulationsController>>();
            var mockPostulationsService = new Mock<IPostulationsService>();
            mockPostulationsService.Setup(e => e.ObtenirSelonId(It.IsAny<int>())).Returns(Task.FromResult<Postulation>(postulation));

            var postulationsController = new PostulationsController
            (
                null,
                null,
                mockLogger.Object,
                null,
                mockPostulationsService.Object,
                null
            );

            // Lorsque
            var actionResult = await postulationsController.Edit(5);

            // Alors
            actionResult.Should().BeOfType(typeof(BadRequestObjectResult));

        }

        [Fact]
        public async Task Edit_IdExistant_Retourne_ViewResult()
        {
            //Etant donné

            // Init postulation
            var fixture = new Fixture();
            var postulation = fixture.Build<Postulation>()
                                        .With(p => p.Id, 5)
                                        .With(p => p.IdCandidat, "IdTest")
                                        .With(p => p.OffreDEmploiId, 5)
                                        .With(p => p.PretentionSalariale, 20000)
                                        .With(p => p.DateDisponibilite, DateTime.Now.AddDays(-10))
                                        .Create();

            // Init Mock
            var mockLogger = new Mock<ILogger<PostulationsController>>();
            var mockPostulationsService = new Mock<IPostulationsService>();
            mockPostulationsService.Setup(e => e.ObtenirSelonId(It.IsAny<int>())).Returns(Task.FromResult<Postulation>(postulation));

            var postulationsController = new PostulationsController
            (
                null,
                null,
                mockLogger.Object,
                null,
                mockPostulationsService.Object,
                null
            );

            //Lorsque
            var actionResult = await postulationsController.Edit(5) as ViewResult;

            //Alors
            actionResult.Should().NotBeNull();
            var postulationResult = actionResult.Model as Postulation;
            postulationResult.Should().Be(postulation);
        }
        #endregion

        #region Edit HTTPPost

        [Fact]
        public async Task Edit_QuandAucunCV_RetourneVueAvecErreur()
        {
            // ETANT DONNEE
            var fixture = new Fixture();
            var mockDocumentsService = new Mock<IDocumentsService>();
            var mockPostulationsService = new Mock<IPostulationsService>();
            var mockLogger = new Mock<ILogger<PostulationsController>>();
            var mockNoteService = new Mock<INoteService>();

            // Créer un objet RequetePostulation avec un ID candidat prédéfini
            var postulation = fixture.Build<Postulation>()
                                            .With(r => r.IdCandidat, "idCandidat")
                                            .With(r => r.OffreDEmploiId, 1)
                                            .With(r => r.DateDisponibilite, DateTime.Now.AddDays(1))
                                            .With(r => r.PretentionSalariale, 100000)
                                            .Create();

            var documentsSansCV = new List<DocumentModel>()
            {

                new DocumentModel()
                {
                    Id = "idLettreMotivation",
                    Type = "LettreDeMotivation",
                    Chemin = "cheminLettreMotivation"
                }
            };

            // Simuler l'absence de CV
            mockDocumentsService.Setup(ds => ds.ObtenirTout(postulation.IdCandidat))
                                .ReturnsAsync(documentsSansCV);

            var controller = new PostulationsController(
                null,
                null,
                mockLogger.Object,
                mockDocumentsService.Object,
                mockPostulationsService.Object,
                mockNoteService.Object
                );


            // LORSQUE
            var viewResult = await controller.Edit(postulation) as ViewResult;

            // ALORS
            viewResult.Should().NotBeNull();
            var postulationResult = viewResult.Model as Postulation;
            controller.ModelState.Count.Should().Be(1);
            controller.ModelState.Should().ContainKey("Error");
            controller.ModelState["Error"].Errors.Should().NotBeEmpty();
            controller.ModelState["Error"].Errors.FirstOrDefault()
                .ErrorMessage.Should().Be("Un CV est requis pour postuler. Veuillez télécharger d'abord un CV dans votre espace Documents.");

        }


        [Fact]
        public async Task Edit_QuandAucuneLettreMotivation_RetourneVueAvecErreur()
        {
            // ETANT DONNEE
            var fixture = new Fixture();
            var mockDocumentsService = new Mock<IDocumentsService>();
            var mockPostulationsService = new Mock<IPostulationsService>();
            var mockLogger = new Mock<ILogger<PostulationsController>>();
            var mockNoteService = new Mock<INoteService>();

            // Créer un objet RequetePostulation avec un ID candidat prédéfini
            var postulation = fixture.Build<Postulation>()
                                            .With(r => r.IdCandidat, "idCandidat")
                                            .With(r => r.OffreDEmploiId, 1)
                                            .With(r => r.DateDisponibilite, DateTime.Now.AddDays(1))
                                            .With(r => r.PretentionSalariale, 100000)
                                            .Create();

            var documentsSansLettreMotivation = new List<DocumentModel>()
            {

                new DocumentModel()
                {
                    Id = "idCV",
                    Type = "CV",
                    Chemin = "cheminCV"
                }
            };

            // Simuler l'absence de Lettre de motivation
            mockDocumentsService.Setup(ds => ds.ObtenirTout(postulation.IdCandidat))
                                .ReturnsAsync(documentsSansLettreMotivation);

            var controller = new PostulationsController(
                null,
                null,
                mockLogger.Object,
                mockDocumentsService.Object,
                mockPostulationsService.Object,
                mockNoteService.Object

                );


            // LORSQUE
            var viewResult = await controller.Edit(postulation) as ViewResult;

            // ALORS
            viewResult.Should().NotBeNull();
            var postulationResult = viewResult.Model as Postulation;
            controller.ModelState.Count.Should().Be(1);
            controller.ModelState.Should().ContainKey("Error");
            controller.ModelState["Error"].Errors.Should().NotBeEmpty();
            controller.ModelState["Error"].Errors.FirstOrDefault()
                .ErrorMessage.Should().Be("Une lettre de motivation est requise pour postuler. Veuillez télécharger une lettre de motivation dans votre espace Documents.");

        }

        [Fact]
        public async Task Edit_QuandDateDispoInfDateDuJour_RetourneVueAvecErreur()
        {
            // ETANT DONNEE
            var fixture = new Fixture();
            var mockDocumentsService = new Mock<IDocumentsService>();
            var mockPostulationsService = new Mock<IPostulationsService>();
            var mockLogger = new Mock<ILogger<PostulationsController>>();
            var mockNoteService = new Mock<INoteService>();

            // Créer un objet RequetePostulation
            var postulation = fixture.Build<Postulation>()
                                            .With(r => r.IdCandidat, "idCandidat")
                                            .With(r => r.OffreDEmploiId, 1)
                                            .With(r => r.DateDisponibilite, DateTime.Now.AddDays(-2))
                                            .With(r => r.PretentionSalariale, 100000)
                                            .Create();

            var documentsAvecCVetLettre = new List<DocumentModel>()

            {
                new DocumentModel() { Id = "idCV", Type = "CV", Chemin = "cheminCV" },
                new DocumentModel() { Id = "idLettreMotivation", Type = "LettreDeMotivation", Chemin = "cheminLettreMotivation" }
            };


            mockDocumentsService.Setup(ds => ds.ObtenirTout(postulation.IdCandidat))
                                .ReturnsAsync(documentsAvecCVetLettre);

            var controller = new PostulationsController(
                null,
                null,
                mockLogger.Object,
                mockDocumentsService.Object,
                mockPostulationsService.Object,
                mockNoteService.Object
                );


            // LORSQUE
            var viewResult = await controller.Edit(postulation) as ViewResult;

            // ALORS
            viewResult.Should().NotBeNull();
            var postulationResult = viewResult.Model as Postulation;
            controller.ModelState.Count.Should().Be(1);
            controller.ModelState.Should().ContainKey("DateDisponibilite");
            controller.ModelState["DateDisponibilite"].Errors.Should().NotBeEmpty();
            controller.ModelState["DateDisponibilite"].Errors.FirstOrDefault()
                .ErrorMessage.Should().Be("La date de disponibilité doit être supérieur à la date du jour");

        }

        [Fact]
        public async Task Edit_QuandDateDispoSupDateDuJourPlus45Jours_RetourneVueAvecErreur()
        {
            // ETANT DONNEE
            var fixture = new Fixture();
            var mockDocumentsService = new Mock<IDocumentsService>();
            var mockPostulationsService = new Mock<IPostulationsService>();
            var mockLogger = new Mock<ILogger<PostulationsController>>();
            var mockNoteService = new Mock<INoteService>();

            // Créer un objet RequetePostulation
            var postulation = fixture.Build<Postulation>()
                                            .With(r => r.IdCandidat, "idCandidat")
                                            .With(r => r.OffreDEmploiId, 1)
                                            .With(r => r.DateDisponibilite, DateTime.Now.AddDays(100))
                                            .With(r => r.PretentionSalariale, 100000)
                                            .Create();

            var documentsAvecCVetLettre = new List<DocumentModel>()

            {
                new DocumentModel() { Id = "idCV", Type = "CV", Chemin = "cheminCV" },
                new DocumentModel() { Id = "idLettreMotivation", Type = "LettreDeMotivation", Chemin = "cheminLettreMotivation" }
            };


            mockDocumentsService.Setup(ds => ds.ObtenirTout(postulation.IdCandidat))
                                .ReturnsAsync(documentsAvecCVetLettre);

            var controller = new PostulationsController(
                null,
                null,
                mockLogger.Object,
                mockDocumentsService.Object,
                mockPostulationsService.Object,
                mockNoteService.Object
                );


            // LORSQUE
            var viewResult = await controller.Edit(postulation) as ViewResult;

            // ALORS
            viewResult.Should().NotBeNull();
            var postulationResult = viewResult.Model as Postulation;
            controller.ModelState.Count.Should().Be(1);
            controller.ModelState.Should().ContainKey("DateDisponibilite");
            controller.ModelState["DateDisponibilite"].Errors.Should().NotBeEmpty();
            controller.ModelState["DateDisponibilite"].Errors.FirstOrDefault()
                .ErrorMessage.Should().Be("La date de disponibilité doit être inférieur ou égale à la date du jour + 45 jours");

        }

        [Fact]
        public async Task Edit_QuandPretentionSalarialeSup150000_RetourneVueAvecErreur()
        {
            // ETANT DONNEE
            var fixture = new Fixture();
            var mockDocumentsService = new Mock<IDocumentsService>();
            var mockPostulationsService = new Mock<IPostulationsService>();
            var mockLogger = new Mock<ILogger<PostulationsController>>();
            var mockNoteService = new Mock<INoteService>();

            // Créer un objet RequetePostulation
            var postulation = fixture.Build<Postulation>()
                                            .With(r => r.IdCandidat, "idCandidat")
                                            .With(r => r.OffreDEmploiId, 1)
                                            .With(r => r.DateDisponibilite, DateTime.Now.AddDays(20))
                                            .With(r => r.PretentionSalariale, 160000)
                                            .Create();

            var documentsAvecCVetLettre = new List<DocumentModel>()

            {
                new DocumentModel() { Id = "idCV", Type = "CV", Chemin = "cheminCV" },
                new DocumentModel() { Id = "idLettreMotivation", Type = "LettreDeMotivation", Chemin = "cheminLettreMotivation" }
            };


            mockDocumentsService.Setup(ds => ds.ObtenirTout(postulation.IdCandidat))
                                .ReturnsAsync(documentsAvecCVetLettre);

            var controller = new PostulationsController(
                null,
                null,
                mockLogger.Object,
                mockDocumentsService.Object,
                mockPostulationsService.Object,
                mockNoteService.Object
                );


            // LORSQUE
            var viewResult = await controller.Edit(postulation) as ViewResult;

            // ALORS
            viewResult.Should().NotBeNull();
            var postulationResult = viewResult.Model as Postulation;
            controller.ModelState.Count.Should().Be(1);
            controller.ModelState.Should().ContainKey("PretentionSalariale");
            controller.ModelState["PretentionSalariale"].Errors.Should().NotBeEmpty();
            controller.ModelState["PretentionSalariale"].Errors.FirstOrDefault()
                .ErrorMessage.Should().Be("Votre prétention salariale est au-delà de nos limites");

        }

        [Fact]
        public async Task Edit_QuandIdCandidatNullOrEmpty_RetourneVueAvecErreur()
        {
            // ETANT DONNEE
            var fixture = new Fixture();
            var mockDocumentsService = new Mock<IDocumentsService>();
            var mockPostulationsService = new Mock<IPostulationsService>();
            var mockLogger = new Mock<ILogger<PostulationsController>>();
            var mockNoteService = new Mock<INoteService>();

            // Créer un objet RequetePostulation
            var postulation = fixture.Build<Postulation>()
                                            .With(r => r.IdCandidat, "")
                                            .With(r => r.OffreDEmploiId, 1)
                                            .With(r => r.DateDisponibilite, DateTime.Now.AddDays(20))
                                            .With(r => r.PretentionSalariale, 120000)
                                            .Create();

            var documentsAvecCVetLettre = new List<DocumentModel>()

            {
                new DocumentModel() { Id = "idCV", Type = "CV", Chemin = "cheminCV" },
                new DocumentModel() { Id = "idLettreMotivation", Type = "LettreDeMotivation", Chemin = "cheminLettreMotivation" }
            };


            mockDocumentsService.Setup(ds => ds.ObtenirTout(postulation.IdCandidat))
                                .ReturnsAsync(documentsAvecCVetLettre);

            var controller = new PostulationsController(
                null,
                null,
                mockLogger.Object,
                mockDocumentsService.Object,
                mockPostulationsService.Object,
                mockNoteService.Object
                );


            // LORSQUE
            var viewResult = await controller.Edit(postulation) as ViewResult;

            // ALORS
            viewResult.Should().NotBeNull();
            var postulationResult = viewResult.Model as Postulation;
            controller.ModelState.Count.Should().Be(1);
            controller.ModelState.Should().ContainKey("all");
            controller.ModelState["all"].Errors.Should().NotBeEmpty();
            controller.ModelState["all"].Errors.FirstOrDefault()
                .ErrorMessage.Should().Be("Problème lors de l'ajout de la postulation, veuillez reessayer!");

        }

        [Fact]
        public async Task Edit_QuandModelStateValide_RetourneRedirectToAction()
        {
            // ETANT DONNEE
            var fixture = new Fixture();
            var mockDocumentsService = new Mock<IDocumentsService>();
            var mockPostulationsService = new Mock<IPostulationsService>();
            var mockLogger = new Mock<ILogger<PostulationsController>>();
            var mockNoteService = new Mock<INoteService>();

            // Créer un objet RequetePostulation
            var postulation = fixture.Build<Postulation>()
                                            .With(r => r.IdCandidat, "idCandidat")
                                            .With(r => r.OffreDEmploiId, 1)
                                            .With(r => r.DateDisponibilite, DateTime.Now.AddDays(1))
                                            .With(r => r.PretentionSalariale, 100000)
                                            .Create();

            var documentsAvecCVetLettre = new List<DocumentModel>()

            {
                new DocumentModel() { Id = "idCV", Type = "CV", Chemin = "cheminCV" },
                new DocumentModel() { Id = "idLettreMotivation", Type = "LettreDeMotivation", Chemin = "cheminLettreMotivation" }
            };


            mockDocumentsService.Setup(ds => ds.ObtenirTout(postulation.IdCandidat))
                                .ReturnsAsync(documentsAvecCVetLettre);

            var controller = new PostulationsController(
                null,
                null,
                mockLogger.Object,
                mockDocumentsService.Object,
                mockPostulationsService.Object,
                mockNoteService.Object);

            mockPostulationsService.Setup(service => service.Modifier(postulation));


            // LORSQUE
            var RedirectToActionResult = await controller.Edit(postulation) as RedirectToActionResult;

            // ALORS
            RedirectToActionResult.Should().NotBeNull();
            RedirectToActionResult.ActionName.Should().Be("ListePostulations");
            mockPostulationsService.Verify(ps => ps.Modifier(postulation), Times.Once);
            controller.ModelState.ErrorCount.Should().Be(0);
            controller.ModelState.IsValid.Should().BeTrue();
        }

        #endregion
    }
}
