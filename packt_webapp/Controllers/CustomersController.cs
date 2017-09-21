using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using packt_webapp.Dtos;
using packt_webapp.Entities;
using packt_webapp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace packt_webapp.Controllers
{
    [Route("api/[controller]")]
    public class CustomersController : Controller
    {
        private ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
		public IActionResult GetAllCustomers()
		{
			var allCustomers = _customerRepository.GetAll().ToList();

			var allCustomersDto = allCustomers.Select(x => Mapper.Map<CustomerDto>(x));

			return Ok(allCustomersDto);
		}
    }
}
