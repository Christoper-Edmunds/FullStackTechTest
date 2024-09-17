using System.Diagnostics;
using DAL;
using Microsoft.AspNetCore.Mvc;
using FullStackTechTest.Models.Home;
using FullStackTechTest.Models.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FullStackTechTest.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IPersonRepository _personRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IUploadRepository _uploadRepository;
    private readonly ISpecialityRepository _specialityRepository;

    public HomeController(ILogger<HomeController> logger, IPersonRepository personRepository, IAddressRepository addressRepository, IUploadRepository uploadRepository, ISpecialityRepository specialityRepository)
    {
        _logger = logger;
        _personRepository = personRepository;
        _addressRepository = addressRepository;
        _uploadRepository = uploadRepository;
        _specialityRepository = specialityRepository;
    }

    public async Task<IActionResult> Index()
    {
        var model = await IndexViewModel.CreateAsync(_personRepository);
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var model = await DetailsViewModel.CreateAsync(id, false, _personRepository, _addressRepository, _specialityRepository);

        model.SpecialitiesSelectList = model.Specialities
        .Select(s => new SelectListItem
        {
            Value = s.Id.ToString(),
            Text = s.SpecialityName
        })
        .ToList();

        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await DetailsViewModel.CreateAsync(id, true, _personRepository, _addressRepository, _specialityRepository);
        return View("Details", model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, [FromForm] DetailsViewModel model)
    {
        await _personRepository.SaveAsync(model.Person);
        await _addressRepository.SaveAsync(model.Address);
        return RedirectToAction("Details", new { id = model.Person.Id });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<IActionResult> Upload()
    {
        var model = await UploadViewModel.CreateAsync();
        return View(model);
    }

    public async Task<IActionResult> DeserializeJsonResult()
    {
        await _uploadRepository.DeserializeJsonResult();

        return RedirectToAction("Upload");
    }

    public async Task<IActionResult> GetAllSpecialities()
    {
        var specialities = await _specialityRepository.ListAllSpecialitiesAsync();

        return View(specialities);

    }

    public async Task<IActionResult> GetPersonsSpecialities(int id)
    {
        var specialities = await _specialityRepository.ListAllSpecialitiesByIdAsync(id);

        return View(specialities);
    }


}