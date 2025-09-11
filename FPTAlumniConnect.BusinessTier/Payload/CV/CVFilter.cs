using FPTAlumniConnect.BusinessTier.Utils;
using FPTAlumniConnect.DataTier.Enums;
using FPTAlumniConnect.DataTier.Models;
using System.Linq.Expressions;

namespace FPTAlumniConnect.BusinessTier.Payload.CV
{
    public class CVFilter
    {
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public string? Status { get; set; }
        public int? UserId { get; set; }
        public string? City { get; set; }
        public string? JobLevel { get; set; }
        public int? MinSalary { get; set; }
        public int? MaxSalary { get; set; }
        public bool? IsDeal { get; set; }
        public string? DesiredJob { get; set; }
        public string? Position { get; set; }
        public int? MajorId { get; set; }

        public Expression<Func<Cv, bool>> BuildPredicate()
        {
            var predicate = PredicateBuilder.True<Cv>();

            if (!string.IsNullOrEmpty(Status))
            {
                if (Enum.TryParse<CVStatus>(Status, out var parsedStatus))
                    predicate = predicate.And(x => x.Status == parsedStatus);
            }

            if (UserId.HasValue)
                predicate = predicate.And(x => x.UserId == UserId.Value);

            if (!string.IsNullOrEmpty(City))
                predicate = predicate.And(x => x.City == City);

            if (MinSalary.HasValue)
                predicate = predicate.And(x => x.MinSalary >= MinSalary.Value);

            if (MaxSalary.HasValue)
                predicate = predicate.And(x => x.MaxSalary <= MaxSalary.Value);

            if (IsDeal.HasValue)
                predicate = predicate.And(x => x.IsDeal == IsDeal.Value);

            if (!string.IsNullOrEmpty(DesiredJob))
                predicate = predicate.And(x => x.DesiredJob == DesiredJob);

            if (!string.IsNullOrEmpty(Position))
                predicate = predicate.And(x => x.Position == Position);

            if (MajorId.HasValue)
                predicate = predicate.And(x => x.MajorId == MajorId.Value);

            return predicate;
        }
    }
}