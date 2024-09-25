using Models;
using static DAL.UploadRepository;
using Microsoft.AspNetCore.Http;


namespace DAL;

public interface IUploadRepository
{
    Task DeserializeJsonResult(IFormFile fileUpload);
}