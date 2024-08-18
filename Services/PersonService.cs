using ServiceContracts;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using System.ComponentModel.DataAnnotations;
using Services.Helpers;
using ServiceContracts.Enums;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class PersonService : IPersonsService
    {
        private readonly ICountriesService _countriesService;
        private readonly PersonsDbContext _db;

        public PersonService(PersonsDbContext personsDbContext, ICountriesService countriesService)
        {
            _db = personsDbContext;
            _countriesService = countriesService;
        }

        public PersonResponse AddPerson(PersonAddRequest personAddRequest)
        {
            if (personAddRequest == null) throw new ArgumentNullException(nameof(personAddRequest));

            ValidationHelper.ModelValidation(personAddRequest);

            Person person = personAddRequest.ToPerson();

            person.PersonID =Guid.NewGuid();

            _db.Persons.Add(person);
            _db.SaveChanges();



            return person.ToPersonResponse();

        }

        public List<PersonResponse> GetAllPersons()
        {
            var persons  = _db.Persons.Include("Country").ToList();
            return persons
                .Select(person=>person.ToPersonResponse()).ToList();
        }


        public PersonResponse GetPersonByID(Guid? personID)
        {
            if (personID ==null)return null;

            Person? person = _db.Persons.Include("Country").FirstOrDefault(temp=>temp.PersonID == personID);
            if (person == null) return null;

            return person.ToPersonResponse();
        }
        public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;

            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString)) return matchingPersons;

            switch (searchBy)
            {
                case nameof(Person.PersonName):
                    matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.PersonName) ? temp.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;
                case nameof(Person.Email):
                    matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.Email) ? temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;
                case nameof(Person.DateOfBirth):
                    matchingPersons = allPersons.Where(temp => (temp.DateOfBirth != null ? temp.DateOfBirth.Value.ToString("dd-MM-yyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;
                case nameof(Person.Gender):
                    matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.Gender) ? temp.Gender.Equals(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;
                case nameof(Person.CountryID):
                    matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.Country) ? temp.Country.StartsWith(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;
                case nameof(Person.Address):
                    matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.Address) ? temp.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;
                default:
                    matchingPersons = allPersons;
                    break;
            }
            return matchingPersons;
        }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortedBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortedBy)) return allPersons;

            List<PersonResponse> sortedPersons = (sortedBy, sortOrder)
                switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) =>
                allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) =>
                 allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC) =>
allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.DESC) =>
                 allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) =>
allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) =>
                 allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),



                (nameof(PersonResponse.Age), SortOrderOptions.ASC) =>
allPersons.OrderBy(temp => temp.Age).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.DESC) =>
                 allPersons.OrderByDescending(temp => temp.Age).ToList(),



                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) =>
allPersons.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) =>
                 allPersons.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),


                (nameof(PersonResponse.Country), SortOrderOptions.ASC) =>
allPersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.DESC) =>
                 allPersons.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),


                (nameof(PersonResponse.Address), SortOrderOptions.ASC) =>
allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.DESC) =>
                 allPersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),


                (nameof(PersonResponse.RecieveNewsLetters), SortOrderOptions.ASC) =>
allPersons.OrderBy(temp => temp.RecieveNewsLetters).ToList(),

                (nameof(PersonResponse.RecieveNewsLetters), SortOrderOptions.DESC) =>
                 allPersons.OrderByDescending(temp => temp.RecieveNewsLetters).ToList(),

                _ => allPersons
            };
            return sortedPersons;
        }

        public bool DeletePerson(Guid? personID)
        {
            if (personID == null)
                throw new ArgumentNullException(nameof(personID));

            Person? person = _db.Persons.FirstOrDefault(temp => temp.PersonID == personID);

            if (person == null) return false;
            _db.Persons.Remove(_db.Persons.First(temp => temp.PersonID == personID));
            _db.SaveChanges();
            return true;
        }

        public PersonResponse UpdatePerson(PersonUpdateRequest personUpdateRequest)
        {
            //Null Object check
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(PersonUpdateRequest));

            //Validation
            ValidationHelper.ModelValidation(personUpdateRequest);

            //get Matching Person
            Person? MatchingPerson = _db.Persons.FirstOrDefault(temp => temp.PersonID == personUpdateRequest.PersonID);
            if (MatchingPerson == null)
            {
                throw new ArgumentException("Given PersonID does not exist");
            }
            //update Details
            MatchingPerson.PersonName = personUpdateRequest.PersonName;
            MatchingPerson.Email = personUpdateRequest.Email;
            MatchingPerson.Address = personUpdateRequest.Address;
            MatchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            MatchingPerson.Gender = personUpdateRequest.Gender.ToString();
            MatchingPerson.CountryID = personUpdateRequest.CountryID;
            MatchingPerson.RecieveNewsLetters = personUpdateRequest.RecieveNewsLetters;
            _db.SaveChanges();

            return MatchingPerson.ToPersonResponse();
        }
    }
}
