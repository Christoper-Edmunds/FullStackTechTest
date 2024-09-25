using DAL;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;

namespace FullStackTechTest.Models.Home;

public class UploadViewModel
{
    public List<Specialities> Specialities { get; set; }
    public List<SelectListItem> SpecialitiesSelectList { get; set; }
    public int SelectedSpeciality { get; set; }

    public static async Task<UploadViewModel> CreateAsync(ISpecialityRepository specialitiesRepository)
    {
        var model = new UploadViewModel
        {
            Specialities = await specialitiesRepository.ListAllSpecialitiesAsync(),
            SpecialitiesSelectList = new List<SelectListItem>(),

        };
        return model;
    }
}