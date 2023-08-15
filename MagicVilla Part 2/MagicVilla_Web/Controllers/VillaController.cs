using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;

        public VillaController(IVillaService villaService, 
                               IMapper mapper)
        {
            _villaService = villaService;
            _mapper = mapper;
        }
        public async Task<IActionResult> IndexVilla()
        {
            List<VillaDTO> list = new();
            var response = await _villaService.GetAllAsync<APIResponse>();

            if(response != null && response.IsSuccess)
                list = JsonConvert.DeserializeObject<List<VillaDTO>>(response.Result.ToString());

            return View(list);
        }

        [Authorize(Roles = StaticDetails.Admin)]
        public async Task<IActionResult> CreateVilla()
        {
            return View();
        }

        [Authorize(Roles = StaticDetails.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVilla(VillaCreateDTO dto)
        {
            if(ModelState.IsValid)
            {
                var response = await _villaService.CreateAsync<APIResponse>(dto);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Villa created successfully.";
                    return RedirectToAction(nameof(IndexVilla));
                }
            }

            TempData["success"] = "Error encountered.";
            return View(dto);
        }

        [Authorize(Roles = StaticDetails.Admin)]
        public async Task<IActionResult> UpdateVilla(int villaId)
        {
            var response = await _villaService.GetAsync<APIResponse>(villaId);
            if (response != null && response.IsSuccess)
            {
                VillaDTO villaDTO = JsonConvert.DeserializeObject<VillaDTO>(response.Result.ToString());
                return View(_mapper.Map<VillaUpdateDTO>(villaDTO));
            }

            return NotFound();
        }

        [Authorize(Roles = StaticDetails.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVilla(VillaUpdateDTO dto)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.UpdateAsync<APIResponse>(dto);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Villa updated successfully.";
                    return RedirectToAction(nameof(IndexVilla));
                } 
            }

            TempData["success"] = "Error encountered.";
            return View(dto);
        }

        [Authorize(Roles = StaticDetails.Admin)]
        public async Task<IActionResult> DeleteVilla(int villaId)
        {
            var response = await _villaService.GetAsync<APIResponse>(villaId);
            if (response != null && response.IsSuccess)
            {
                VillaDTO villaDTO = JsonConvert.DeserializeObject<VillaDTO>(response.Result.ToString());
                return View(villaDTO);
            }

            return NotFound();
        }

        [Authorize(Roles = StaticDetails.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVilla(VillaDTO dto)
        {
            var response = await _villaService.DeleteAsync<APIResponse>(dto.Id);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Villa Deleted successfully.";
                return RedirectToAction(nameof(IndexVilla));
            }

            TempData["success"] = "Error encountered.";
            return View(dto);
        }
    }
}
