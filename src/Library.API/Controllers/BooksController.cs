using Library.API.Entities;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.JsonPatch;
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

		[HttpDelete("{id}")]
		public IActionResult DeleteBookForAuthor(Guid authorId, Guid id)
		{
			if (!_libraryRepository.AuthorExists(authorId))
			{
				return NotFound();
			}

			var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);
			if (bookForAuthorFromRepo == null)
			{
				return NotFound();
			}

			_libraryRepository.DeleteBook(bookForAuthorFromRepo);

			if (!_libraryRepository.Save())
			{
				throw new Exception($"Deleting book {id} for author {authorId} failed on save.");
			}

			return NoContent();
		}

		[HttpPut("{Id}")]
		public IActionResult UpdateBookForAuthor(Guid authorId, Guid Id,
			[FromBody] BookForUpdateDto bookForUpdate)
		{
			if(bookForUpdate == null)
			{
				return NotFound();
			}

			if (!_libraryRepository.AuthorExists(authorId))
			{
				return NotFound();
			}

			var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, Id);
			if (bookForAuthorFromRepo == null)
			{
				var bookToAdd = AutoMapper.Mapper.Map<Book>(bookForUpdate);
				bookToAdd.Id = Id;

				_libraryRepository.AddBookForAuthor(authorId, bookToAdd);

				if (!_libraryRepository.Save())
				{
					throw new Exception($"Upserting the book {Id} for author {authorId} failed");
				}

				var bookToReturn = AutoMapper.Mapper.Map<BooksDTO>(bookToAdd);

				return CreatedAtRoute("GetBookForAuthor", 
					new { authorId = bookToReturn.AuthorId, id = bookToReturn.Id }, bookToReturn);
			}

			var book = AutoMapper.Mapper.Map(bookForAuthorFromRepo, bookForAuthorFromRepo);

			_libraryRepository.UpdateBookForAuthor(bookForAuthorFromRepo);

			if (!_libraryRepository.Save())
			{
				throw new Exception($"failed to update book {Id} for author {authorId} ");
			}

			return NoContent();
		}

		[HttpPatch("{id}")]
		public IActionResult PartiallyUpdateBookForAuthor(Guid authorId, Guid id, 
				[FromBody] JsonPatchDocument<BookForUpdateDto> patchDocument)
		{
			if(patchDocument == null)
			{
				return BadRequest();
			}

			if (!_libraryRepository.AuthorExists(authorId))
			{
				return NotFound();
			}

			var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);
			if (bookForAuthorFromRepo == null)
			{
				var bookForUpdate = new BookForUpdateDto();
				patchDocument.ApplyTo(bookForUpdate);

				var bookToAdd = AutoMapper.Mapper.Map<Book>(bookForUpdate);
				bookToAdd.Id = id;

				_libraryRepository.AddBookForAuthor(authorId, bookToAdd);

				if (!_libraryRepository.Save())
				{
					throw new Exception($"Upserting with patch for book {id} for author {authorId} failed");
				}

				var bookToReturn = AutoMapper.Mapper.Map<BooksDTO>(bookToAdd);

				return CreatedAtRoute("GetBookForAuthor",
					new { authorId = bookToReturn.AuthorId, id = bookToReturn.Id }, bookToReturn);
			}

			var bookToPatch = AutoMapper.Mapper.Map<BookForUpdateDto>(bookForAuthorFromRepo);

			patchDocument.ApplyTo(bookToPatch);

			AutoMapper.Mapper.Map(bookToPatch, bookForAuthorFromRepo);

			_libraryRepository.UpdateBookForAuthor(bookForAuthorFromRepo);

			if (!_libraryRepository.Save())
			{
				throw new Exception($"Patching Book {id} for author {authorId} failed");
			}

			return NoContent();
		}
	}
}
