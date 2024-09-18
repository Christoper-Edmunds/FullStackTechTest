using Models;

namespace DAL
{
    public interface ISpecialityRepository
    {
        Task<List<Specialities>> ListAllSpecialitiesAsync();
        Task<List<Specialities>> ListAllSpecialitiesByIdAsync(int personId);
        Task RemoveSpecialitiesFromLinkTable(int personId, int specialityId);
        Task SaveSpecialitiesToLinkTable(int personId, int specialityId);
    }
}