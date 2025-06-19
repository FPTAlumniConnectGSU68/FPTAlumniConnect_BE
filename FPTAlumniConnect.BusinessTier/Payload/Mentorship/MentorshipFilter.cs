using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.Mentorship;

public class MentorshipFilter
{
    public int? AumniId { get; set; }

    public string? Type { get; set; }

    public string? Status { get; set; }
}
