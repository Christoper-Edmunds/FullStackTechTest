using Models;
using static DAL.UploadRepository;

namespace DAL;

public interface IUploadRepository
{
    //---- People Table
    Task<List<Person>> ListAllAsync();
    Task SaveAsync(Person person);

    //---- Address Table
    Task<Address> GetForPersonIdAsync(int personId);
    Task SaveAsync(Address address);

    Task JSONDeserialize();
}