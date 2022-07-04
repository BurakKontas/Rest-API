using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using HotelListing.API.Models.Country;
using AutoMapper;
using HotelListing.API.Contracts;
using Microsoft.AspNetCore.Authorization;
using HotelListing.API.Exceptions;
using HotelListing.API.Models;
using Microsoft.AspNetCore.OData.Query;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    //[ApiVersion("1.0")] version
    //api/countries?api-version=1 //we spesified api-version string in Program.cs
    //or X-Version key with version value in header
    //or spesify route [Route("api/v{version:apiVersion}/[controller]")] then use like api/v1/countries
    //also you can [ApiVersion("1.0", Deprecated = true)] to deprecate version
    
    //[Authorize] //authorize all
    public class CountriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICountriesRepository _countriesRepository;
        private readonly ILogger _logger;

        public CountriesController(IMapper mapper, ICountriesRepository countriesRepository, ILogger<CountriesController> logger)
        {
            this._mapper = mapper;
            this._countriesRepository = countriesRepository;
            this._logger = logger;
        }

        // GET: api/Countries/GetAll
        [HttpGet("GetAll")]
        [EnableQuery]
        //api/Countries/getall?$select=name&$filter
        //$filter=name eq 'Turkey'
        //gt greater than
        //lt less than
        //$orderby=name 
        //you can use all of them with other
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {
          if (_countriesRepository == null)
          {
              return NotFound();
          }
            var countries = await _countriesRepository.GetAllAsync<GetCountryDto>();
            return Ok(countries); //200 success
        }

        // GET: api/Countries/?StartIndex=0&pagesize=25&PageNumber=1
        [HttpGet]
        public async Task<ActionResult<PagedResult<GetCountryDto>>> GetPagedCountries([FromQuery] QueryParameters queryParameter)
        {
            if (_countriesRepository == null)
            {
                return NotFound();
            }
            var pagedCountriesResult = await _countriesRepository.GetAllAsync<GetCountryDto>(queryParameter);
            return Ok(pagedCountriesResult); //200 success
        }

        // GET: api/Countries/5
        //[HttpGet("{id}/hotelId/{hotelId}")]
        // Get: api/Countries/5/hotelId/2 
        // This GET will return spesific countries spesific hotel but if you do that you should modify codes
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountry(int id)
        {
            if (_countriesRepository == null)
            {
                return NotFound();
            }

            var country = await _countriesRepository.GetDetails(id);
            return Ok(country);
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountryDto)
        {
            if (id != updateCountryDto.Id)
            {
                return BadRequest(); //return BadRequest("Invalid Record ID"); //you can pass messages to requests
            }

            try
            {
                await _countriesRepository.UpdateAsync(id,updateCountryDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CountryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CountryDto>> PostCountry(CreateCountryDto createCountryDto)
        {
          if (_countriesRepository == null)
          {
              return Problem("Entity set 'HotelListingDbContext.Countries'  is null.");
          }
            //var country = new Country
            //{
            //    Name = createCountryDto.Name,
            //    ShortName = createCountryDto.ShortName,
            //};

            var country = await _countriesRepository.AddAsync<CreateCountryDto, GetCountryDto>(createCountryDto);
            return CreatedAtAction(nameof(GetCountry), new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")] // [Authorize(Roles = "Administrator","User")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            if (_countriesRepository == null)
            {
                return NotFound();
            }

            await _countriesRepository.DeleteAsync(id);
            return NoContent();
        }

        private async Task<bool> CountryExists(int id)
        {
            return await _countriesRepository.Exists(id);
        }
    }
}
