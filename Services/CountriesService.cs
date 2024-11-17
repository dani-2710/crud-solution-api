using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly PersonsDbContext _db;

        public CountriesService(PersonsDbContext personsDbContext)
        {
            _db = personsDbContext;
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            if (await _db.Countries.CountAsync(country => country.CountryName == countryAddRequest.CountryName) > 0)
            {
                throw new ArgumentException("Given country name is already exists");
            }


            Country country = countryAddRequest.ToCountry();
            country.CountryID = Guid.NewGuid();
            await _db.Countries.AddAsync(country);
            await _db.SaveChangesAsync();
            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return await _db.Countries.Select(country => country.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null) return null;

            Country? country = await _db.Countries.FirstOrDefaultAsync(country => country.CountryID == countryID);

            if (country == null) return null;

            return country.ToCountryResponse();
        }
    }
}
