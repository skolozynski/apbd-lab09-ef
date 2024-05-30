using EntityFrameworkApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{

    private readonly ApbdContext _context;
    public ClientsController(ApbdContext context)
    {
        _context = context;
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(e => e.IdClient == id);
        if (client == null)
        {
            return NotFound($"Client with id {id} does not exist.");
        }

        var trips = await _context.ClientTrips.Where(e => e.IdClient == id).ToListAsync();
        if (trips.Count > 0)
        {
            return BadRequest("Cannot delete client with trips.");
        }
        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        return Ok(client);
    }

}