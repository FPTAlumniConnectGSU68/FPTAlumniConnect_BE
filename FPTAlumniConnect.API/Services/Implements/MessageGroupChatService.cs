using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.MessageGroupChat;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class MessageGroupChatService : BaseService<MessageGroupChatService>, IMessageGroupChatService
    {

        public MessageGroupChatService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<MessageGroupChatService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateMessageGroupChat(MessageGroupChatInfo request)
        {
            // Check MemberId
            User checkMemberId = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            predicate: s => s.UserId == request.MemberId);
            if (checkMemberId == null)
            {
                throw new BadHttpRequestException("MemberIdNotFound");
            }

            MessageGroupChat newMessage = _mapper.Map<MessageGroupChat>(request);
            await _unitOfWork.GetRepository<MessageGroupChat>().InsertAsync(newMessage);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newMessage.Id;
        }

        public async Task<MessageGroupChatInfo> GetMessageGroupChatById(int id)
        {
            MessageGroupChat messageGroupChat = await _unitOfWork.GetRepository<MessageGroupChat>()
                .SingleOrDefaultAsync(predicate: x => x.Id.Equals(id))
                ?? throw new BadHttpRequestException("MessageGroupChatNotFound");

            return _mapper.Map<MessageGroupChatInfo>(messageGroupChat);
        }

        public async Task<bool> UpdateMessageGroupChat(int id, MessageGroupChatInfo request)
        {
            MessageGroupChat messageToUpdate = await _unitOfWork.GetRepository<MessageGroupChat>()
                .SingleOrDefaultAsync(predicate: x => x.Id.Equals(id))
                ?? throw new BadHttpRequestException("MessageGroupChatNotFound");

            // Check MemberId
            User checkMemberId = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            predicate: s => s.UserId == request.MemberId);
            if (checkMemberId == null)
            {
                throw new BadHttpRequestException("MemberIdNotFound");
            }

            messageToUpdate.Content = string.IsNullOrEmpty(request.Content) ? messageToUpdate.Content : request.Content;
            messageToUpdate.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;
            messageToUpdate.UpdatedAt = TimeHelper.NowInVietnam();

            _unitOfWork.GetRepository<MessageGroupChat>().UpdateAsync(messageToUpdate);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<IPaginate<MessageGroupChatInfo>> ViewAllMessagesInGroupChat(MessageGroupChatFilter filter, PagingModel pagingModel)
        {
            var response = await _unitOfWork.GetRepository<MessageGroupChat>()
                .GetPagingListAsync(
                    selector: x => _mapper.Map<MessageGroupChatInfo>(x),
                    filter: filter,
                    orderBy: x => x.OrderBy(x => x.CreatedAt),
                    page: pagingModel.page,
                    size: pagingModel.size
                );
            return response;
        }
    }
}
