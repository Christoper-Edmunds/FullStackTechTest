using Models;
using static DAL.UploadRepository;

namespace DAL;

public interface IUploadRepository
{
    Task DeserializeJsonResult();
}