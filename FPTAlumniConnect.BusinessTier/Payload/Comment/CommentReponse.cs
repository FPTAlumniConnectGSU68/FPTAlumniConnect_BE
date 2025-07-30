using AutoMapper;
using CommentModel = FPTAlumniConnect.DataTier.Models.Comment;


namespace FPTAlumniConnect.BusinessTier.Payload.Comment
{
    public class CommentReponse
    {
        public int CommentId { get; set; }

        public int? PostId { get; set; }

        public int? AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public string? AuthorAvatar { get; set; }
        public string Content { get; set; } = null!;

        public int? ParentCommentId { get; set; }

        public string? Type { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int ReplyCount => ChildComments.Count;

        // Nested child comments
        public List<CommentReponse> ChildComments { get; set; } = new();
    }

    public static class CommentTreeHelper
    {
        public static List<CommentReponse> BuildCommentTree(List<CommentModel> comments, IMapper mapper)
        {
            var dtoDict = comments
                .Select(c => mapper.Map<CommentReponse>(c))
                .ToDictionary(dto => dto.CommentId);

            List<CommentReponse> roots = new();

            foreach (var dto in dtoDict.Values)
            {
                if (dto.ParentCommentId.HasValue && dtoDict.ContainsKey(dto.ParentCommentId.Value))
                {
                    // Add to parent’s ChildComments
                    dtoDict[dto.ParentCommentId.Value].ChildComments.Add(dto);
                }
                else
                {
                    // It's a root comment
                    roots.Add(dto);
                }
            }

            return roots;
        }
    }
}
