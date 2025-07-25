using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.GroupChat;
using FPTAlumniConnect.BusinessTier.Payload.User;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class GroupChatService : BaseService<GroupChatService>, IGroupChatService
    {
        public GroupChatService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<GroupChatService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateGroupChat(GroupChatInfo request)
        {
            var newGroupChat = _mapper.Map<GroupChat>(request);
            await _unitOfWork.GetRepository<GroupChat>().InsertAsync(newGroupChat);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newGroupChat.Id;
        }
        public async Task<GroupChatReponse> GetGroupChatById(int id)
        {
            GroupChat groupChat = await _unitOfWork.GetRepository<GroupChat>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ?? throw new BadHttpRequestException("GroupChatNotFound");

            return _mapper.Map<GroupChatReponse>(groupChat);
        }

        public async Task<bool> UpdateGroupChat(int id, GroupChatInfo request)
        {
            GroupChat groupChat = await _unitOfWork.GetRepository<GroupChat>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ?? throw new BadHttpRequestException("GroupChatNotFound");

            groupChat.RoomName = string.IsNullOrEmpty(request.RoomName) ? groupChat.RoomName : request.RoomName;
            groupChat.UpdatedAt = DateTime.Now;
            groupChat.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;
            _unitOfWork.GetRepository<GroupChat>().UpdateAsync(groupChat);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return isSuccessful;
        }
        public async Task<IPaginate<GroupChatReponse>> ViewAllMessagesInGroupChat(GroupChatFilter filter, PagingModel pagingModel)
        {
            IPaginate<GroupChatReponse> response = await _unitOfWork.GetRepository<GroupChat>().GetPagingListAsync(
                selector: x => _mapper.Map<GroupChatReponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.Id),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }

        public async Task<bool> DeleteGroupChat(int id)
        {
            GroupChat groupChat = await _unitOfWork.GetRepository<GroupChat>().SingleOrDefaultAsync(
                predicate: x => x.Id == id) ?? throw new BadHttpRequestException("GroupChatNotFound");

            _unitOfWork.GetRepository<GroupChat>().DeleteAsync(groupChat);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return isSuccessful;
        }

        public async Task<bool> AddUserToGroup(int groupId, int userId)
        {
            var groupExists = await _unitOfWork.GetRepository<GroupChat>().AnyAsync(x => x.Id == groupId);
            var userExists = await _unitOfWork.GetRepository<User>().AnyAsync(x => x.UserId == userId);

            if (!groupExists) throw new BadHttpRequestException("GroupChatNotFound");
            if (!userExists) throw new BadHttpRequestException("UserNotFound");

            var isAlreadyInGroup = await _unitOfWork.GetRepository<GroupChatMember>().AnyAsync(
                x => x.GroupChatId == groupId && x.UserId == userId);

            if (isAlreadyInGroup) throw new BadHttpRequestException("UserAlreadyInGroup");

            var member = new GroupChatMember
            {
                GroupChatId = groupId,
                UserId = userId,
                //JoinedAt = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<GroupChatMember>().InsertAsync(member);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return isSuccessful;
        }

        public async Task<bool> LeaveGroup(int groupId, int userId)
        {
            var member = await _unitOfWork.GetRepository<GroupChatMember>().SingleOrDefaultAsync(
                predicate: x => x.GroupChatId == groupId && x.UserId == userId);

            if (member == null) throw new BadHttpRequestException("UserNotInGroup");

            _unitOfWork.GetRepository<GroupChatMember>().DeleteAsync(member);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return isSuccessful;
        }

        public async Task<List<GroupChatReponse>> GetGroupsByUserId(int userId)
        {
            var groupIds = await _unitOfWork.GetRepository<GroupChatMember>()
                .FindAllAsync(x => x.UserId == userId);

            var ids = groupIds.Select(x => x.GroupChatId).ToList();

            var groups = await _unitOfWork.GetRepository<GroupChat>()
                .GetListAsync(predicate: x => ids.Contains(x.Id));

            return groups.Select(_mapper.Map<GroupChatReponse>).ToList();
        }

        public async Task<List<UserResponse>> GetMembersInGroup(int groupId)
        {
            var members = await _unitOfWork.GetRepository<GroupChatMember>()
                .GetListAsync(predicate: x => x.GroupChatId == groupId, include: x => x.Include(y => y.User));

            return members.Select(x => _mapper.Map<UserResponse>(x.User)).ToList();
        }

        public async Task<List<GroupChatReponse>> SearchGroupsByName(string keyword)
        {
            var groups = await _unitOfWork.GetRepository<GroupChat>()
                .GetListAsync(predicate: x => x.RoomName.Contains(keyword));

            return groups.Select(_mapper.Map<GroupChatReponse>).ToList();
        }

    }
}
