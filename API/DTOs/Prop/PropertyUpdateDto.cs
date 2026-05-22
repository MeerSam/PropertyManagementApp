using System; 

namespace API.DTOs.Prop;

public class PropertyUpdateDto
{ 
    public string? Address { get; set; } = string.Empty;
    public string? Unit { get; set; } = string.Empty;
    public string? City { get; set; } = string.Empty;
    public string? State { get; set; } = string.Empty;
    public string? ZipCode { get; set; } = string.Empty;
    public string? Country { get; set; } = string.Empty;  

    public string? LotNumber { get; set; } = string.Empty;  
    public string? AssignedParking { get; set; } = string.Empty; 
    public string LastUpdatedBy { get; set; } = string.Empty; // DisplayName

    public int? SquareFeet { get; set; }  
    public int? Bedrooms { get; set; } 
    public int? Bathrooms { get; set; }  
    public bool IsRented { get; set; }  = false;
    public bool IsSameAddress { get; set; } = false;
    public string? MailAddress { get; set; } = string.Empty;
    public string? MailUnit { get; set; } = string.Empty;
    public string? MailCity { get; set; } = string.Empty;
    public string? MailState { get; set; } = string.Empty;
    public string? MailZipCode { get; set; } = string.Empty;
    public string? MailCountry { get; set; } = string.Empty;  
}
