using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.API.Helpers;
using Library.API.Entities;

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

		[HttpPatch]
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
	}
}
