using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CrudTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _countriesService = new CountryService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
            _personsService = new PersonService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options),_countriesService);
            _testOutputHelper = testOutputHelper;
            
        }

        #region AddPerson

        [Fact]
        public void AddPerson_NullPerson()
        {
            PersonAddRequest? personAddRequest = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                _personsService.AddPerson(personAddRequest);
            });
        }

        [Fact]
        public void AddPerson_NullPersonName()
        {
            PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName=null};

            Assert.Throws<ArgumentException>(() =>
            {
                _personsService.AddPerson(personAddRequest);
            });
        }


        [Fact]
        public void AddPerson_ProperDetails()
        {
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = "Ahmed",
                CountryID = Guid.NewGuid(),
                DateOfBirth = DateTime.Parse("2003-01-01"),
                Address = "123 street",
                Gender = GenderOptions.Male,
                RecieveNewsLetters= true,
                Email="Ahmed@email.com"
            };

            PersonResponse personResponse=_personsService.AddPerson(personAddRequest);

            List<PersonResponse> PersonsList = _personsService.GetAllPersons();




            Assert.True(personResponse.PersonID != Guid.Empty);
            Assert.Contains(personResponse, PersonsList);
           
        }

        #endregion

        #region GetPersonByID

        [Fact]
        public void GetPersonByID_NullPersonID()
        {
            Guid? personID = null;

            PersonResponse PersonFromGet =_personsService.GetPersonByID(personID);

            Assert.Null(PersonFromGet);
        }

        [Fact]
        public void GetPersonByID_WithPersonID()
        {
            CountryAddRequest? countryAddRequest = new CountryAddRequest() { CountryName ="Egypt"};

            CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest() 
            {PersonName="Omar",
                Email="omar@email.com",
                DateOfBirth=DateTime.Parse("2002-03-01") ,
                Address="123 street",
                CountryID=countryResponse.CountryID,
                Gender=GenderOptions.Male,RecieveNewsLetters = true,
            };

            PersonResponse personFromAddRequest = _personsService.AddPerson(personAddRequest);

            PersonResponse personFromGetRequest = _personsService.GetPersonByID(personFromAddRequest.PersonID);

            Assert.Equal(personFromGetRequest, personFromAddRequest);
        }

        #endregion

        #region GetAllPersons

        [Fact]
        public void GetAllPersons_EmptyList()
        {
            List<PersonResponse> personResponses = _personsService.GetAllPersons();

            Assert.Empty(personResponses);
        }


        [Fact]
        public void GetAllPersons_AddFewPersons()
        {
            //arrange
            CountryAddRequest country1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country2 = new CountryAddRequest() { CountryName = "Egypt" };

            CountryResponse countryResponse1 = _countriesService.AddCountry(country1);
            CountryResponse countryResponse2 = _countriesService.AddCountry(country2);

            PersonAddRequest person1 = new PersonAddRequest() { PersonName = "Person3 name", Email = "person3@email.com", Address = "Person678", DateOfBirth = DateTime.Parse("2002-11-01"), Gender = GenderOptions.Female, CountryID = countryResponse1.CountryID, RecieveNewsLetters = false };
            PersonAddRequest person2 = new PersonAddRequest() { PersonName = "Person4 name", Email = "person4@email.com", Address = "Person655", DateOfBirth = DateTime.Parse("2008-11-01"), Gender = GenderOptions.Other, CountryID = countryResponse2.CountryID, RecieveNewsLetters = false };

            PersonResponse personResponse1 = _personsService.AddPerson(person1);
            PersonResponse personResponse2 = _personsService.AddPerson(person2);

            List<PersonResponse> personResponsesFromAdd = new List<PersonResponse>() { personResponse1, personResponse2 };

            //test print
            _testOutputHelper.WriteLine("Expected: ");
            personResponsesFromAdd.ForEach(person => _testOutputHelper.WriteLine(person.ToString()));

            List<PersonResponse> personResponsesfromGet = _personsService.GetAllPersons();

            _testOutputHelper.WriteLine("Actual: ");
            personResponsesfromGet.ForEach(person => _testOutputHelper.WriteLine(person.ToString()));
            foreach (var personResponseFromAdd in personResponsesFromAdd)
            {
                Assert.Contains(personResponseFromAdd, personResponsesfromGet);
            }


            //act


        }

        #endregion

        #region GetfilteredPersons
        [Fact]
        public void GetFilterdPersons_EmptySearchstring()
        {
            //arrange
            CountryAddRequest country1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country2 = new CountryAddRequest() { CountryName = "Egypt" };

            CountryResponse countryResponse1 = _countriesService.AddCountry(country1);
            CountryResponse countryResponse2 = _countriesService.AddCountry(country2);

            PersonAddRequest person1 = new PersonAddRequest() { PersonName = "Person3 name", Email = "person3@email.com", Address = "Person678", DateOfBirth = DateTime.Parse("2002-11-01"), Gender = GenderOptions.Female, CountryID = countryResponse1.CountryID, RecieveNewsLetters = false };
            PersonAddRequest person2 = new PersonAddRequest() { PersonName = "Person4 name", Email = "person4@email.com", Address = "Person655", DateOfBirth = DateTime.Parse("2008-11-01"), Gender = GenderOptions.Other, CountryID = countryResponse2.CountryID, RecieveNewsLetters = false };

            PersonResponse personResponse1 = _personsService.AddPerson(person1);
            PersonResponse personResponse2 = _personsService.AddPerson(person2);

            List<PersonResponse> personResponsesFromAdd = new List<PersonResponse>() { personResponse1, personResponse2 };

            //test print
            _testOutputHelper.WriteLine("Expected: ");
            personResponsesFromAdd.ForEach(person => _testOutputHelper.WriteLine(person.ToString()));

            List<PersonResponse> personResponsesfromFilterd = _personsService.GetFilteredPersons(nameof(Person.PersonName), "");

            _testOutputHelper.WriteLine("Actual: ");
            personResponsesfromFilterd.ForEach(person => _testOutputHelper.WriteLine(person.ToString()));
            foreach (var personResponseFromAdd in personResponsesFromAdd)
            {
                Assert.Contains(personResponseFromAdd, personResponsesfromFilterd);
            }


            //act


        }

        [Fact]
        public void GetFilterdPersons_SearchbyName()
        {
            //arrange
            CountryAddRequest country1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country2 = new CountryAddRequest() { CountryName = "Egypt" };

            CountryResponse countryResponse1 = _countriesService.AddCountry(country1);
            CountryResponse countryResponse2 = _countriesService.AddCountry(country2);

            PersonAddRequest person1 = new PersonAddRequest() { PersonName = "Smith", Email = "person3@email.com", Address = "Person678", DateOfBirth = DateTime.Parse("2002-11-01"), Gender = GenderOptions.Female, CountryID = countryResponse1.CountryID, RecieveNewsLetters = false };
            PersonAddRequest person2 = new PersonAddRequest() { PersonName = "Mary", Email = "person4@email.com", Address = "Person655", DateOfBirth = DateTime.Parse("2008-11-01"), Gender = GenderOptions.Other, CountryID = countryResponse2.CountryID, RecieveNewsLetters = false };

            PersonResponse personResponse1 = _personsService.AddPerson(person1);
            PersonResponse personResponse2 = _personsService.AddPerson(person2);

            List<PersonResponse> personResponsesFromAdd = new List<PersonResponse>() { personResponse1, personResponse2 };

            //test print
            _testOutputHelper.WriteLine("Expected: ");
            personResponsesFromAdd.ForEach(person => _testOutputHelper.WriteLine(person.ToString()));

            List<PersonResponse> personResponsesfromFilterd = _personsService.GetFilteredPersons(nameof(Person.PersonName), "ma");

            _testOutputHelper.WriteLine("Actual: ");
            personResponsesfromFilterd.ForEach(person => _testOutputHelper.WriteLine(person.ToString()));
            foreach (var personResponseFromAdd in personResponsesFromAdd)
            {
                if (personResponseFromAdd != null)
                {
                    if (personResponseFromAdd.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(personResponseFromAdd, personResponsesfromFilterd);

                    }
                }
            }


            //act


        }
        #endregion
        #region GetSortedPersons
        [Fact]
        public void GetSortedPersons()
        {
            //arrange
            CountryAddRequest country1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country2 = new CountryAddRequest() { CountryName = "Egypt" };

            CountryResponse countryResponse1 = _countriesService.AddCountry(country1);
            CountryResponse countryResponse2 = _countriesService.AddCountry(country2);

            PersonAddRequest person1 = new PersonAddRequest() { PersonName = "Smith", Email = "person3@email.com", Address = "Person678", DateOfBirth = DateTime.Parse("2002-11-01"), Gender = GenderOptions.Female, CountryID = countryResponse1.CountryID, RecieveNewsLetters = false };
            PersonAddRequest person2 = new PersonAddRequest() { PersonName = "Mary", Email = "person4@email.com", Address = "Person655", DateOfBirth = DateTime.Parse("2008-11-01"), Gender = GenderOptions.Other, CountryID = countryResponse2.CountryID, RecieveNewsLetters = false };

            PersonResponse personResponse1 = _personsService.AddPerson(person1);
            PersonResponse personResponse2 = _personsService.AddPerson(person2);

            List<PersonResponse> personResponsesFromAdd = new List<PersonResponse>() { personResponse1, personResponse2 };

            //test print
            _testOutputHelper.WriteLine("Expected: ");
            personResponsesFromAdd.ForEach(person => _testOutputHelper.WriteLine(person.ToString()));

            List<PersonResponse> allPersons = _personsService.GetAllPersons();

            List<PersonResponse> personResponsesfromSort = _personsService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            _testOutputHelper.WriteLine("Actual: ");
            personResponsesfromSort.ForEach(person => _testOutputHelper.WriteLine(person.ToString()));

            personResponsesFromAdd = personResponsesFromAdd.OrderByDescending(temp => temp.PersonName).ToList();

            for (int i = 0; i < allPersons.Count; i++)
            {
                Assert.Equal(personResponsesfromSort[i], personResponsesFromAdd[i]);
            }



        }
        #endregion
        #region PersonUpdate
        [Fact]
        public void PersonUpdate_NullPerson()
        {
            PersonUpdateRequest? personUpdateRequest = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                _personsService.UpdatePerson(personUpdateRequest);
            });
        }

        [Fact]
        public void PersonUpdate_InvalidPersonID()
        {
            PersonUpdateRequest? personUpdateRequest = new PersonUpdateRequest() { PersonID = Guid.NewGuid() };

            Assert.Throws<ArgumentException>(() =>
            {
                _personsService.UpdatePerson(personUpdateRequest);
            });
        }

        [Fact]
        public void PersonUpdate_PersonNameIsNull()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "Uk" };

            CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest() { PersonName = "Ahmed", Email = "Ahmed@email.com", Address = "123 street", DateOfBirth = DateTime.Parse("2000-12-10"), Gender = GenderOptions.Male, CountryID = countryResponse.CountryID, RecieveNewsLetters = true };

            PersonResponse personResponse = _personsService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest() { PersonID = personResponse.PersonID, PersonName = null, Email = "Ahmed27@email.com", Address = "555 street", DateOfBirth = DateTime.Parse("2000-12-10"), Gender = GenderOptions.Male, CountryID = countryResponse.CountryID, RecieveNewsLetters = true };

            Assert.Throws<ArgumentException>(() =>
            {
                _personsService.UpdatePerson(personUpdateRequest);
            });


        }



        [Fact]
        public void PersonUpdate_PersonIsUpdated()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "Uk" };

            CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest() { PersonName = "Ahmed", Email = "Ahmed@email.com", Address = "123 street", DateOfBirth = DateTime.Parse("2000-12-10"), Gender = GenderOptions.Male, CountryID = countryResponse.CountryID, RecieveNewsLetters = true };

            PersonResponse personResponse = _personsService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest() { PersonID = personResponse.PersonID, PersonName = "Ahmed27", Email = "Ahmed27@email.com", Address = "555 street", DateOfBirth = DateTime.Parse("2000-12-10"), Gender = GenderOptions.Male, CountryID = countryResponse.CountryID, RecieveNewsLetters = true };

            PersonResponse updatedPersonResponse = _personsService.UpdatePerson(personUpdateRequest);

            PersonResponse? personResponseFromGet = _personsService.GetPersonByID(personUpdateRequest.PersonID);

            Assert.Equal(personResponseFromGet, updatedPersonResponse);
        }

        #endregion

        #region DeletePerson

        [Fact]
        public void DeletePerson_ValidPersonID()
        {

            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "Uk" };

            CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest() { PersonName = "Ahmed", Email = "Ahmed@email.com", Address = "123 street", DateOfBirth = DateTime.Parse("2000-12-10"), Gender = GenderOptions.Male, CountryID = countryResponse.CountryID, RecieveNewsLetters = true };

            PersonResponse personResponse = _personsService.AddPerson(personAddRequest);

            bool isDeleted = _personsService.DeletePerson(personResponse.PersonID);

            Assert.True(isDeleted);
        }



        [Fact]
        public void DeletePerson_InvalidPersonID()
        {


            bool isDeleted = _personsService.DeletePerson(Guid.NewGuid());

            Assert.False(isDeleted);
        }
        #endregion
    }
}
