﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace packt_webapp.Dtos
{
	public class CustomerCreateDto
	{
		[Required(ErrorMessage = "Please give the first name")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Pleasve give the last name")]
		public string Lastname { get; set; }

        [Required]
        [Range(1,100)]
		public int Age { get; set; }
	}
}
