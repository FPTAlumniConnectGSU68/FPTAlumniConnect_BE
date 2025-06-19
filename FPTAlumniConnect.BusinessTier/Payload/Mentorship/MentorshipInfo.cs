using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.Mentorship;

public class MentorshipInfo
{
    public int? AlumniId { get; set; }

    public string? RequestMessage { get; set; }

    public string? Type { get; set; }

    public string? Status { get; set; }
}
