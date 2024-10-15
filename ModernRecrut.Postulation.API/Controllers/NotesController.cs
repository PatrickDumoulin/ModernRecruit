using Microsoft.AspNetCore.Mvc;
using ModernRecrut.Postulation.API.DTO;
using ModernRecrut.Postulation.API.Interfaces;
using ModernRecrut.Postulation.API.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ModernRecrut.Postulation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NotesController(INoteService noteService)
        {
            _noteService = noteService;
        }

        // GET: api/Notes
        [HttpGet]
        public async Task<IEnumerable<NoteDetail>> Get()
        {
            return await _noteService.ObtenirTout();
        }

        // GET: api/Notes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NoteDetail>> Get(int id)
        {
            NoteDetail? postulation = await _noteService.ObtenirSelonId(id);

            if (postulation == null)
                return NotFound();

            return Ok(postulation);
        }

        // PUT: api/Notes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] NoteDetail note)
        {
            if (id != note.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                bool modification = await _noteService.Modifier(note);
                if (!modification)
                    return BadRequest();

                return NoContent();
            }
            return BadRequest();
        }

        // POST: api/Notes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NoteDetail>> Post([FromBody] RequeteNote requeteNote)
        {
            if (ModelState.IsValid)
            {
                NoteDetail? note = await _noteService.Ajouter(requeteNote);

                if (note == null)
                    return BadRequest();

                return CreatedAtAction(nameof(Get), new { id = note?.Id }, note);
            }

            return BadRequest();
        }

        // DELETE: api/Notes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            NoteDetail? note = await _noteService.ObtenirSelonId(id);

            if (note == null)
                return NotFound();

            await _noteService.Supprimer(note);

            return NoContent();
        }
    }
}
