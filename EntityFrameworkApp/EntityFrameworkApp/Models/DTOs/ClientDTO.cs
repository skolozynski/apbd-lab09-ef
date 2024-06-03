namespace EntityFrameworkApp.Models.DTOs;

public class ClientDTO
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Telephone { get; set; } = null!;

    public string Pesel { get; set; } = null!;

    public string TripName { get; set; } = null!;

    public DateTime PaymentDate { get; set; }
}