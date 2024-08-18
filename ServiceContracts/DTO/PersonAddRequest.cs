using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ServiceContracts.Enums;
using Entities;

namespace ServiceContracts.DTO
{
    public class PersonAddRequest
    {
        [Required(ErrorMessage = "person name cannot be blank")]

        public string? PersonName { get; set; }
        [Required(ErrorMessage = "Email cannot be blank")]
        [EmailAddress(ErrorMessage = "Please enter a valid Email address")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "Please select a gender")]
        public GenderOptions? Gender { get; set; }
        [Required(ErrorMessage = "Please select a Country")]
        public Guid CountryID { get; set; }
        [Required(ErrorMessage = "Please enter address")]
        public string? Address { get; set; }
        public bool RecieveNewsLetters { get; set; }

        public Person ToPerson()
        {
            return new Person() { PersonName = PersonName, Email = Email, DateOfBirth = DateOfBirth, Gender = Gender.ToString(), CountryID = CountryID, Address = Address, RecieveNewsLetters = RecieveNewsLetters };
        }

    }
}
