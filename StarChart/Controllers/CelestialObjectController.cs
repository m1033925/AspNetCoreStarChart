using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [ApiController]
    [Route("")]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            CelestialObject satellite = new CelestialObject()
            {
                OrbitedObjectId = 1
            };

            if (_context.CelestialObjects.Any(x => x.Id == id))
            {
                var res = _context.CelestialObjects.Where(x => x.Id == id).FirstOrDefault();
                res.Satellites = new List<CelestialObject>();
                res.Satellites.Add(satellite);
            }

            if (!(_context.CelestialObjects.Where(x => x.Id == id).Count() > 0))
            {
                return NotFound();
            }

            if (_context.CelestialObjects.Where(x => x.Id == id).Count() > 0)
            {
                return Ok(_context.CelestialObjects.Where(x => x.Id == id).FirstOrDefault());
            }

            return Ok(id);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            CelestialObject satellite1 = new CelestialObject()
            {
                OrbitedObjectId = 1,
                Name = "Sun"
            };

            CelestialObject satellite2 = new CelestialObject()
            {
                OrbitedObjectId = 2,
                Name = "Earth"
            };

            var satelliteList = new List<CelestialObject>();
            satelliteList.Add(satellite1);
            satelliteList.Add(satellite2);

            if (!(_context.CelestialObjects.Where(x => x.Name == name).Count() > 0))
            {
                return NotFound();
            }

            if (_context.CelestialObjects.Where(x => x.Name == name).Count() > 0)
            {
                var res = _context.CelestialObjects.Where(x => x.Name == name);
                foreach (var item in res)
                {
                    item.Satellites = new List<CelestialObject>();
                    item.Satellites = satelliteList.Where(x => x.OrbitedObjectId == item.Id).ToList();
                }
                return Ok(res);
            }

            return Ok(name);
        }

        [HttpGet]
        public IActionResult GetAll()
        {

            CelestialObject satellite1 = new CelestialObject()
            {
                OrbitedObjectId = 1
            };

            CelestialObject satellite2 = new CelestialObject()
            {
                OrbitedObjectId = 2
            };

            var satelliteList = new List<CelestialObject>();
            satelliteList.Add(satellite1);
            satelliteList.Add(satellite2);

            foreach (var item in _context.CelestialObjects)
            {
                item.Satellites = new List<CelestialObject>();
                item.Satellites = satelliteList.Where(x => x.OrbitedObjectId == item.Id).ToList();
            }

            return Ok(_context.CelestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestial)
        {
            _context.Add(celestial);
            int id = _context.SaveChanges();
            var routeValues = new { id = id };
            return CreatedAtRoute("GetById", routeValues, celestial);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObj)
        {
            CelestialObject celestial = _context.CelestialObjects.Where(x => x.Id == id).FirstOrDefault();
            if(celestial == null || celestial.Id <= 0)
            {
                return NotFound();
            }
            else
            {
                celestial.Name = celestialObj.Name;
                celestial.OrbitalPeriod = celestialObj.OrbitalPeriod;
                celestial.OrbitedObjectId = celestialObj.OrbitedObjectId;
                _context.Update(celestial);
                _context.SaveChanges();
                return NoContent();
            }
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            CelestialObject celestial = _context.CelestialObjects.Where(x => x.Id == id).FirstOrDefault();
            if (celestial == null || celestial.Id <= 0)
            {
                return NotFound();
            }
            else
            {
                celestial.Name = name;
                _context.Update(celestial);
                _context.SaveChanges();
                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            List<CelestialObject> celestialList = _context.CelestialObjects.Where(x => x.Id == id || x.OrbitedObjectId == id).ToList();
            if (celestialList == null || celestialList.Count <= 0)
            {
                return NotFound();
            }
            else
            {
                _context.CelestialObjects.RemoveRange(celestialList);
                _context.SaveChanges();
                return NoContent();
            }
        }
    }
}
