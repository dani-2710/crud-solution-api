using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    public interface IPersonsService
    {
        Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);

        Task<List<PersonResponse>> GetAllPersons();

        Task<PersonResponse?> GetPersonById(Guid? personID);

        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

        List<PersonResponse> GetSortedPersons
            (List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);

        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);

        Task<bool> DeletePerson(Guid? personID);

        Task<MemoryStream> GetPersonsCSV();
    }
}
