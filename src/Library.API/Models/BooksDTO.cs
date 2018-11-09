using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Models
{
	public class BooksDTO
	{
		public Guid Id { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public Guid	AuthorId { get; set; }
	}

	public class BookForCreationDto
	{
		[Required]
		[MaxLength(100)]
		public string Title { get; set; }

		[MaxLength(500)]
		public string Description { get; set; }
	}

	public class BookForUpdateDto
	{
		public string Title { get; set; }

		public string Description { get; set; }
	}
}
