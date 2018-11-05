using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.API.Helpers;

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
			throw new Exception("Random Exception thrown");

			var authorsFromRepo = _libraryRepository.GetAuthors();

			var authors = AutoMapper.Mapper.Map<IEnumerable<AuthorDTO>>(authorsFromRepo);
			return Ok(authors);
		}

		[HttpGet("{id}")]
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
	}
}
