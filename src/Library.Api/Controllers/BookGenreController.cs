using AutoMapper;
using Library.Api.ViewModels;
using Library.Business.Interfaces;
using Library.Business.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Api.Controllers
{
    [Authorize(Roles = "Administrator")]
    [ApiController]
    [Route("api/[controller]")]
    public class BookGenreController : Controller
    {
        private readonly IBookGenreRepository _bookGenreRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public BookGenreController(IBookGenreRepository bookGenreRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _bookGenreRepository = bookGenreRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<BookGenreViewModel>> GetBookGenres()
        {
            return _mapper.Map<IEnumerable<BookGenreViewModel>>(await _bookGenreRepository.GetAll());
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<BookGenreViewModel>> GetBookGenre(int id)
        {
            var bookGenre = _mapper.Map<BookGenreViewModel>(await _bookGenreRepository.GetById(id));

            if (bookGenre == null)
            {
                return NotFound();
            }

            return bookGenre;
        }

        [HttpPost]
        public async Task<ActionResult<BookGenre>> PostBookGenre(BookGenreViewModel bookGenreViewModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var bookGenre = _mapper.Map<BookGenre>(bookGenreViewModel);

            await _bookGenreRepository.Add(bookGenre);

            return CreatedAtAction("GetBookGenre", new { id = bookGenre.Id }, bookGenre);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BookGenre>> PutBookGenre(int id, BookGenreViewModel bookGenreViewModel)
        {
            if (id != bookGenreViewModel.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var bookGenre = _mapper.Map<BookGenre>(bookGenreViewModel);

                if (bookGenre == null)
                {
                    return NotFound(new { message = "BookGenre not found " });
                }

                await _bookGenreRepository.Update(bookGenre);
            }
            catch (DbUpdateConcurrencyException)
            {
                var bookGenre = await _bookGenreRepository.GetById(id);

                if (bookGenre == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<BookGenre>> DeleteBookGenre(int id)
        {
            var bookGenre = await _bookGenreRepository.FirstOrDefault(x => x.Id == id);

            if (bookGenre == null)
            {
                return NotFound();
            }

            await _bookGenreRepository.Remove(id);

            return bookGenre;
        }
    }
}
