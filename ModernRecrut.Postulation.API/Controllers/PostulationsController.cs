using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModernRecrut.Postulation.API.Data;
using ModernRecrut.Postulation.API.DTO;
using ModernRecrut.Postulation.API.Interfaces;
using ModernRecrut.Postulation.API.Models;

namespace ModernRecrut.Postulation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostulationsController : ControllerBase
    {
        private readonly IPostulationService _postulationService;

        public PostulationsController(IPostulationService postulationService)
        {
            _postulationService = postulationService;
        }

        // GET: api/Postulations
        [HttpGet]
        public async Task<IEnumerable<PostulationDetail>> Get()
        {
            return await _postulationService.ObtenirTout();
        }

        // GET: api/Postulations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostulationDetail>> Get(int id)
        {
            PostulationDetail? postulation = await _postulationService.ObtenirSelonId(id);

            if (postulation == null)
                return NotFound();

            return Ok(postulation);
        }

        // PUT: api/Postulations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id,[FromBody] PostulationDetail postulation)
        {
            if (id != postulation.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                bool modification = await _postulationService.Modifier(postulation);
                if (!modification)
                    return BadRequest();

                return NoContent();
            }
            return BadRequest();
        }

        // POST: api/Postulations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PostulationDetail>> Post([FromBody] RequetePostulation requetePostulation)
        {
            if (ModelState.IsValid)
            {
                PostulationDetail? postulation = await _postulationService.Ajouter(requetePostulation);

                if (postulation == null)
                    return BadRequest();

                return CreatedAtAction(nameof(Get), new { id = postulation?.Id }, postulation);
            }

            return BadRequest();
        }

        // DELETE: api/Postulations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            PostulationDetail? postulation = await _postulationService.ObtenirSelonId(id);

            if (postulation == null)
                return NotFound();

            await _postulationService.Supprimer(postulation);

            return NoContent();
        }
    }
}
