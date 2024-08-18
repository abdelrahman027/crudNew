﻿using ServiceContracts;
using ServiceContracts.DTO;
using Entities;

namespace Services
{
    public class CountryService : ICountriesService
    {
        private readonly PersonsDbContext _db;

        public CountryService(PersonsDbContext personsDbContext)
        {
            _db = personsDbContext;


        }
        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest == null) throw new ArgumentNullException(nameof(countryAddRequest));
            if(countryAddRequest.CountryName == null) throw new ArgumentException(nameof(countryAddRequest.CountryName));


            if (_db.Countries.Count(temp=>temp.CountryName == countryAddRequest.CountryName) >0)
            {
                throw new ArgumentException("Country name already exists");
            }
            Country country = countryAddRequest.ToCountry();

            country.CountryID = Guid.NewGuid();
            _db.Countries.Add(country); 
            _db.SaveChanges();

            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
           return _db.Countries.Select(country=>country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByID(Guid? CountryID)
        {
            if (CountryID == null) return null;
            Country? countryResponseFromList = _db.Countries.FirstOrDefault(temp=>temp.CountryID==CountryID);
            
            if (countryResponseFromList == null) return null;

            return countryResponseFromList.ToCountryResponse();
        }
    }
}
