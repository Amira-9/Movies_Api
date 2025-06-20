﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies_Api.Models;

namespace Movies_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private new List<string> _allowedExtentions = new List<string> { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;


        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {

            var movies = await _context.Movies
                .OrderByDescending(x => x.Rate)
                .Include(m => m.Genre)
                .Select(m => new MovieDetailsDto
                {
                    Id = m.Id,
                    GenreId = m.GenreId,
                    GenreName = m.Genre.Name,
                    Poster = m.Poster, 
                    Rate =m.Rate, 
                    StoryLine =m.StoryLine,
                    Title = m.Title,
                    Year = m.Year
                })
                .ToListAsync();
            return Ok(movies);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie = await _context.Movies.Include(m => m.Genre).SingleOrDefaultAsync(m => m.Id == id);
            if(movie == null)
            {
                return NotFound();
            }
            var dto = new MovieDetailsDto
            {
                Id = movie.Id,
                GenreId = movie.GenreId,
                GenreName = movie.Genre.Name,
                Poster = movie.Poster,
                Rate = movie.Rate,
                StoryLine = movie.StoryLine,
                Title = movie.Title,
                Year = movie.Year
            };
            return Ok(dto);
        }
        [HttpGet("GetByGenreId")]
        public async Task<IActionResult> GetByGenreIdAsync(byte genreId)
        {
            var movies = await _context.Movies
                .Where(m => m.GenreId == genreId)
                .OrderByDescending(x => x.Rate)
                .Include(m => m.Genre)
                .Select(m => new MovieDetailsDto
                {
                    Id = m.Id,
                    GenreId = m.GenreId,
                    GenreName = m.Genre.Name,
                    Poster = m.Poster,
                    Rate = m.Rate,
                    StoryLine = m.StoryLine,
                    Title = m.Title,
                    Year = m.Year
                })
                .ToListAsync();
            return Ok(movies);
        }
        [HttpPost]

        public async Task<IActionResult> CreateAsync([FromForm]MovieDto dto)
        {
            if (dto.Poster == null)
                return BadRequest("Poster is Required");
            if (!_allowedExtentions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest("Only .png or .jpg are allowed");
            
            if( dto.Poster.Length > _maxAllowedPosterSize)
            {
                return BadRequest("Max allowed size for poster is 1MB");
            }
            var isValidGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);
            if (! isValidGenre)
            {
                return BadRequest("invalid Genre Id");
            }
            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);

            var movie = new Movie
            {
                GenreId = dto.GenreId,
                Title = dto.Title,
                Year=dto.Year,
                Poster=dataStream.ToArray(),
                Rate=dto.Rate,
                StoryLine=dto.StoryLine
            };
            await _context.AddAsync(movie);
            _context.SaveChanges();
            return Ok(movie);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id , [FromForm] MovieDto dto)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound($"no movie is found with Id: {id}");
            }
            var isValidGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);
            if (!isValidGenre)
            {
                return BadRequest("invalid Genre Id");
            }
            if(dto.Poster != null)
            {
                if (!_allowedExtentions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    return BadRequest("Only .png or .jpg are allowed");

                if (dto.Poster.Length > _maxAllowedPosterSize)
                {
                    return BadRequest("Max allowed size for poster is 1MB");
                }
                using var dataStream = new MemoryStream();
                await dto.Poster.CopyToAsync(dataStream);
                movie.Poster = dataStream.ToArray();
            }
            movie.Title = dto.Title;
            movie.Year = dto.Year;
            movie.StoryLine = dto.StoryLine;
            movie.Rate = dto.Rate;
            movie.GenreId = dto.GenreId;

            _context.SaveChanges();
            return Ok(movie);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound($"no movie is found with Id: {id}");
            _context.Remove(movie);
            _context.SaveChanges();
            return Ok(movie);
        }
    }
}
