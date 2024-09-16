using Models;

namespace DAL
{
    public interface ISpecialityRepository
    {
        Task<List<Specialities>> ListAllSpecialitiesAsync();
        Task<List<string>> ListAllSpecialitiesByIdAsync(int personId);
    }
}