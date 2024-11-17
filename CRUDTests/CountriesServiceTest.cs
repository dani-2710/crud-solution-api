using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Xunit.Sdk;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        public CountriesServiceTest()
        {
            _countriesService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
        }

        # region AddCountry
        // When CountryAddRequest is null, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            // Arrange
            CountryAddRequest? request = null;

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                // Act
                await _countriesService.AddCountry(request);
            });
        }

        // When the CountryName is null, it should throw ArgumentException

        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            // Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = null
            };

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // Act
                await _countriesService.AddCountry(request);
            });
        }

        // When the CountryName is duplicate, it should throw ArgumentException

        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            // Arrange
            CountryAddRequest? request1 = new CountryAddRequest()
            {
                CountryName = "USA"
            }; CountryAddRequest? request2 = new CountryAddRequest()
            {
                CountryName = "USA"
            };

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // Act
                await _countriesService.AddCountry(request1);
                await _countriesService.AddCountry(request2);
            });
        }

        // When you supply proper country name, it should insert (add)
        // the country to the existing list of countries

        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            // Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = "USA"
            };

            // Act
            CountryResponse response = await _countriesService.AddCountry(request);
            List<CountryResponse> countries = await _countriesService.GetAllCountries();

            // Assert
            Assert.True(response.CountryID != Guid.Empty);
            Assert.Contains(response, countries);
        }

        #endregion

        #region GetAllCountries
        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            // Acts
            List<CountryResponse> countries = await _countriesService.GetAllCountries();

            // Assert
            Assert.Empty(countries);
        }
        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            // Arrange
            List<CountryAddRequest> countries = new List<CountryAddRequest>()
            {
                new CountryAddRequest() {CountryName = "USA"},
                new CountryAddRequest() {CountryName = "ETHIOPIA"},
                new CountryAddRequest() {CountryName = "ISRAEL"},
            };

            // Act
            List<CountryResponse> response = new List<CountryResponse>();
            foreach (var country in countries)
            {
                response.Add(await _countriesService.AddCountry(country));
            }

            List<CountryResponse> response_countries = await _countriesService.GetAllCountries();

            foreach (var country in response)
            {
                // Assert
                Assert.Contains(country, response_countries);
            }
        }
        #endregion

        #region GetContryByID

        [Fact]

        public async Task GetCountryByID_NullCountryID()
        {
            // Arrange
            Guid? countryID = null;

            // Act
            CountryResponse? countryResponse = await _countriesService.GetCountryByCountryID(countryID);

            // Assert
            Assert.Null(countryResponse);
        }

        [Fact]
        public async Task GetCountryByID_ValidCountryID()
        {
            // Arrange
            CountryAddRequest? countryAddRequest = new CountryAddRequest()
            {
                CountryName = "Ethiopia"
            };

            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            // Act
            CountryResponse? addedCountry = await _countriesService.GetCountryByCountryID(countryResponse.CountryID);

            // Assert
            Assert.NotNull(addedCountry);
            Assert.Equal(countryResponse, addedCountry);
        }

        #endregion
    }
}
