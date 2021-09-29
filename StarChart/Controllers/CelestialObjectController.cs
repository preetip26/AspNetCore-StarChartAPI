using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            List<CelestialObject> allSatellites = new List<CelestialObject>();
            CelestialObject celestialObject = _context.CelestialObjects.FirstOrDefault(celesObj => celesObj.Id == id);
            if (celestialObject == null)
            {
                return NotFound();
            }
            else
            {
                foreach(CelestialObject celesObj in _context.CelestialObjects)
                {
                    if(celesObj.Id == id)
                    {
                        allSatellites.Add(celesObj);
                    }
                }
                celestialObject.Satellites = allSatellites;
                return Ok(celestialObject);
            }
        }
        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            List<CelestialObject> allSatellites = new List<CelestialObject>();
            var celestialObjects = _context.CelestialObjects.Where(celesObj => celesObj.Name == name).ToList();
            if (!celestialObjects.Any())
            {
                return NotFound();
            }
            else
            {
                foreach (var celesObj in celestialObjects)
                {
                    celesObj.Satellites = _context.CelestialObjects.Where(obj => obj.OrbitedObjectId == celesObj.Id).ToList();
                }
                return Ok(celestialObjects);
            }
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();
            foreach (var celesObj in celestialObjects)
            {
                celesObj.Satellites = _context.CelestialObjects.Where(obj => obj.OrbitedObjectId == celesObj.Id).ToList();
            }
            return Ok(celestialObjects);
        }
        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }
        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            CelestialObject obj = _context.CelestialObjects.FirstOrDefault(celesObj => celesObj.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                obj.Name = celestialObject.Name;
                obj.OrbitalPeriod = celestialObject.OrbitalPeriod;
                obj.OrbitedObjectId = celestialObject.OrbitedObjectId;
                _context.CelestialObjects.Update(obj);
                return NoContent();
            }
        }
        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            CelestialObject obj = _context.CelestialObjects.FirstOrDefault(celesObj => celesObj.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                obj.Name = name;
                _context.CelestialObjects.Update(obj);
                return NoContent();
            }
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            List<CelestialObject> allObjects = new List<CelestialObject>();
            CelestialObject celestialObject = _context.CelestialObjects.FirstOrDefault(celesObj => celesObj.Id == id);
            if (celestialObject == null)
            {
                return NotFound();
            }
            else
            {
                foreach (CelestialObject celesObj in _context.CelestialObjects)
                {
                    if (celesObj.Id == id)
                    {
                        allObjects.Add(celesObj);
                    }
                }
                _context.CelestialObjects.RemoveRange(allObjects);
                _context.SaveChanges();
                return NoContent();
            }
        }
    }
}
