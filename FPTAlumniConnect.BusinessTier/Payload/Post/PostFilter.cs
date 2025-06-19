using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.Post
{
    public class PostFilter
    {
        public int? AuthorId { get; set; }

        public string? Title { get; set; }

        public int? Views { get; set; }

        public int? MajorId { get; set; }

        public bool? IsPrivate { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
