using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Models
{
	public class AuthorDTO
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public int Age { get; set; }

		public string Genre { get; set; }
	}

	public class AuthorForCreationDTO
	{
		public string FirstName { get; set; }

		public string LastName { get; set; }

		public DateTimeOffset DateOfBirth { get; set; }

		public string Genre { get; set; }

		public ICollection<BookForCreationDto> Books { get; set; }
		= new List<BookForCreationDto>();

	}
}
