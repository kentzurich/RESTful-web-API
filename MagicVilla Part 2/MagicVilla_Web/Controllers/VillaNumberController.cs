using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Models.ViewModel;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Data;

namespace MagicVilla_Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IVillaNumberService _villaNumberService;
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;

        public VillaNumberController(IVillaNumberService villaNumberService, 
                                     IMapper mapper, 
                                     IVillaService villaService)
        {
            _villaNumberService = villaNumberService;
            _villaService = villaService;
            _mapper = mapper;
        }

        public async Task<IActionResult> IndexVillaNumber()
        {
            List<VillaNumberDTO> list = new();
            var response = await _villaNumberService.GetAllAsync<APIResponse>();

            if (response != null && response.IsSuccess)
                list = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(response.Result.ToString());

            return View(list);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateVillaNumber()
        {
            VillaNumberCreateVM villaNumberVM = new();
            villaNumberVM.VillaList = await PopulateDropdown(villaNumberVM.VillaList);
            return View(villaNumberVM);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateVM model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaNumberService.CreateAsync<APIResponse>(model.VillaNumber);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Villa Number created successfully.";
                    return RedirectToAction(nameof(IndexVillaNumber));
                }
                else
                {
                    TempData["error"] = (response.ErrorMessages != null && response.ErrorMessages.Count > 0 ?
                        response.ErrorMessages[0] : "Error encountered.");
                }
            }

            model.VillaList = await PopulateDropdown(model.VillaList);
            TempData["success"] = "Error encountered.";
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateVillaNumber(int villaId)
        {
            VillaNumberUpdateVM villaNumberVM = new();
            var response = await _villaNumberService.GetAsync<APIResponse>(villaId);
            if (response != null && response.IsSuccess)
            {
                VillaNumberDTO villaDTO = JsonConvert.DeserializeObject<VillaNumberDTO>(response.Result.ToString());
                villaNumberVM.VillaNumber = _mapper.Map<VillaNumberUpdateDTO>(villaDTO);
            }
            else
            {
                TempData["error"] = (response.ErrorMessages != null && response.ErrorMessages.Count > 0 ?
                    response.ErrorMessages[0] : "Error encountered.");
            }
            villaNumberVM.VillaList = await PopulateDropdown(villaNumberVM.VillaList);
            return View(villaNumberVM);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdateVM model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaNumberService.UpdateAsync<APIResponse>(model.VillaNumber);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Villa Number updated successfully.";
                    return RedirectToAction(nameof(IndexVillaNumber));
                }
                else
                {
                    TempData["error"] = (response.ErrorMessages != null && response.ErrorMessages.Count > 0 ?
                        response.ErrorMessages[0] : "Error encountered.");
                }
            }

            model.VillaList = await PopulateDropdown(model.VillaList);
            TempData["success"] = "Error encountered.";
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteVillaNumber(int villaId)
        {
            VillaNumberDeleteVM villaNumberVM = new();
            var response = await _villaNumberService.GetAsync<APIResponse>(villaId);
            if (response != null && response.IsSuccess)
            {
                VillaNumberDTO villaDTO = JsonConvert.DeserializeObject<VillaNumberDTO>(response.Result.ToString());
                villaNumberVM.VillaNumber = _mapper.Map<VillaNumberDTO>(villaDTO);
            }
            else
            {
                TempData["error"] = (response.ErrorMessages != null && response.ErrorMessages.Count > 0 ?
                    response.ErrorMessages[0] : "Error encountered.");
            }
            villaNumberVM.VillaList = await PopulateDropdown(villaNumberVM.VillaList);
            return View(villaNumberVM);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVillaNumber(VillaNumberDeleteVM model)
        {
            var response = await _villaNumberService.DeleteAsync<APIResponse>(model.VillaNumber.VillaNo);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Villa Number deleted successfully.";
                return RedirectToAction(nameof(IndexVillaNumber));
            }
            else
            {
                TempData["error"] = (response.ErrorMessages != null && response.ErrorMessages.Count > 0 ?
                    response.ErrorMessages[0] : "Error encountered.");
            }
            return View(model);
        }

        private async Task<IEnumerable<SelectListItem>> PopulateDropdown(IEnumerable<SelectListItem> obj)
        {
            var response = await _villaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                obj = JsonConvert
                    .DeserializeObject<List<VillaDTO>>(response.Result.ToString())
                    .Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    });
            }
            return obj;
        }
    }
}
