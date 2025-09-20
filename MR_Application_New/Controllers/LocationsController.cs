using DAL.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model_New.Models;
using System.Web.Http;

namespace MR_Application_New.Controllers
{
    [Authorize]

    [AllowAnonymous]
    public class LocationsController : Controller
    {

        private readonly MrAppDbNewContext _locationContext;
        private readonly IGenericRepository<OutLetMasterDetail, MrAppDbNewContext> _mapService;




        private List<OutLetMasterDetail> GetDestinations()
        {
            return _locationContext.OutLetMasterDetails.ToList(); // Retrieve all locations from the database
        }
        public LocationsController(MrAppDbNewContext mylocationContext, IGenericRepository<OutLetMasterDetail, MrAppDbNewContext> mapService)
        {
            _locationContext = mylocationContext;
            _mapService = mapService;
        }



        public IActionResult Index()
        {
            var locations = _locationContext.OutLetMasterDetails.ToList();

            return View(locations);

        }

   



        public IActionResult Getcurrent(int? rscode)
        {
            if (rscode.HasValue) // Check if RSCODE is provided in the AJAX request
            {
                // Process the AJAX request to get unique PartyHLLCodes for the selected RSCODE
                var partyNames = _locationContext.OutLetMasterDetails
                                          .AsNoTracking()
                                    .Where(l => l.Rscode == rscode.Value)
                                    .Select(l => l.PartyName)
                                    .Distinct()
                                    .ToList();

                return Json(partyNames); // Return the data as JSON for AJAX request
            }

            // If no RSCODE, render the view with unique RSCODE destinations
            var destinations = GetDestinations()
                                .GroupBy(d => d.Rscode)
                                .Select(g => g.First())
                                .ToList();

            return View(destinations); // Return the view for non-AJAX requests
        }


        [Microsoft.AspNetCore.Mvc.HttpGet]
        public JsonResult GetLocationDetails(int rscode, string partyName)
        {
            var locationDetails = _locationContext.OutLetMasterDetails
                                .Where(l => l.Rscode == rscode && l.PartyName == partyName)
                                .Select(l => new
                                {

                                    l.Address1,
                                    l.Address2,
                                    l.Address3,
                                    l.Address4,
                                    l.PartyHllcode,
                                    l.Latitude,
                                    l.Longitude

                                })
                                .FirstOrDefault();

            return Json(locationDetails);
        }








        [Microsoft.AspNetCore.Mvc.HttpGet]
        public IActionResult Create()
        {
            var destinations = _locationContext.OutLetMasterDetails.Select(l => new
            {
                l.Rscode,
                l.PartyHllcode,
            }).ToList();

            ViewBag.Destinations = destinations;


            ViewBag.SourceLatitude = null;
            ViewBag.SourceLongitude = null;
            ViewBag.DestinationLatitude = null;
            ViewBag.DestinationLongitude = null;

            return View();
        }


    }
}
