using DAL;
using Models;

namespace FullStackTechTest.Models.Home;

public class DetailsViewModel
{
    public Person Person { get; set; }
    public Address Address { get; set; }
    public List<Specialities> Specialities { get; set; }
    public List<Specialities> AllSpecialities { get; set; }


    public bool IsEditing { get; set; }

    public static async Task<DetailsViewModel> CreateAsync(int personId, bool isEditing, IPersonRepository personRepository, IAddressRepository addressRepository, ISpecialityRepository specialitiesRepository)
    {
        var model = new DetailsViewModel
        {
            Person = await personRepository.GetByIdAsync(personId),
            Address = await addressRepository.GetForPersonIdAsync(personId),
            Specialities = await specialitiesRepository.ListAllSpecialitiesByIdAsync(personId),
            AllSpecialities = await specialitiesRepository.ListAllSpecialitiesAsync(),
            IsEditing = isEditing
        };
        return model;
    }
}