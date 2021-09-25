using AutoMapper;
using Library.Api.ViewModels;
using Library.Business.Interfaces;
using Library.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookGenreRepository _bookGenreRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IReserveRepository _reserveRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        private readonly UserManager<IdentityUser> _userManager;

        public BooksController(IAuthorRepository authorRepository,
            IBookGenreRepository bookGenreRepository,
            IBookRepository bookRepository,
            IReserveRepository reserveRepository,
            IUserRepository userRepository,
            IMapper mapper, UserManager<IdentityUser> userManager)
        {
            _authorRepository = authorRepository;
            _bookGenreRepository = bookGenreRepository;
            _bookRepository = bookRepository;
            _reserveRepository = reserveRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpGet("")]
        //[HttpGet("{authorId?}/{bookGenreId?}")]
        //public async Task<IEnumerable<BookViewModel>> GetBooks(int? authorId = null, int? bookGenreId = null)
        public async Task<IEnumerable<BookViewModel>> GetBooks()
        {
            //if (authorId != null && bookGenreId != null)
            //{
            //    return _mapper.Map<IEnumerable<BookViewModel>>(await _bookRepository.GetWhere(b => b.AuthorId == authorId && b.BookGenreId == bookGenreId));
            //}

            //if (authorId != null)
            //{
            //    return _mapper.Map<IEnumerable<BookViewModel>>(await _bookRepository.GetWhere(b => b.AuthorId == authorId));
            //}

            //if (bookGenreId != null)
            //{
            //    return _mapper.Map<IEnumerable<BookViewModel>>(await _bookRepository.GetWhere(b => b.BookGenreId == bookGenreId));
            //}

            return _mapper.Map<IEnumerable<BookViewModel>>(await _bookRepository.GetAll());
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<BookViewModel>> GetBook(int id)
        {
            var book = _mapper.Map<BookViewModel>(await _bookRepository.GetById(id));

            if (book == null)
            {
                return NotFound();
            }
            
            var author = await _authorRepository.GetById(book.AuthorId);
            book.Author = _mapper.Map<AuthorViewModel>(author);

            var bookGenre = await _bookGenreRepository.GetById(book.BookGenreId);
            book.BookGenre = _mapper.Map<BookGenreViewModel>(bookGenre);

            return book;
        }
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(BookViewModel bookViewModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var book = _mapper.Map<Book>(bookViewModel);
            var author = await _authorRepository.GetById(bookViewModel.AuthorId);
            
            if (author == null)
            {
                return NotFound(new { message = "Author not found "});
            }

            var bookGenre = await _bookGenreRepository.GetById(bookViewModel.BookGenreId);

            if (bookGenre == null)
            {
                return NotFound(new { message = "BookGenre not found " });
            }

            await _bookRepository.Add(book);

            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }
        [Authorize(Roles = "Administrator")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Book>> PutBook(int id, BookViewModel bookViewModel)
        {

            if (id != bookViewModel.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var book = _mapper.Map<Book>(bookViewModel);
                var author = await _authorRepository.GetById(bookViewModel.AuthorId);

                if (author == null)
                {
                    return NotFound(new { message = "Author not found " });
                }

                var bookGenre = await _bookGenreRepository.GetById(bookViewModel.BookGenreId);

                if (bookGenre == null)
                {
                    return NotFound(new { message = "BookGenre not found " });
                }

                await _bookRepository.Update(book);
            }
            catch (DbUpdateConcurrencyException)
            {
                var book = await _bookRepository.GetById(id);
                if (book == null)
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
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Book>> DeleteBook(int id)
        {
            var book = await _bookRepository.FirstOrDefault(x => x.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            await _bookRepository.Remove(id);

            return book;
        }

        [Authorize(Roles = "Administrator, CommonUser")]
        [HttpPost("Reserve")]
        public async Task<ActionResult> PostReserveBook(ReserveViewModel reserveViewModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var reserve = _mapper.Map<Reserve>(reserveViewModel);

            await _reserveRepository.Add(reserve);

            return Ok(new { message = "success"});
        }

        [Authorize(Roles = "Administrator, CommonUser")]
        [HttpPut("Return/{id}")]
        public async Task<ActionResult<ReserveViewModel>> PutReturnBook(int id, ReserveViewModel reserveViewModel)
        {
            if (id != reserveViewModel.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var reserve = _mapper.Map<Reserve>(reserveViewModel);

                await _reserveRepository.Update(reserve);
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


    }
}
