using Entities;
using ServiceContracts.Enums;
using System.Reflection.Metadata.Ecma335;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents DTO class that is used as return type of most methods of Persons Services
    /// </summary>
    public class PersonResponse
    {
        public Guid PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public Guid? CountryID { get; set; }
        public string? Country { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        public int? Age { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj.GetType() != typeof(PersonResponse)) return false;

            var personResponseObj = (PersonResponse)obj;

            return PersonID == personResponseObj.PersonID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonID = PersonID,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender!, false),
                Address = Address,
                CountryID = CountryID,
                ReceiveNewsLetters = ReceiveNewsLetters

            };
        }

    }

    public static class PersonExtensions
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                Address = person.Address,
                CountryID = person.CountryID,
                ReceiveNewsLetters = person.ReceiveNewsLetters,
                Age = person.DateOfBirth != null ?
                Convert.ToInt32(Math.Round((DateTime.Now - person.DateOfBirth).Value.TotalDays / 365.25)) : null,
                Country = person.Country?.CountryName
            };
        }
    }
}
