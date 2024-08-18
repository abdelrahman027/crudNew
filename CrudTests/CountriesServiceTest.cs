using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Entities;

using Microsoft.EntityFrameworkCore;

namespace CrudTests
{
    public class CountriesServiceTest
    {

        private readonly ICountriesService _countriesService;

        public CountriesServiceTest()
        {
            _countriesService = new CountryService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
        }


        #region AddCountry
        [Fact]
        public void AddCountry_NullCountry()
        {
            CountryAddRequest countryAddRequest = null;
            Assert.Throws<ArgumentNullException>(() =>
            {
                _countriesService.AddCountry(countryAddRequest);
            });
        }

        [Fact]
        public void AddCountry_NullCountryName()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest() { 
            CountryName=null};
            Assert.Throws<ArgumentException>(() =>
            {
                _countriesService.AddCountry(countryAddRequest);
            });
        }

        [Fact]
        public void AddCountry_DublicateName()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            CountryAddRequest countryAddRequest1 = new CountryAddRequest()
            {
                CountryName = "USA"
            };

            Assert.Throws<ArgumentException>(() =>
            {
                _countriesService.AddCountry(countryAddRequest);
                _countriesService.AddCountry(countryAddRequest1);

            });
        }

        [Fact]
        public void AddCountry_ProperName()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "Japan"
            };


            CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);

            List<CountryResponse> allCountries = _countriesService.GetAllCountries();

            Assert.True(countryResponse.CountryID != Guid.Empty);
            //Assert.Contains(countryResponse,allCountries);
        }
        #endregion

        #region GetAllCountries

        [Fact]

        public void GetAllCountries_EmptyList()
        {
            List<CountryResponse> CountriesFromResponse = _countriesService.GetAllCountries();

            Assert.Empty(CountriesFromResponse);
        }

        [Fact]

        public void GetAllCountries_AddFewCountries()
        {

            List<CountryAddRequest> countryReqestList = new List<CountryAddRequest>()
            {
                new CountryAddRequest(){CountryName="UK"},
                new CountryAddRequest(){CountryName="Egypt"}
            };

            List<CountryResponse> CountryListFromAddCountry = new List<CountryResponse>();

            foreach (CountryAddRequest countryReqest in countryReqestList)
            {
                CountryListFromAddCountry.Add(_countriesService.AddCountry(countryReqest));

            }

            List<CountryResponse> ActualCountryResponseList = _countriesService.GetAllCountries();

            foreach (CountryResponse expectedCountry in CountryListFromAddCountry)
            {
                Assert.Contains(expectedCountry, ActualCountryResponseList);
            }
        }
        #endregion

        #region GetCountryByID

        [Fact]
        public void GetCountryByID_NullID()
        {
            Guid? countryID = null;

            CountryResponse? CountryResponseFromGetMethod = _countriesService.GetCountryByID(countryID);

            Assert.Null(CountryResponseFromGetMethod);
        }

        [Fact]
        public void GetCountryByID_ValidCountryID()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "Japan"
            };

            CountryResponse countryFromAddCountry = _countriesService.AddCountry(countryAddRequest);

            CountryResponse? CountryFromGetCountry = _countriesService.GetCountryByID(countryFromAddCountry.CountryID);


            Assert.Equal(countryFromAddCountry, CountryFromGetCountry);
        }
        #endregion
    }
}