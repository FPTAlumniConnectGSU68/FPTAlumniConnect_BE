﻿namespace FPTAlumniConnect.BusinessTier.Payload.Mentorship;

public class MentorshipReponse
{
    public int Id { get; set; }

    public int? AumniId { get; set; }
    public string? AlumniName { get; set; }

    public string? RequestMessage { get; set; }
    public DateTime CreatedAt { get; set; }

    public string? Type { get; set; }

    public string? Status { get; set; }
}
