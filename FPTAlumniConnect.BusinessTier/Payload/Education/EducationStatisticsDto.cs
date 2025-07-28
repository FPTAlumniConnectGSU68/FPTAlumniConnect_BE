namespace FPTAlumniConnect.BusinessTier.Payload.Education
{
    public class EducationStatisticsDto
    {
        public string? GroupByField { get; set; }  // Tên nhóm như: SchoolName, Major, Location...
        public string? SubGroupByField { get; set; }  // Dùng nếu cần thống kê phụ: ví dụ Major trong mỗi School
        public int TotalCount { get; set; }        // Tổng số bản ghi thuộc nhóm đó
        public int? Year { get; set; }             // Dành cho thống kê theo năm (nếu có)
    }
}