using System.Linq.Expressions;
using FPTAlumniConnect.DataTier.Enums;
using FPTAlumniConnect.DataTier.Models;

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
            Expression<Func<Cv, bool>> predicate = x => true;

            if (StartAt.HasValue)
            {
                var startAtPredicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.GreaterThanOrEqual(
                        Expression.Property(Expression.Parameter(typeof(Cv), "x"), nameof(Cv.StartAt)),
                        Expression.Constant(StartAt.Value)),
                    Expression.Parameter(typeof(Cv), "x"));
                predicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.AndAlso(predicate.Body, startAtPredicate.Body),
                    predicate.Parameters);
            }

            if (EndAt.HasValue)
            {
                var endAtPredicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.LessThanOrEqual(
                        Expression.Property(Expression.Parameter(typeof(Cv), "x"), nameof(Cv.EndAt)),
                        Expression.Constant(EndAt.Value)),
                    Expression.Parameter(typeof(Cv), "x"));
                predicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.AndAlso(predicate.Body, endAtPredicate.Body),
                    predicate.Parameters);
            }

            if (!string.IsNullOrEmpty(Status))
            {
                var statusPredicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.Equal(
                        Expression.Property(Expression.Parameter(typeof(Cv), "x"), nameof(Cv.Status)),
                        Expression.Constant(Enum.Parse<CVStatus>(Status))),
                    Expression.Parameter(typeof(Cv), "x"));
                predicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.AndAlso(predicate.Body, statusPredicate.Body),
                    predicate.Parameters);
            }

            if (UserId.HasValue)
            {
                var userIdPredicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.Equal(
                        Expression.Property(Expression.Parameter(typeof(Cv), "x"), nameof(Cv.UserId)),
                        Expression.Constant(UserId.Value)),
                    Expression.Parameter(typeof(Cv), "x"));
                predicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.AndAlso(predicate.Body, userIdPredicate.Body),
                    predicate.Parameters);
            }

            if (!string.IsNullOrEmpty(City))
            {
                var cityPredicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.Equal(
                        Expression.Property(Expression.Parameter(typeof(Cv), "x"), nameof(Cv.City)),
                        Expression.Constant(City)),
                    Expression.Parameter(typeof(Cv), "x"));
                predicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.AndAlso(predicate.Body, cityPredicate.Body),
                    predicate.Parameters);
            }

            if (!string.IsNullOrEmpty(JobLevel))
            {
                var jobLevelPredicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.Equal(
                        Expression.Property(Expression.Parameter(typeof(Cv), "x"), nameof(Cv.JobLevel)),
                        Expression.Constant(JobLevel)),
                    Expression.Parameter(typeof(Cv), "x"));
                predicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.AndAlso(predicate.Body, jobLevelPredicate.Body),
                    predicate.Parameters);
            }

            if (MinSalary.HasValue)
            {
                var minSalaryPredicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.GreaterThanOrEqual(
                        Expression.Property(Expression.Parameter(typeof(Cv), "x"), nameof(Cv.MinSalary)),
                        Expression.Constant(MinSalary.Value)),
                    Expression.Parameter(typeof(Cv), "x"));
                predicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.AndAlso(predicate.Body, minSalaryPredicate.Body),
                    predicate.Parameters);
            }

            if (MaxSalary.HasValue)
            {
                var maxSalaryPredicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.LessThanOrEqual(
                        Expression.Property(Expression.Parameter(typeof(Cv), "x"), nameof(Cv.MaxSalary)),
                        Expression.Constant(MaxSalary.Value)),
                    Expression.Parameter(typeof(Cv), "x"));
                predicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.AndAlso(predicate.Body, maxSalaryPredicate.Body),
                    predicate.Parameters);
            }

            if (IsDeal.HasValue)
            {
                var isDealPredicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.Equal(
                        Expression.Property(Expression.Parameter(typeof(Cv), "x"), nameof(Cv.IsDeal)),
                        Expression.Constant(IsDeal.Value)),
                    Expression.Parameter(typeof(Cv), "x"));
                predicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.AndAlso(predicate.Body, isDealPredicate.Body),
                    predicate.Parameters);
            }

            if (!string.IsNullOrEmpty(DesiredJob))
            {
                var desiredJobPredicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.Equal(
                        Expression.Property(Expression.Parameter(typeof(Cv), "x"), nameof(Cv.DesiredJob)),
                        Expression.Constant(DesiredJob)),
                    Expression.Parameter(typeof(Cv), "x"));
                predicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.AndAlso(predicate.Body, desiredJobPredicate.Body),
                    predicate.Parameters);
            }

            if (!string.IsNullOrEmpty(Position))
            {
                var positionPredicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.Equal(
                        Expression.Property(Expression.Parameter(typeof(Cv), "x"), nameof(Cv.Position)),
                        Expression.Constant(Position)),
                    Expression.Parameter(typeof(Cv), "x"));
                predicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.AndAlso(predicate.Body, positionPredicate.Body),
                    predicate.Parameters);
            }

            if (MajorId.HasValue)
            {
                var majorIdPredicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.Equal(
                        Expression.Property(Expression.Parameter(typeof(Cv), "x"), nameof(Cv.MajorId)),
                        Expression.Constant(MajorId.Value)),
                    Expression.Parameter(typeof(Cv), "x"));
                predicate = Expression.Lambda<Func<Cv, bool>>(
                    Expression.AndAlso(predicate.Body, majorIdPredicate.Body),
                    predicate.Parameters);
            }

            return predicate;
        }
    }
}