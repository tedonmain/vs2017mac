﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using packt_webapp.Dtos;
using packt_webapp.Entities;
using packt_webapp.QueryParameters;
using packt_webapp.Repositories;

namespace packt_webapp.Controllers
{
    //[ApiVersion("1.0", Deprecated = true)] 
    //[Route("api/v{version:apiVersion}/customers")]
    [Route("api/[controller]")]
    public class CustomersController : Controller
    {
        private readonly ILogger<CustomersController> _logger;

        private ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository, ILogger<CustomersController> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
            _logger.LogInformation(" ------> CustomersControler started...");

        }

        [HttpGet]
        [ProducesResponseType(typeof(List<Customer>), 200)] //[ProducesResponseType(typeof(void), 200)]
        //public IActionResult GetAllCustomers() // CustomerQueryParameters customerQueryParameters
        public IActionResult GetAllCustomers(CustomerQueryParameters customerQueryParameters) // CustomerQueryParameters customerQueryParameters
		{
            //throw new Exception("  ----> Test Exception");

            _logger.LogInformation(" -------> GetAllCustomers()...");

            //var allCustomers = _customerRepository.GetAll().ToList();
            var allCustomers = _customerRepository.GetAll(customerQueryParameters);


            var allCustomersDto = allCustomers.Select(x => Mapper.Map<CustomerDto>(x));

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(new { totalCount = _customerRepository.Count()}));

            return Ok(allCustomersDto);
        }

        [HttpGet]
        [Route("{id}", Name = "GetSingleCustomer")]
        public IActionResult GetSingleCustomer(Guid id)
        {
            Customer CustomerFromRepo = _customerRepository.GetSingle(id);
            if (CustomerFromRepo == null)
            {
                return NotFound();
            }
            return Ok(Mapper.Map<CustomerDto>(CustomerFromRepo));
        }

        // POST api/customers - adding a new customer
        [HttpPost]
        [ProducesResponseType(typeof(void), 201)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult AddCustomer([FromBody]CustomerCreateDto customerCreateDto)
        {
            if (customerCreateDto == null) return BadRequest("customer create object was null");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            Customer toAdd = Mapper.Map<Customer>(customerCreateDto);

            _customerRepository.Add(toAdd);

            bool result = _customerRepository.Save();

            if (!result) throw new Exception("(POST):something went wrong when adding a new customer");
            //return Ok(Mapper.Map<CustomerDto>(toAdd));

            return CreatedAtRoute("GetSingleCustomer", new { id = toAdd.Id }, Mapper.Map<CustomerDto>(toAdd));
        }

        // PUT api/customers/id 
        [HttpPut]
        [Route("{id}")]
        public IActionResult UpdateCustomer(Guid id, [FromBody] CustomerUpdateDto updateDto)
        {
            if (updateDto == null) return BadRequest();

            var existingCustomer = _customerRepository.GetSingle(id);

            if (existingCustomer == null) return NotFound();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            Mapper.Map(updateDto, existingCustomer);

            _customerRepository.Update(existingCustomer);

            bool result = _customerRepository.Save();

            if (!result) throw new Exception($"(PUT):something went wrong when updating the customer with id: {id}");

			return Ok(Mapper.Map<CustomerDto>(existingCustomer));
        }

        [HttpPatch]
        [Route("{id}")]
        public IActionResult PartiallyUpdate(Guid id, [FromBody] JsonPatchDocument<CustomerUpdateDto> customerPatchDoc)
        {
            if (customerPatchDoc == null) return BadRequest();

            var existingCustomer = _customerRepository.GetSingle(id);
            if (existingCustomer == null) return NotFound();

            var customerToPatch = Mapper.Map<CustomerUpdateDto>(existingCustomer);
            customerPatchDoc.ApplyTo(customerToPatch, ModelState);

            TryValidateModel(customerToPatch);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            Mapper.Map(customerToPatch, existingCustomer);

            _customerRepository.Update(existingCustomer);

            bool result = _customerRepository.Save();

            if (!result) throw new Exception($"(PATCH):something went wrong when updating the customer with id: {id}");

            return Ok(Mapper.Map<CustomerDto>(existingCustomer));
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Remove(Guid id) {
            var existingCustomer = _customerRepository.GetSingle(id);
            if (existingCustomer == null) return NotFound();
            _customerRepository.Delete(id);
            bool result = _customerRepository.Save();
            if (!result) throw new Exception($"(DELETE):something went wrong when deleting the customer with id: {id}");
            return NoContent();
        }
    }
}
