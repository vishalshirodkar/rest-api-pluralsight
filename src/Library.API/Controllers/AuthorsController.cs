using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.API.Helpers;
using Library.API.Entities;
using Microsoft.AspNetCore.Http;

namespace Library.API.Controllers
{
	[Route("api/authors")]
	public class AuthorsController : Controller
	{
		private ILibraryRepository _libraryRepository;

		public AuthorsController(ILibraryRepository libraryRepository)
		{
			_libraryRepository = libraryRepository;
		}

		[HttpGet()]
		public IActionResult GetAuthors()
		{
			var authorsFromRepo = _libraryRepository.GetAuthors();

			var authors = AutoMapper.Mapper.Map<IEnumerable<AuthorDTO>>(authorsFromRepo);
			return Ok(authors);
		}

		[HttpGet("{id}", Name = "GetAuthor")]
		public IActionResult GetAuthor(Guid id)
		{
			var authorFromRepo = _libraryRepository.GetAuthor(id);

			if (authorFromRepo == null)
			{
				return NotFound();
			}

			var authors = AutoMapper.Mapper.Map<AuthorDTO>(authorFromRepo);
			return Ok(authors);
		}

		[HttpPost]
		public IActionResult CreateAuthor([FromBody] AuthorForCreationDTO author)
		{
			if(author == null)
			{
				return BadRequest();
			}

			var authorObj = AutoMapper.Mapper.Map<Author>(author);

			_libraryRepository.AddAuthor(authorObj);

			if (!_libraryRepository.Save())
			{
				throw new Exception("Unable to save author details");
			}

			var authorDTO = AutoMapper.Mapper.Map<AuthorDTO>(authorObj);

			return CreatedAtRoute("GetAuthor", new { id = authorDTO.Id }, authorDTO);

		}


		[HttpPost("{id}")]
		public IActionResult BlockAuthorCreation(Guid id)
		{
			if (_libraryRepository.AuthorExists(id))
			{
				return new StatusCodeResult(StatusCodes.Status409Conflict);
			}

			return NotFound();
		}

		[HttpDelete("{id}")]
		public IActionResult DeleteAuthor(Guid id)
		{
			var authorFromRepo = _libraryRepository.GetAuthor(id);
			if (authorFromRepo == null)
			{
				return NotFound();
			}

			_libraryRepository.DeleteAuthor(authorFromRepo);

			if (!_libraryRepository.Save())
			{
				throw new Exception($"Deleting author {id} failed on save.");
			}

			return NoContent();
		}

	}
}
