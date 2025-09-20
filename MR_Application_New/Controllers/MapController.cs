using DAL.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model_New.Models;

namespace MR_Application_New.Controllers
{

    [Route("api/Map")]
    [ApiController]
    [Authorize]

    [AllowAnonymous]
    public class MapController : Controller
    {

        private readonly MrAppDbNewContext _locationContext;
        private readonly IGenericRepository<OutLetMasterDetail, MrAppDbNewContext> _mapService;

        public MapController(IGenericRepository<OutLetMasterDetail, MrAppDbNewContext> mapService, MrAppDbNewContext mylocationContext)
        {
            _mapService = mapService;
            _locationContext = mylocationContext;
        }





        [HttpGet("getCoordinates")]


        public async Task<IActionResult> GetCoordinatesAsync(int rscode, string PartyName)
        {
            // Replace this with your logic to fetch coordinates based on Rscode and PartyHLLCode


            // Fetch coordinates from the service
            var coordinates = await _mapService.GetCoordinatesAsync(rscode, PartyName);

            // Log what is returned from the service
            Console.WriteLine($"Coordinates found: {coordinates}");

            if (coordinates == null)
            {
                return NotFound("Coordinates not found.");
            }

            return Ok(coordinates);

        }

    }
}
