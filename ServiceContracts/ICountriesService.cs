using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Country entity
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Adds a country object to the list of countries
        /// </summary>
        /// <param name="countryAddRequest"></param>
        /// <returns>The country object after adding it (including newly generated country id)</returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Returns all countries from the list
        /// </summary>
        /// <returns>All countries from the list </returns>
        Task<List<CountryResponse>> GetAllCountries();

        /// <summary>
        /// Return a country object based on the given country id
        /// </summary>
        /// <param name="countryID"></param>
        /// <returns>Matching country as CountryResponse object </returns>
        Task<CountryResponse?> GetCountryByCountryID(Guid? countryID);
    }
}
