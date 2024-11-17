using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _countriesService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));

            _personService = new PersonsService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options), _countriesService);
            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson

        [Fact]
        public async Task AddPerson_NullPerson()
        {
            // Arrange
            PersonAddRequest? personAddRequest = null;

            // Act
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _personService.AddPerson(personAddRequest));
        }

        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
            // Arrange
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Test",
                Email = "test@test.com",
                Address = "Street123",
                CountryID = Guid.NewGuid(),
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters = true
            };

            // Act
            PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
            List<PersonResponse> persons = await _personService.GetAllPersons();
            // Assert
            Assert.True(personResponse.PersonID != Guid.Empty);
            Assert.Contains(personResponse, persons);
        }

        #endregion

        #region GetAllPersons

        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            // Arrange
            List<PersonResponse> personResponses = await _personService.GetAllPersons();

            // Assert
            Assert.Empty(personResponses);
        }

        [Fact]

        public async Task GetAllPersons_AddFewPersons()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "USA"
            };

            CountryAddRequest countryAddRequest2 = new CountryAddRequest()
            {
                CountryName = "ETH"
            };

            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Test",
                Email = "test@test.com",
                Address = "Street123",
                CountryID = countryResponse.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters = true
            };

            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "Test2",
                Email = "test2@test.com",
                Address = "Street321",
                CountryID = countryResponse2.CountryID,
                Gender = GenderOptions.Female,
                DateOfBirth = DateTime.Parse("2001-01-01"),
                ReceiveNewsLetters = false
            };

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
            {
                personAddRequest,
                personAddRequest2
            };


            // Act
            List<PersonResponse> responses = new List<PersonResponse>();

            _testOutputHelper.WriteLine("Expected:");
            foreach (var response in responses)
            {
                _testOutputHelper.WriteLine(response.ToString());
            }

            foreach (var addRequest in personAddRequests)
            {
                responses.Add(await _personService.AddPerson(addRequest));
            }

            List<PersonResponse> personResponses = await _personService.GetAllPersons();

            _testOutputHelper.WriteLine("Actual:");
            foreach (var person in personResponses)
            {
                _testOutputHelper.WriteLine(person.ToString());
            }

            foreach (var response in responses)
            {
                // Assert
                Assert.Contains(response, personResponses);
            }

        }

        #endregion

        #region GetPersonByID

        [Fact]
        public async Task GetPersonByID_NullPersonID()
        {
            // Arrange
            Guid? personID = null;

            // Act
            PersonResponse? personResponse = await _personService.GetPersonById(personID);

            // Assert
            Assert.Null(personResponse);

        }

        [Fact]
        public async Task GetPersonByID_WithPersonID()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "ETHIOPIA"
            };
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            // Act
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Test",
                Email = "test@test.com",
                Address = "Streett123",
                CountryID = countryResponse.CountryID,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };
            PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
            PersonResponse? foundPerson = await _personService.GetPersonById(personResponse.PersonID);

            // Assert
            Assert.Equal(personResponse, foundPerson);
        }

        #endregion

        #region GetFilteredPersons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "USA"
            };

            CountryAddRequest countryAddRequest2 = new CountryAddRequest()
            {
                CountryName = "ETH"
            };

            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Test",
                Email = "test@test.com",
                Address = "Street123",
                CountryID = countryResponse.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters = true
            };

            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "person",
                Email = "test2@test.com",
                Address = "Street321",
                CountryID = countryResponse2.CountryID,
                Gender = GenderOptions.Female,
                DateOfBirth = DateTime.Parse("2001-01-01"),
                ReceiveNewsLetters = false
            };

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
            {
                personAddRequest,
                personAddRequest2
            };


            // Act
            List<PersonResponse> responses = new List<PersonResponse>();

            foreach (var addRequest in personAddRequests)
            {
                responses.Add(await _personService.AddPerson(addRequest));
            }

            List<PersonResponse> personResponses = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");

            foreach (var response in responses)
            {
                // Assert
                Assert.Contains(response, personResponses);
            }

        }

        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "USA"
            };

            CountryAddRequest countryAddRequest2 = new CountryAddRequest()
            {
                CountryName = "ETH"
            };

            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Test",
                Email = "test@test.com",
                Address = "Street123",
                CountryID = countryResponse.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters = true
            };

            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "bEst",
                Email = "test2@test.com",
                Address = "Street321",
                CountryID = countryResponse2.CountryID,
                Gender = GenderOptions.Female,
                DateOfBirth = DateTime.Parse("2001-01-01"),
                ReceiveNewsLetters = false
            };

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
            {
                personAddRequest,
                personAddRequest2
            };


            // Act
            List<PersonResponse> responses = new List<PersonResponse>();

            foreach (var addRequest in personAddRequests)
            {
                responses.Add(await _personService.AddPerson(addRequest));
            }

            List<PersonResponse> personResponses = await _personService.GetFilteredPersons(nameof(Person.PersonName), "est");

            foreach (PersonResponse response in responses)
            {
                if (response.PersonName != null)
                {
                    // Assert
                    if (response.PersonName.Contains("est", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(response, personResponses);
                    }
                }
            }

        }

        #endregion

        #region GetSortedPersons

        [Fact]
        public async Task GetSortedPersons()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "USA"
            };

            CountryAddRequest countryAddRequest2 = new CountryAddRequest()
            {
                CountryName = "ETH"
            };

            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Test",
                Email = "test@test.com",
                Address = "Street123",
                CountryID = countryResponse.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters = true
            };

            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "bEst",
                Email = "test2@test.com",
                Address = "Street321",
                CountryID = countryResponse2.CountryID,
                Gender = GenderOptions.Female,
                DateOfBirth = DateTime.Parse("2001-01-01"),
                ReceiveNewsLetters = false
            };

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
            {
                personAddRequest,
                personAddRequest2
            };


            // Act
            List<PersonResponse> responses = new List<PersonResponse>();

            foreach (var addRequest in personAddRequests)
            {
                responses.Add(await _personService.AddPerson(addRequest));
            }
            List<PersonResponse> allPersons = await _personService.GetAllPersons();
            List<PersonResponse> personResponses = _personService.GetSortedPersons
                (allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            responses.OrderByDescending(res => res.PersonName).ToList();

            // Assert
            for (int i = 0; i < responses.Count; i++)
            {
                Assert.Equal(responses[i], personResponses[i]);
            }
        }

        #endregion

        #region UpdatePerson

        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            // Arrange
            PersonUpdateRequest? personUpdateRequest = null;


            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                // Act
                await _personService.UpdatePerson(personUpdateRequest);
            });
        }

        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
        {
            // Arrange
            PersonUpdateRequest? personUpdateRequest = new PersonUpdateRequest()
            {
                PersonID = Guid.NewGuid(),
                PersonName = "test",
                Email = "test@test.com",
                Address = "akjshdkas"
            };


            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // Act
                await _personService.UpdatePerson(personUpdateRequest);
            });
        }

        [Fact]
        public async Task UpdatePerson_PersonFullDetailUpdate()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "UK"
            };
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Test",
                CountryID = countryResponse.CountryID,
                Address = "Street123",
                DateOfBirth = DateTime.Parse("2000-01-01"),
                Email = "test@test.com",
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true

            };

            PersonResponse personResponse = await _personService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = "Updated Test";
            personUpdateRequest.Email = "updatedtest@gmail.com";

            // Act
            PersonResponse updatedPersonResponse = await _personService.UpdatePerson(personUpdateRequest);


            PersonResponse? foundPerson = await _personService.GetPersonById(updatedPersonResponse.PersonID);

            //Assert
            Assert.Equal(foundPerson?.PersonName, updatedPersonResponse.PersonName);
        }

        #endregion

        #region DeletePerson

        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "ETH"
            };

            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Test",
                CountryID = countryResponse.CountryID,
                Address = "street123",
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                Email = "test@test.com"
            };
            PersonResponse personResponse = await _personService.AddPerson(personAddRequest);

            // Act
            bool isDeleted = await _personService.DeletePerson(personResponse.PersonID);

            // Assert
            Assert.True(isDeleted);
        }

        [Fact]
        public async Task DeletePerson_InValidPersonID()
        {
            // Act
            bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());

            // Assert
            Assert.False(isDeleted);
        }
        #endregion
    }
}
