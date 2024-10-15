using AutoFixture;
using FluentAssertions;
using ModernRecrut.Postulation.API.DTO;
using ModernRecrut.Postulation.API.Interfaces;
using ModernRecrut.Postulation.API.Models;
using ModernRecrut.Postulation.API.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernRecrut.Postulation.API.UnitTests
{
    public class GenererEvaluationServiceTests
    {
        [Fact]
        public async void GenererNote_PretentionSalarialeInf20000_RetourneNoteSalaireInferieur()
        {

            // ETANT DONNEE
            var mockNoteService = new Mock<INoteService>();

            mockNoteService.Setup(r => r.Ajouter(It.IsAny<RequeteNote>()));

            GenererEvaluationService service = new GenererEvaluationService(mockNoteService.Object);

            var fixture = new Fixture();
            var postulation = fixture.Build<PostulationDetail>()
                                            .With(r => r.PretentionSalariale, 10000) 
                                            .Create();

            var expectedNote = new NoteDetail { Note = "Salaire inférieur à la norme" };
            mockNoteService.Setup(r => r.Ajouter(It.IsAny<RequeteNote>()))
                           .ReturnsAsync(expectedNote);

            // LORSQUE
            var resultat = await service.GenererNote(postulation);

            // ALORS
            resultat.Should().NotBeNull();
            resultat.Note.Should().Be("Salaire inférieur à la norme");
            mockNoteService.Verify(r => r.Ajouter(It.Is<RequeteNote>(n => 
            n.Note == "Salaire inférieur à la norme")), Times.Once());
        }

        [Fact]
        public async void GenererNote_PretentionSalariales_SupOuEgal20000_Ou_InfOuEgal3999_RetourneNoteSalaireProcheMaisInferieur()
        {

            // ETANT DONNEE
            var mockNoteService = new Mock<INoteService>();

            mockNoteService.Setup(r => r.Ajouter(It.IsAny<RequeteNote>()));

            GenererEvaluationService service = new GenererEvaluationService(mockNoteService.Object);

            var fixture = new Fixture();
            var postulation = fixture.Build<PostulationDetail>()
                                            .With(r => r.PretentionSalariale, 22000)
                                            .Create();

            var expectedNote = new NoteDetail { Note = "Salaire proche mais inférieur à la norme" };
            mockNoteService.Setup(r => r.Ajouter(It.IsAny<RequeteNote>()))
                           .ReturnsAsync(expectedNote);

            // LORSQUE
            var resultat = await service.GenererNote(postulation);

            // ALORS
            resultat.Should().NotBeNull();
            resultat.Note.Should().Be("Salaire proche mais inférieur à la norme");
            mockNoteService.Verify(r => r.Ajouter(It.Is<RequeteNote>(n =>
            n.Note == "Salaire proche mais inférieur à la norme")), Times.Once());
        }

        [Fact]
        public async void GenererNote_PretentionSalariales_SupOuEgal40000_Et_InfOuEgal79999_RetourneNoteSalaireDansNorme()
        {

            // ETANT DONNEE
            var mockNoteService = new Mock<INoteService>();

            mockNoteService.Setup(r => r.Ajouter(It.IsAny<RequeteNote>()));

            GenererEvaluationService service = new GenererEvaluationService(mockNoteService.Object);

            var fixture = new Fixture();
            var postulation = fixture.Build<PostulationDetail>()
                                            .With(r => r.PretentionSalariale, 42000)
                                            .Create();

            var expectedNote = new NoteDetail { Note = "Salaire dans la norme" };
            mockNoteService.Setup(r => r.Ajouter(It.IsAny<RequeteNote>()))
                           .ReturnsAsync(expectedNote);

            // LORSQUE
            var resultat = await service.GenererNote(postulation);

            // ALORS
            resultat.Should().NotBeNull();
            resultat.Note.Should().Be("Salaire dans la norme");
            mockNoteService.Verify(r => r.Ajouter(It.Is<RequeteNote>(n =>
            n.Note == "Salaire dans la norme")), Times.Once());
        }

        [Fact]
        public async void GenererNote_PretentionSalariales_SupOuEgal80000_Et_InfOuEgal99999_RetourneNoteSalaireProcheMaisSuperieur()
        {

            // ETANT DONNEE
            var mockNoteService = new Mock<INoteService>();

            mockNoteService.Setup(r => r.Ajouter(It.IsAny<RequeteNote>()));

            GenererEvaluationService service = new GenererEvaluationService(mockNoteService.Object);

            var fixture = new Fixture();
            var postulation = fixture.Build<PostulationDetail>()
                                            .With(r => r.PretentionSalariale, 82000)
                                            .Create();

            var expectedNote = new NoteDetail { Note = "Salaire proche mais supérieur à la norme" };
            mockNoteService.Setup(r => r.Ajouter(It.IsAny<RequeteNote>()))
                           .ReturnsAsync(expectedNote);

            // LORSQUE
            var resultat = await service.GenererNote(postulation);

            // ALORS
            resultat.Should().NotBeNull();
            resultat.Note.Should().Be("Salaire proche mais supérieur à la norme");
            mockNoteService.Verify(r => r.Ajouter(It.Is<RequeteNote>(n =>
            n.Note == "Salaire proche mais supérieur à la norme")), Times.Once());
        }

        [Fact]
        public async void GenererNote_PretentionSalariales_SupOuEgal100000_RetourneNoteSalaireSuperieurANorme()
        {

            // ETANT DONNEE
            var mockNoteService = new Mock<INoteService>();

            mockNoteService.Setup(r => r.Ajouter(It.IsAny<RequeteNote>()));

            GenererEvaluationService service = new GenererEvaluationService(mockNoteService.Object);

            var fixture = new Fixture();
            var postulation = fixture.Build<PostulationDetail>()
                                            .With(r => r.PretentionSalariale, 110000)
                                            .Create();

            var expectedNote = new NoteDetail { Note = "Salaire supérieur à la norme" };
            mockNoteService.Setup(r => r.Ajouter(It.IsAny<RequeteNote>()))
                           .ReturnsAsync(expectedNote);

            // LORSQUE
            var resultat = await service.GenererNote(postulation);

            // ALORS
            resultat.Should().NotBeNull();
            resultat.Note.Should().Be("Salaire supérieur à la norme");
            mockNoteService.Verify(r => r.Ajouter(It.Is<RequeteNote>(n =>
            n.Note == "Salaire supérieur à la norme")), Times.Once());
        }
    }
}
