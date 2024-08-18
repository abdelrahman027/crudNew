using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    public interface IPersonsService
    {
        PersonResponse AddPerson(PersonAddRequest personAddRequest);

        List<PersonResponse> GetAllPersons();

        PersonResponse GetPersonByID(Guid? personID);

        List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString);

        List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortedBy, SortOrderOptions sortOrder);
        PersonResponse UpdatePerson(PersonUpdateRequest personUpdateRequest);
        bool DeletePerson(Guid? personID);

    }
}
