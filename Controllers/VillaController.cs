using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RoyalVilla_Api.Data;
using RoyalVilla_Api.Data.Models;
using RoyalVilla_Api.Data.Models.DTO;

namespace RoyalVilla_Api.Controllers
{
    [Route("/api/[Controller]")]
    [ApiController]
   // [Authorize(Roles ="Admin,Customer")]
    public class VillaController : ControllerBase
    {
        private AppDbContext _db;
        private IMapper _mapper;
        public VillaController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper =mapper;
        }
        [HttpGet]
       // [Authorize(Roles ="Admin")]
        //[Route("/Villa")]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>),StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVilla()
        {

            var villas =  await _db.Villas.ToListAsync();
            var dtoResponseVilla = _mapper.Map<List<VillaDTO>>(villas);
            var response = ApiResponse<IEnumerable<VillaDTO>>.Ok("Data Retrived successfully", dtoResponseVilla);
            return Ok(response);
        }
        [HttpGet("{id}")]
      //  [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetVillaById(int id)
        {

            try
            {
                if (id<=0)
                {
                    return NotFound(ApiResponse<object>.NotFound("Villa Id Must be greater than 0"));

                }

                var villa = await _db.Villas.Where(e => e.Id==id).SingleOrDefaultAsync();
                if (villa==null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"villa With id: {id} Not exsit"));
                }
                return Ok(ApiResponse<VillaDTO>.Ok("Records retrived Successfully",_mapper.Map<VillaDTO>(villa)));
            }
            catch (Exception ex)
            {
                var errorresponse = ApiResponse<Object>.Error(500, "An error occured while Creating villa ", ex.Message);

                return StatusCode(500, errorresponse);
            }

        }
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status201Created)]

        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateVilla(VillaCreateDTO villaDTO)
        {
            try {
                if (villaDTO==null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("villa properties is required"));
                }
                var duplicateVilla = await _db.Villas.FirstOrDefaultAsync(e => e.Name.ToLower()==villaDTO.Name.ToLower() );
                if (duplicateVilla!=null)
                {
                    return Conflict(ApiResponse<object>.Conflict($"a villa with Name{villaDTO.Name} is already exsit"));
                }
                Villa villa = _mapper.Map<Villa>(villaDTO);

                _db.Villas.Add(villa);
                await _db.SaveChangesAsync();
                var response = ApiResponse<VillaDTO>.CreatedAt("Created successfully", _mapper.Map<VillaDTO>(villa)) ;
                return CreatedAtAction(nameof(GetVillaById), new { id = villa.Id },response);
                
            }

            catch (Exception ex)
            {
                var errorresponse = ApiResponse<Object>.Error(500, "An error occured while Creating villa ", ex.Message);

                return StatusCode(500, errorresponse);
            }

        }
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateVilla(VillaUpdateDTO villaDTO,int id)
        {
            try
            {
                if (villaDTO==null)
                {
                    return BadRequest(ApiResponse<Object>.BadRequest("villa properties is required"));
                }

                if (villaDTO.Id!=id)
                {
                    return NotFound(ApiResponse<object>.NotFound($"villa with id : {id} in url not Matches Villa id in  response body  "));
                }
                var duplicateVilla = await _db.Villas.FirstOrDefaultAsync(e => e.Name.ToLower()==villaDTO.Name.ToLower() && e.Id!=id);
                if (duplicateVilla!=null)
                {
                    return Conflict(ApiResponse<object>.Conflict($"a villa with Name{villaDTO.Name} is already exsit"));
                }
                Villa exitingvilla = await _db.Villas.FirstOrDefaultAsync(e=>e.Id==id);
                if (exitingvilla==null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"villa with id : {id} not found "));
                }
                _mapper.Map(villaDTO, exitingvilla);
                exitingvilla.UpdateDate= DateTime.Now;
                await _db.SaveChangesAsync();
                return Ok(ApiResponse<VillaDTO>.Ok("", _mapper.Map<VillaDTO>(villaDTO)));

            }

            catch (Exception ex)
            {

                var errorresponse = ApiResponse<Object>.Error(500, "An error occured while Creating villa ", ex.Message);

                return StatusCode(500, errorresponse);
            }

        }


        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            try
            {
                var villa = await _db.Villas.Where(e => e.Id==id).SingleOrDefaultAsync();
                if (villa==null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"villa with {id} not found Bravo Menna i'm so proud for you :-)"));
                }
                _db.Villas.Remove(villa);
                await _db.SaveChangesAsync();
                return Ok(ApiResponse<object>.NoContent());
            }
            catch (Exception ex)
            {

                var errorresponse = ApiResponse<Object>.Error(500, "An error occured while Creating villa ", ex.Message);

                return StatusCode(500, errorresponse);
            }
        }
    }
}
