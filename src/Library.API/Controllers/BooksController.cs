using Library.API.Entities;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
	[Route("api/authors/{authorId}/books")]
	public class BooksController : Controller
	{
		private ILibraryRepository _libraryRepository;

		public BooksController(ILibraryRepository libraryRepository)
		{
			_libraryRepository = libraryRepository;
		}

		[HttpGet()]
		public IActionResult GetBooksForAuthor(Guid authorId)
		{
			if (!_libraryRepository.AuthorExists(authorId))
			{
				return NotFound();
			}

			var booksForAuthorsRepo = _libraryRepository.GetBooksForAuthor(authorId);

			var booksForAuthor = AutoMapper.Mapper.Map<IEnumerable<BooksDTO>>(booksForAuthorsRepo);

			return Ok(booksForAuthor);
		}

		[HttpGet("{id}", Name = "GetBookForAuthor")]
		public IActionResult GetBookForAuthor(Guid authorId, Guid Id)
		{
			if (!_libraryRepository.AuthorExists(authorId))
			{
				return NotFound();
			}

			var bookForAuthorRepo = _libraryRepository.GetBookForAuthor(authorId, Id);
			if (bookForAuthorRepo == null)
			{
				return NotFound();
			}

			var bookForAuthor = AutoMapper.Mapper.Map<BooksDTO>(bookForAuthorRepo);
			return Ok(bookForAuthor);
		}

		[HttpPost]
		public IActionResult CreateBook(Guid authorId, [FromBody] BookForCreationDto book)
		{
			if (book == null)
			{
				return BadRequest();
			}

			if (!_libraryRepository.AuthorExists(authorId))
			{
				return NotFound();
			}

			var bookObj = AutoMapper.Mapper.Map<Book>(book);

			_libraryRepository.AddBookForAuthor(authorId, bookObj);

			if (!_libraryRepository.Save())
			{
				throw new Exception($"Failed to add Book for author { authorId }");
			}

			var bookDto = AutoMapper.Mapper.Map<BooksDTO>(bookObj);

			return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, Id = bookDto.Id }, bookDto);
		}
	}
}
