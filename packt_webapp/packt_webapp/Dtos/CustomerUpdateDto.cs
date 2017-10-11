using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace packt_webapp.Dtos
{
	public class CustomerUpdateDto
	{
        [Required(ErrorMessage = "Please provide the first name")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Please provide the last name")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Please provide age 1-100")]
        [Range(0, 100)]
        public int Age { get; set; }
	}
}
