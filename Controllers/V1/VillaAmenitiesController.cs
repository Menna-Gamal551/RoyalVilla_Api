using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Numerics;
using RoyalVilla_Api.Data;
using RoyalVilla_Api.Data.Models;
using RoyalVillaDTO;
using Asp.Versioning;

namespace RoyalVilla_Api.Controllers.V1
{
    [Route("api/V{version:ApiVersion}/[controller]")]
    [ApiVersion("1.0")]
  
    [ApiController]
    public class VillaAmenitiesController : ControllerBase
    {
        private AppDbContext _db;
        private IMapper _mapper;
        public VillaAmenitiesController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<VillaAmentyDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<VillaAmentyDTO>>>> GetVillaAmenities()
        {
            try
            {
                var villaAmenities = await _db.villaAmenities.Include(e => e.villa).ToListAsync();
                if (villaAmenities == null)
                {
                    return NotFound(ApiResponse<object>.NotFound("there is not Villa Amenities yet:-("));
                }
                var villaAmenitiesDTo = _mapper.Map<List<VillaAmentyDTO>>(villaAmenities);
                var response = ApiResponse<IEnumerable<VillaAmentyDTO>>.Ok("", villaAmenitiesDTo);
                return Ok(response);
            }
            catch (Exception ex)
            {

                var errorresponse = ApiResponse<object>.Error(500, "An error occured while Deleting villa Amenty ", ex.Message);

                return StatusCode(500, errorresponse);
            }

        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<VillaAmentyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<VillaAmentyDTO>>> GetVillaAmenty(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Id Must be greater than 0"));
                }
                var villaAmenty = await _db.villaAmenities.Include(e => e.villa).FirstOrDefaultAsync(e => e.Id == id);
                if (villaAmenty == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"villa Amenty with Id : {id} not found"));
                }
                var respone = ApiResponse<VillaAmentyDTO>.Ok("retrivied successfully ", _mapper.Map<VillaAmentyDTO>(villaAmenty));
                return Ok(respone);
            }
            catch (Exception ex)
            {

                var errorresponse = ApiResponse<object>.Error(500, "An error occured while Deleting villa Amenty ", ex.Message);

                return StatusCode(500, errorresponse);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<VillaAmentyDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<VillaAmentyDTO>>> CreateVillaAmenty(VillaAmentyCreateDTO villaAmentyCreateDTO)
        {
            try
            {
                if (villaAmentyCreateDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Villa Amenty Data Is Required !"));
                }
                VillaAmenities villaAmenities = _mapper.Map<VillaAmenities>(villaAmentyCreateDTO);
                villaAmenities.villa = await _db.Villas.FirstOrDefaultAsync(e => e.Id == villaAmentyCreateDTO.villaId);
                if (villaAmenities.villa == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Villa  with id : {villaAmentyCreateDTO.villaId} not found"));
                }
                villaAmenities.CreateDate = DateTime.Now;
                await _db.villaAmenities.AddAsync(villaAmenities);
                await _db.SaveChangesAsync();
                var response = ApiResponse<VillaAmentyDTO>.CreatedAt("Villa Amenty Created Successfully", _mapper.Map<VillaAmentyDTO>(villaAmenities));
                return CreatedAtAction(nameof(CreateVillaAmenty), new { id = villaAmenities.villaId }, response);
            }
            catch (Exception ex)
            {

                var errorresponse = ApiResponse<object>.Error(500, "An error occured while creating villa Amenty ", ex.Message);

                return StatusCode(500, errorresponse);
            }

        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<VillaAmentyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<VillaAmentyDTO>>> UpdateVillaAmenty(int id, VillaAmentyUpdateDTO villaAmentyUpdateDTO)
        {
            try
            {
                if (villaAmentyUpdateDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Villa Amenty Data Is Required !"));
                }
                if (villaAmentyUpdateDTO.Id != id)
                {
                    return NotFound(ApiResponse<object>.NotFound($"villa  Amenty with id : {id} in url not Matches Villa Amenty id in  response body  "));
                }

                var villaAmenty = await _db.villaAmenities.FirstOrDefaultAsync(e => e.Id == id);
                if (villaAmenty == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Villa Amenty with id : {villaAmentyUpdateDTO.Id} not found"));
                }
                _mapper.Map(villaAmentyUpdateDTO, villaAmenty);
                villaAmenty.UpdateDate = DateTime.Now;
                villaAmenty.villa = await _db.Villas.FirstOrDefaultAsync(e => e.Id == villaAmentyUpdateDTO.villaId);
                if (villaAmenty.villa == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Villa  with id : {villaAmentyUpdateDTO.villaId} not found"));
                }

                await _db.SaveChangesAsync();
                var respone = ApiResponse<VillaAmentyDTO>.Ok("retrivied successfully ", _mapper.Map<VillaAmentyDTO>(villaAmenty));
                return Ok(respone);
            }
            catch (Exception ex)
            {

                var errorresponse = ApiResponse<object>.Error(500, "An error occured while Deleting villa Amenty ", ex.Message);

                return StatusCode(500, errorresponse);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<ApiResponse<object>>> DeleteVillaAmenty(int id)
        {
            try
            {
                var villaAmenty = await _db.villaAmenities.FirstOrDefaultAsync(e => e.Id == id);
                if (villaAmenty == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Villa Amenty  with id : {id} not found"));
                }

                _db.Remove(villaAmenty);
                await _db.SaveChangesAsync();
                return Ok(ApiResponse<object>.NoContent("Villa Amenty Deleted successfully"));
            }
            catch (Exception ex)
            {

                var errorresponse = ApiResponse<object>.Error(500, "An error occured while Deleting villa Amenty ", ex.Message);

                return StatusCode(500, errorresponse);
            }
        }


    }
}
