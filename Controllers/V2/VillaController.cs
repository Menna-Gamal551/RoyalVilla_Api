using Asp.Versioning;
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
using RoyalVillaDTO;

namespace RoyalVilla_Api.Controllers.V2
{
    [Route("/api/V{version:ApiVersion}/[Controller]")]
    [ApiVersion("2.0")]
    [ApiController]

    // [Authorize(Roles ="Admin,Customer")]
    public class VillaController : ControllerBase
    {
        private AppDbContext _db;
        private IMapper _mapper;
        public VillaController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        [HttpGet]
        // [Authorize(Roles ="Admin")]
        //[Route("/Villa")]
      
        public async Task<ActionResult<string>> GetVilla()
        {

            return "this is v2";
        }
        [HttpGet("{id}")]
        //  [AllowAnonymous]
        
        public async Task<ActionResult<string>> GetVillaById(int id)
        {

            return "this is v2 for 1d "+id;
        }
       
    }
}
