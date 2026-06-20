using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GymMarket.API.Models;

public partial class Trainer
{
    public string TrainerId { get; set; } = null!;

    public string? Name { get; set; }

    public string? Email { get; set; }

    [JsonIgnore]
    public string? Password { get; set; }

    public string? Certification { get; set; }

    public string? Bio { get; set; }

    public int? Experience { get; set; }

    public decimal? Rating { get; set; }

    public string? ProfilePicture { get; set; }

    [Column("Desciption")]
    public string Description { get; set; } = string.Empty;

    // Receiving bank account for course payments. Students pay via a VietQR built
    // from these, and the trainer reconciles transfers against their own bank.
    public string? BankBin { get; set; }

    public string? BankAccountNo { get; set; }

    public string? BankAccountName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UserId { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual AppUser? AppUser { get; set; }
}
