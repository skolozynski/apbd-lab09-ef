using EntityFrameworkApp.Data;
using EntityFrameworkApp.Models;
using EntityFrameworkApp.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly ApbdContext _context;
    public TripsController(ApbdContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] int pageNum, [FromQuery] int pageSize = 10)
    {
        var trips = await _context.Trips
            .Select( e => new
            {
               Name = e.Name,
               Description = e.Description,
               DateFrom = e.DateFrom,
               DateTo = e.DateTo,
               MaxPeople = e.MaxPeople,
               Countries = e.IdCountries.Select( c => new
               {
                   Name = c.Name
               }),
               Clients = e.ClientTrips.Select(c => new
               {
                   FirstName = c.IdClientNavigation.FirstName,
                   LastName = c.IdClientNavigation.LastName,
               })
            }).ToListAsync();
        
        return Ok( new
        {
            PageNum = pageNum,
            PageSize = pageSize,
            AllPages = trips.Count/pageSize,
            Trips = trips
        });
    }


    [HttpPost("/api/trips/{id:int}/clients/")]
    public async Task<IActionResult> AddClientToTrip(int id, ClientDTO clientDto)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(e => e.Pesel == clientDto.Pesel);
        if (client != null)
        {
            return BadRequest("Client with this PESEL already exists.");
        }

        var clientId = await _context.Clients
            .Where(e => e.Pesel == clientDto.Pesel).Select(e => e.IdClient).FirstOrDefaultAsync();
        var clientInTrip = await _context.ClientTrips
            .Where(e => e.IdClient == clientId && e.IdTrip == id).FirstOrDefaultAsync();
        if (clientInTrip != null)
        {
            return BadRequest("Client is already in this trip.");
        }
        var trip = await _context.Trips.FirstOrDefaultAsync(e => e.IdTrip == id && e.DateFrom > DateTime.Now);
        if (trip != null)
        {
            return BadRequest("Trip has already started.");
        }
        
        var newClient = await _context.Clients.AddAsync(new Client()
        {
            FirstName = clientDto.FirstName,
            LastName = clientDto.LastName,
            Email = clientDto.Email,
            Telephone = clientDto.Telephone,
            Pesel = clientDto.Pesel
        });
        await _context.ClientTrips.AddAsync(new ClientTrip()
        {
            IdClient = newClient.Entity.IdClient,
            IdTrip = id,
            RegisteredAt = DateTime.Now,
            PaymentDate = clientDto.PaymentDate
        });
        
        await _context.SaveChangesAsync();
        
        return Ok("Client has been added to trip.");
    }
}