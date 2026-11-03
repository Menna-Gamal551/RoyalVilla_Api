using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RoyalVilla_Api.Data;
using RoyalVilla_Api.Data.Models;
using RoyalVilla_Api.Services.IServices;
using RoyalVillaDTO;
using System.Text;

namespace RoyalVilla_Api.Controllers.V2
{
    [Route("/api/V{version:ApiVersion}/[Controller]")]
    [ApiVersion("2.0")]
    [ApiController]

    // [Authorize(Roles ="Admin,Customer")]
    public class VillaController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        public VillaController(AppDbContext db, IMapper mapper, IImageService imageService)
        {
            _db = db;
            _mapper = mapper;
            _imageService=imageService;
        }
        [HttpGet]
        // [Authorize(Roles ="Admin")]
        //[Route("/Villa")]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVilla([FromQuery]string? filterBy, [FromQuery] string? filterQuery, [FromQuery] string? sortBy
            , [FromQuery] string? sortOrder="asc",int page=1,int pageSize=10)
        {
            if (page<1) page=1;
            if (pageSize<1) pageSize=10;
            if (pageSize>100) pageSize=100;
            var villaQuery= _db.Villas.AsQueryable();
          //  if (!string.IsNullOrEmpty(nameSearch))
          if(!string.IsNullOrEmpty(filterBy)&&!string.IsNullOrEmpty(filterQuery))
            {
               
                switch (filterBy.ToLower())
                {
                    case "name":
                        villaQuery=villaQuery.Where(u => u.Name.ToLower().Contains(filterQuery.ToLower()));
                            break;
                    case "details":
                        villaQuery=villaQuery.Where(u => u.Details.ToLower().Contains(filterQuery.ToLower()));
                        break;
                    case "rate":
                        if(double.TryParse(filterQuery,out double rate))
                         villaQuery=villaQuery.Where(u => u.Rate==rate);
                        break;
                    case "minrate":
                        if (double.TryParse(filterQuery, out double minrate))
                            villaQuery=villaQuery.Where(u => u.Rate>=minrate);
                        break;
                    case "maxrate":
                        if (double.TryParse(filterQuery, out double maxrate))
                            villaQuery=villaQuery.Where(u => u.Rate<=maxrate);
                        break;
                    case "occupancy":
                        if (int.TryParse(filterQuery, out int occupancy))
                            villaQuery=villaQuery.Where(u => u.Occupancy==occupancy);
                        break;
                    case "sqft":
                        if (int.TryParse(filterQuery, out int sqft))
                            villaQuery=villaQuery.Where(u => u.Sqft==sqft);
                        break;
                }
                
            }
            if (!string.IsNullOrEmpty(sortBy))
            {
                var isDescending = sortOrder?.ToLower()=="desc";
                villaQuery=sortBy.ToLower() switch
                {
                    "name" => isDescending ? villaQuery.OrderByDescending(u => u.Name) : villaQuery.OrderBy(u => u.Name),
                    "rate" => isDescending ? villaQuery.OrderByDescending(u => u.Rate) : villaQuery.OrderBy(u => u.Rate),
                    "occupancy" => isDescending ? villaQuery.OrderByDescending(u => u.Occupancy) : villaQuery.OrderBy(u => u.Occupancy),
                    "sqft" => isDescending ? villaQuery.OrderByDescending(u => u.Sqft) : villaQuery.OrderBy(u => u.Sqft),
                    "id" => isDescending ? villaQuery.OrderByDescending(u => u.Id) : villaQuery.OrderBy(u => u.Id),
                    _ => villaQuery.OrderBy(u => u.Id),
                };
            } else
            {
                villaQuery= villaQuery.OrderBy(u => u.Id);
            }
            var skip = (page-1)*pageSize;
            var totalVilla = await villaQuery.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalVilla/pageSize);

            var villas = await villaQuery.Skip(skip).Take(pageSize).ToListAsync();
            var dtoResponseVilla = _mapper.Map<List<VillaDTO>>(villas);
            

            var messagebuilder = new StringBuilder();
            messagebuilder.Append($"a successful Retrived {dtoResponseVilla.Count} villa/s ");
            messagebuilder.Append($"(Page {page} of {totalPages}),{totalVilla} total records ");
            if (!string.IsNullOrEmpty(filterBy)&&!string.IsNullOrEmpty(filterQuery))
            {
                messagebuilder.Append($"filtered by {filterBy}: '{filterQuery}'");
            }
            if (!string.IsNullOrEmpty(sortBy))
            {
                messagebuilder.Append($"sorted by {sortBy}: '{sortOrder?.ToLower()??"asc"}' ");
            }

            Response.Headers.Append("X-Pagination-CurrentPage",page.ToString());
            Response.Headers.Append("X-Pagination-PageSize",pageSize.ToString());
            Response.Headers.Append("X-Pagination-TotalPages",totalPages.ToString());
            Response.Headers.Append("X-Pagination-TotalVillas",totalVilla.ToString());


                var response = ApiResponse<IEnumerable<VillaDTO>>.Ok(messagebuilder.ToString(), dtoResponseVilla);
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
                if (id <= 0)
                {
                    return NotFound(ApiResponse<object>.NotFound("Villa Id Must be greater than 0"));

                }

                var villa = await _db.Villas.Where(e => e.Id == id).SingleOrDefaultAsync();
                if (villa == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"villa With id: {id} Not exsit"));
                }
                return Ok(ApiResponse<VillaDTO>.Ok("Records retrived Successfully", _mapper.Map<VillaDTO>(villa)));
            }
            catch (Exception ex)
            {
                var errorresponse = ApiResponse<object>.Error(500, "An error occured while Creating villa ", ex.Message);

                return StatusCode(500, errorresponse);
            }

        }
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [AllowAnonymous]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status201Created)]

        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateVilla([FromForm]VillaCreateDTO villaDTO)
        {
            try
            {
                if (villaDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("villa properties is required"));
                }
                var duplicateVilla = await _db.Villas.FirstOrDefaultAsync(e => e.Name.ToLower() == villaDTO.Name.ToLower());
                if (duplicateVilla != null)
                {
                    return Conflict(ApiResponse<object>.Conflict($"a villa with Name{villaDTO.Name} is already exsit"));
                }
                Villa villa = _mapper.Map<Villa>(villaDTO);
                if (villaDTO.Image!=null)
                {
                    if (!_imageService.ValidateImage(villaDTO.Image))
                    {
                        return BadRequest(
                         "Image file invalid. Allowed formats {jpg,png,jpeg} Maximum size is 5 MB.");
                    }
                    villa.ImageUrl= await _imageService.UploadImageAsync(villaDTO.Image);
                }
                

                _db.Villas.Add(villa);
                await _db.SaveChangesAsync();
                var response = ApiResponse<VillaDTO>.CreatedAt("Created successfully", _mapper.Map<VillaDTO>(villa));
                return CreatedAtAction(nameof(GetVillaById), new { id = villa.Id }, response);

            }

            catch (Exception ex)
            {
                var errorresponse = ApiResponse<object>.Error(500, "An error occured while Creating villa ", ex.Message);

                return StatusCode(500, errorresponse);
            }

        }
        [HttpPut("{id:int}")]
        //[Authorize(Roles = "Admin")]
        [AllowAnonymous]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateVilla([FromForm]VillaUpdateDTO villaDTO, int id)
        {
            try
            {
                if (villaDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("villa properties is required"));
                }

                if (villaDTO.Id != id)
                {
                    return NotFound(ApiResponse<object>.NotFound($"villa with id : {id} in url not Matches Villa id in  response body  "));
                }
                if (villaDTO.Image!=null&&!_imageService.ValidateImage(villaDTO.Image))
                {
                   
                        return BadRequest(
                         "Image file invalid. Allowed formats {jpg,png,jpeg} Maximum size is 5 MB.");
                   
                }
                var duplicateVilla = await _db.Villas.FirstOrDefaultAsync(e => e.Name.ToLower() == villaDTO.Name.ToLower() && e.Id != id);
                if (duplicateVilla != null)
                {
                    return Conflict(ApiResponse<object>.Conflict($"a villa with Name{villaDTO.Name} is already exsit"));
                }
                Villa exitingvilla = await _db.Villas.FirstOrDefaultAsync(e => e.Id == id);
                if (exitingvilla == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"villa with id : {id} not found "));
                }

                var oldImageUrl=exitingvilla.ImageUrl;
                _mapper.Map(villaDTO, exitingvilla);
                exitingvilla.UpdateDate = DateTime.Now;
                if (villaDTO.Image!=null)
                { 
                exitingvilla.ImageUrl=await _imageService.UploadImageAsync(villaDTO.Image);
                    villaDTO.ImageUrl=exitingvilla.ImageUrl;
                    if (!string.IsNullOrEmpty(oldImageUrl)&&oldImageUrl!=exitingvilla.ImageUrl)
                    {
                        await _imageService.DeleteImageAsync(oldImageUrl);
                    }
                }

                await _db.SaveChangesAsync();
                return Ok(ApiResponse<VillaDTO>.Ok("", _mapper.Map<VillaDTO>(villaDTO)));

            }

            catch (Exception ex)
            {

                var errorresponse = ApiResponse<object>.Error(500, "An error occured while updating villa ", ex.Message);

                return StatusCode(500, errorresponse);
            }

        }


        [HttpDelete("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteVilla(int id)
        {
            try
            {
                var villa = await _db.Villas.Where(e => e.Id == id).SingleOrDefaultAsync();
                if (villa == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"villa with {id} not found "));
                }
                _db.Villas.Remove(villa);
                await _db.SaveChangesAsync();
                return Ok(ApiResponse<object>.NoContent());
            }
            catch (Exception ex)
            {

                var errorresponse = ApiResponse<object>.Error(500, "An error occured while Creating villa ", ex.Message);

                return StatusCode(500, errorresponse);
            }
        }
    }
}
