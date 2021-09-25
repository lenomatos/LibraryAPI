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
    public class AuthorsController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public AuthorsController(IAuthorRepository authorRepository, IUserRepository userRepository, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<AuthorViewModel>> GetAuthors()
        {
            return _mapper.Map<IEnumerable<AuthorViewModel>>(await _authorRepository.GetAll());
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorViewModel>> GetAuthor(int id)
        {
            var author = _mapper.Map<AuthorViewModel>(await _authorRepository.GetById(id));

            if (author == null)
            {
                return NotFound();
            }

            return author;
        }

        [HttpPost]
        public async Task<ActionResult<Author>> PostAuthor(AuthorViewModel AuthorViewModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var author = _mapper.Map<Author>(AuthorViewModel);

            await _authorRepository.Add(author);

            return CreatedAtAction("GetAuthor", new { id = author.Id }, author);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Author>> PutAuthor(int id, AuthorViewModel AuthorViewModel)
        {

            if (id != AuthorViewModel.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var author = _mapper.Map<Author>(AuthorViewModel);

                await _authorRepository.Update(author);
            }
            catch (DbUpdateConcurrencyException)
            {
                var author = await _authorRepository.GetById(id);

                if (author == null)
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
        public async Task<ActionResult<Author>> DeleteAuthor(int id)
        {
            var author = await _authorRepository.FirstOrDefault(x => x.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            await _authorRepository.Remove(id);

            return author;
        }
    }
}
