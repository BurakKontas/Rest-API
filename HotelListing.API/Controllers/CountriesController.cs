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

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize] //authorize all
    public class CountriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICountriesRepository _countriesRepository;

        public CountriesController(IMapper mapper, ICountriesRepository countriesRepository)
        {
            this._mapper = mapper;
            this._countriesRepository = countriesRepository;
        }

        // GET: api/Countries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {
          if (_countriesRepository == null)
          {
              return NotFound();
          }
            // Select * from Countries (sql)
            var countries = await _countriesRepository.GetAllAsync();
            // We cant make map from Lists
            var records = _mapper.Map<List<GetCountryDto>>(countries);
            return Ok(records); //200 success
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

            if (country == null)
            {
                return NotFound();
            }

            var record = _mapper.Map<CountryDto>(country);

            return Ok(record); // Ok() is not necessary you can just return country;
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
            
            var country = await _countriesRepository.GetAsync(id);

            if(country == null)
            {
                return NotFound();
            }

            _mapper.Map(updateCountryDto, country);

            try
            {
                await _countriesRepository.UpdateAsync(country);
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
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountryDto)
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

            var country = _mapper.Map<Country>(createCountryDto);

            await _countriesRepository.AddAsync(country);

            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
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
            var country = await _countriesRepository.GetAsync(id);
            if (country == null)
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
