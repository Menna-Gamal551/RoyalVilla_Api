using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RoyalVillaDTO;
using RoyalVillaWeb.Models;
using RoyalVillaWeb.Services.IServices;
using System.Diagnostics;

namespace RoyalVillaWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly IVillaService _villaService;

        public HomeController(ILogger<HomeController> logger,IMapper mapper,IVillaService villaService)
        {
            _logger = logger;
            _mapper = mapper;
            _villaService = villaService;
        }

        public async Task<IActionResult> Index()
        {
                List<VillaDTO> VillaList = new();
            try
            {
                var response = await _villaService.GetAllAsync<ApiResponse<List<VillaDTO>>>("");
                if (response!=null&&response.Success&&response.Data!=null)
                {
                    VillaList = response.Data;
                }
            }
            catch (Exception ex)
            {
                TempData["error"]=$"an error occured :{ex.Message}";
            }

            return View(VillaList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
