using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Movies_Api.Services;

namespace Movies_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenresServices _genresServices;

        public GenresController(IGenresServices genresServices)
        {
            
            _genresServices = genresServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var genres = await _genresServices.GetAll();
            return Ok(genres);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(GenreDto dto)
        {
            var genre = new Genre
            {
                Name = dto.Name
            };
            await _genresServices.Add(genre);
            return Ok(genre);
        }
        [HttpPut("{id}")]
        
        public async Task<IActionResult> UpdateAsync(byte id , [FromBody] GenreDto dto )
        {
            var genre = await _genresServices.GetById(id);
            if (genre == null)
            {
                return NotFound($"no genre is found with Id :{id}");
            }
            genre.Name = dto.Name;
            _genresServices.Update(genre);
            return Ok(genre);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(byte id)
        {
            var genre = await _genresServices.GetById(id);
            if (genre == null)
            {
                return NotFound($"no genre is found with Id :{id}");
            }
            _genresServices.Delete(genre);
           
            return Ok(genre);
        }
    }
}
