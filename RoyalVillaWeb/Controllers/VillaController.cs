using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RoyalVillaDTO;
using RoyalVillaWeb.Models;
using RoyalVillaWeb.Services.IServices;
using System.Diagnostics;

namespace RoyalVillaWeb.Controllers
{
    public class VillaController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly IVillaService _villaService;

        public VillaController(ILogger<HomeController> logger,IMapper mapper,IVillaService villaService)
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
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VillaCreateDTO villaCreateDTO)
        {
            if (!ModelState.IsValid)
            { 
            return View(villaCreateDTO);
            }
            try
            {
                var response = await _villaService.CreateVillaAsync<ApiResponse<VillaDTO>>(villaCreateDTO,"");
                if (response!=null&&response.Success&&response.Data!=null)
                {
                    TempData["Success"]="Villa Created Successfully"; 
                  return  RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["error"]=$"an error occured :{ex.Message}";
            }

            return View(villaCreateDTO);
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id<=0)
            {
                TempData["error"]="Invalid Id ";
                return RedirectToAction(nameof(Index));
            }
                try
            {
                var response = await _villaService.GetAsync<ApiResponse<VillaDTO>>(id, "");
                if (response!=null&&response.Success&&response.Data!=null)
                {
                   
                   return View(response.Data);
                }
            }
            catch (Exception ex)
            {
                TempData["error"]=$"an error occured :{ex.Message}";
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(VillaDTO villaDTO)
        {
          
            try
            {
                var response = await _villaService.DeleteAsync<ApiResponse<object>>(villaDTO.Id, "");
                if (response!=null&&response.Success)
                {
                    TempData["Success"]="Villa Deleted Successfully";
                  
                }
            }
            catch (Exception ex)
            {
                TempData["error"]=$"an error occured :{ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int id)
        {
            if (id<=0)
            {
                TempData["error"]="Invalid Id ";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                var response = await _villaService.GetAsync<ApiResponse<VillaDTO>>(id, "");
                if (response!=null&&response.Success&&response.Data!=null)
                {

                    return View(_mapper.Map<VillaUpdateDTO>(response.Data));
                }
            }
            catch (Exception ex)
            {
                TempData["error"]=$"an error occured :{ex.Message}";
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VillaUpdateDTO villaUpdateDTO)
        {

            try
            {
                var response = await _villaService.UpdateVillaAsync<ApiResponse<object>>(villaUpdateDTO, "");
                if (response!=null&&response.Success&&response.Data!=null)
                {
                    TempData["Success"]="Villa Updated Successfully";

                }
            }
            catch (Exception ex)
            {
                TempData["error"]=$"an error occured :{ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
