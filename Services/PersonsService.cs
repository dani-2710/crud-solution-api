using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        private readonly PersonsDbContext _db;
        private readonly ICountriesService _countriesService;

        public PersonsService(PersonsDbContext personsDbContext, ICountriesService countriesService)
        {
            _db = personsDbContext;
            _countriesService = countriesService;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(personAddRequest));
            }

            ValidationHelper.ModelValidation(personAddRequest);

            Person person = personAddRequest.ToPerson();
            person.PersonID = Guid.NewGuid();

            await _db.Persons.AddAsync(person);
            await _db.SaveChangesAsync();
            // _db.sp_InsertPerson(person);
            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            return await AllPersons();
            // return _db.sp_GetAllPersons().Select(person => ConvertPersonToPersonResponse(person!)).ToList();
        }

        private async Task<List<PersonResponse>> AllPersons()
        {
            var persons = await _db.Persons.Include("Country").ToListAsync();
            return persons.Select(person => person.ToPersonResponse()).ToList();
        }

        public async Task<PersonResponse?> GetPersonById(Guid? personID)
        {
            if (personID == null) return null;
            Person? person = await _db.Persons.Include("Country").FirstOrDefaultAsync(person => person.PersonID == personID);

            if (person == null) return null;

            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = await AllPersons();

            List<PersonResponse> filteredPersons = allPersons;

            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            {
                return filteredPersons;
            }

            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    filteredPersons = allPersons.Where(person =>
                    !string.IsNullOrEmpty(person.PersonName) ?
                    person.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(PersonResponse.Email):
                    filteredPersons = allPersons.Where(person =>
                    !string.IsNullOrEmpty(person.Email) ?
                    person.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(PersonResponse.DateOfBirth):
                    filteredPersons = allPersons.Where(person =>
                    (person.DateOfBirth != null) ?
                    person.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(PersonResponse.Gender):
                    filteredPersons = allPersons.Where(person =>
                    !string.IsNullOrEmpty(person.Gender) ?
                    person.Gender.Equals(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(PersonResponse.Address):
                    filteredPersons = allPersons.Where(person =>
                    !string.IsNullOrEmpty(person.Address) ?
                    person.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                default: filteredPersons = allPersons; break;
            }
            return filteredPersons;
        }

        public List<PersonResponse> GetSortedPersons
            (List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                return allPersons;
            }

            List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC)
                 => allPersons.OrderBy(person => person.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC)
                 => allPersons.OrderByDescending(person => person.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC)
                 => allPersons.OrderBy(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Email), SortOrderOptions.DESC)
                 => allPersons.OrderByDescending(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC)
                 => allPersons.OrderBy(person => person.DateOfBirth).ToList(),
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC)
                 => allPersons.OrderByDescending(person => person.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC)
                 => allPersons.OrderBy(person => person.Age).ToList(),
                (nameof(PersonResponse.Age), SortOrderOptions.DESC)
                 => allPersons.OrderByDescending(person => person.Age).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.ASC)
                 => allPersons.OrderBy(person => person.Gender).ToList(),
                (nameof(PersonResponse.Gender), SortOrderOptions.DESC)
                 => allPersons.OrderByDescending(person => person.Gender).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.ASC)
                 => allPersons.OrderBy(person => person.Country, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Country), SortOrderOptions.DESC)
                 => allPersons.OrderByDescending(person => person.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC)
                 => allPersons.OrderBy(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Address), SortOrderOptions.DESC)
                 => allPersons.OrderByDescending(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC)
                 => allPersons.OrderBy(person => person.ReceiveNewsLetters).ToList(),
                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC)
                 => allPersons.OrderByDescending(person => person.ReceiveNewsLetters).ToList(),

                _ => allPersons
            };

            return sortedPersons;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(personUpdateRequest));
            }

            ValidationHelper.ModelValidation(personUpdateRequest);

            Person? foundPerson = await _db.Persons.FirstOrDefaultAsync
                (person => person.PersonID == personUpdateRequest.PersonID);

            if (foundPerson == null)
            {
                throw new ArgumentException("Given person id doesn't exist");
            }

            foundPerson.PersonName = personUpdateRequest.PersonName;
            foundPerson.Email = personUpdateRequest.Email;
            foundPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            foundPerson.Gender = personUpdateRequest.Gender.ToString();
            foundPerson.Address = personUpdateRequest.Address;
            foundPerson.CountryID = personUpdateRequest.CountryID;
            foundPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            await _db.SaveChangesAsync();

            return foundPerson.ToPersonResponse();

        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            if (personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            };

            Person? person = await _db.Persons.FirstOrDefaultAsync(person => person.PersonID == personID);

            if (person == null) return false;

            _db.Persons.Remove(person);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);

            CsvConfiguration csvConfiguration = new CsvConfiguration(
                CultureInfo.InvariantCulture
            );

            CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration);

            csvWriter.WriteField(nameof(PersonResponse.PersonName));
            csvWriter.WriteField(nameof(PersonResponse.Email));
            csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.Gender));
            csvWriter.WriteField(nameof(PersonResponse.Country));
            csvWriter.WriteField(nameof(PersonResponse.Address));
            csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));

            csvWriter.NextRecord();

            List<PersonResponse> persons = await AllPersons();

            foreach (PersonResponse person in persons)
            {
                csvWriter.WriteField(person.PersonName);
                csvWriter.WriteField(person.Email);
                if (person.DateOfBirth != null)
                {
                    csvWriter.WriteField(person.DateOfBirth.Value.ToString("yyyy-MM-dd"));
                }
                else
                {
                    csvWriter.WriteField("");
                }
                csvWriter.WriteField(person.Age);
                csvWriter.WriteField(person.Gender);
                csvWriter.WriteField(person.Country);
                csvWriter.WriteField(person.Address);
                csvWriter.WriteField(person.ReceiveNewsLetters);
                csvWriter.NextRecord();
                csvWriter.Flush();
            }
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
