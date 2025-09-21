using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Schedule;
using FPTAlumniConnect.BusinessTier.Payload.User;
using FPTAlumniConnect.BusinessTier.Utils;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class UserService : BaseService<UserService>, IUserService
    {
        //private readonly IFirebaseService _firebaseService;
        private readonly IConfiguration _configuration;
        public UserService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<UserService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            //_firebaseService = firebaseService;
            _configuration = configuration;
        }

        public async Task<int> CreateNewUser(UserInfo request)
        {
            User newUser = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.Email.Equals(request.Email)
                );

            if (newUser != null) throw new BadHttpRequestException("UserExisted");

            newUser = _mapper.Map<User>(request);

            await _unitOfWork.GetRepository<User>().InsertAsync(newUser);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newUser.UserId;
        }

        public async Task<GetUserResponse> GetUserById(int id)
        {
            Func<IQueryable<User>, IIncludableQueryable<User, object>> include = q => q.Include(u => u.Role).Include(u => u.Major);
            User user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.UserId.Equals(id), include: include) ??
                throw new BadHttpRequestException("UserNotFound");

            GetUserResponse result = _mapper.Map<GetUserResponse>(user);
            return result;
        }
        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            // Predicate to filter by email and password
            Expression<Func<User, bool>> searchFilter = p =>
                p.Email.Equals(loginRequest.Email) &&
                p.PasswordHash.Equals(loginRequest.Password);

            // Include related Role entity while fetching User
            Func<IQueryable<User>, IIncludableQueryable<User, object>> include = q => q.Include(u => u.Role);

            // Fetch User with related Role
            User user = await _unitOfWork.GetRepository<User>()
                .SingleOrDefaultAsync(predicate: searchFilter, include: include);

            if (user == null)
            {
                return new LoginResponse
                {
                    Message = "Invalid email or password",
                    AccessToken = null,
                    UserInfo = null
                };
            }

            // Generate the JWT token
            var token = JwtUtil.GenerateJwtToken(user);

            // Prepare the login response
            LoginResponse loginResponse = new LoginResponse()
            {
                Message = "Login success",
                AccessToken = token,
                UserInfo = new UserResponse()
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    RoleId = user.RoleId,  // Get RoleId directly from User
                    RoleName = user.Role?.Name ?? "No Role Assigned",  // Safely access Role.Name
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    MentorStatus = user.MentorStatus
                }
            };

            return loginResponse;
        }

        //public async Task<LoginResponse> LoginUser(LoginFirebaseRequest request)
        //{
        //    var cred = await _firebaseService.VerifyIdToken(request.Token);
        //    if (cred == null) throw new BadHttpRequestException("Firebase token không hợp lệ");
        //    var firebaseClaims = cred.Claims;

        //    var email = firebaseClaims.FirstOrDefault(c => c.Key == "email").Value.ToString();
        //    var uid = firebaseClaims.FirstOrDefault(c => c.Key == "user_id").Value.ToString();

        //    User userLogin = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
        //        predicate: x => x.Email.Equals(email)
        //        );

        //    string accessToken;
        //    if (userLogin != null)
        //    {
        //        userLogin.GoogleId = uid;
        //        _unitOfWork.GetRepository<User>().UpdateAsync(userLogin);
        //        accessToken = JwtUtil.GenerateJwtToken(userLogin);
        //        await _unitOfWork.CommitAsync();
        //    }
        //    else throw new BadHttpRequestException("UserNotFound");

        //    return new LoginResponse
        //    {
        //        Message = "Login success",
        //        AccessToken = accessToken,
        //        UserInfo = new UserResponse()
        //        {
        //            UserId = userLogin.UserId,
        //            FirstName = userLogin.FirstName,
        //            LastName = userLogin.LastName,
        //            Email = userLogin.Email,
        //            GoogleId = userLogin.GoogleId,
        //        }
        //    };
        //}
        public async Task<RegisterResponse> Register(RegisterRequest request)
        {
            // Check if email is already registered
            var existingUser = await _unitOfWork.GetRepository<User>()
                .SingleOrDefaultAsync(
                    selector: x => new { x.UserId, x.Email },
                    predicate: x => x.Email.Equals(request.Email)
                );

            if (existingUser != null)
            {
                throw new BadHttpRequestException("Email already registered.");
            }

            // Map RegisterRequest to User entity
            var newUser = _mapper.Map<User>(request);

            // Ensure Code is unique if provided
            if (!string.IsNullOrEmpty(request.Code))
            {
                var existingCode = await _unitOfWork.GetRepository<User>()
                    .SingleOrDefaultAsync(
                        selector: x => new { x.UserId, x.Code },
                        predicate: x => x.Code == request.Code
                    );

                if (existingCode != null)
                {
                    throw new BadHttpRequestException("Code already in use. Please choose another.");
                }

                newUser.Code = request.Code;
            }

            // Secure password hashing (Assume HashPassword is a helper method)
            newUser.PasswordHash = request.Password; // Replace with actual hashing method
            newUser.CreatedAt = TimeHelper.NowInVietnam();
            newUser.CreatedBy = "System";

            // Insert new user into the database
            await _unitOfWork.GetRepository<User>().InsertAsync(newUser);

            // Commit transaction
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccessful)
            {
                throw new BadHttpRequestException("Registration failed. Please try again.");
            }

            // Create and return the response object
            var response = new RegisterResponse
            {
                UserId = newUser.UserId,
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Code = newUser.Code,
                CreatedAt = newUser.CreatedAt.Value
            };

            return response;
        }


        public async Task<bool> UpdateUserInfo(int id, UserInfo request)
        {
            // Fetch the user based on the provided ID.
            User user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.UserId.Equals(id)) ??
                throw new BadHttpRequestException("UserNotFound");

            // Check if the logged-in user is an admin or staff
            //var loggedInUserRole = _httpContextAccessor.HttpContext?.User.FindFirst("Role")?.Value; // Assuming Role is stored as a claim
            //if (loggedInUserRole != null && (loggedInUserRole == "admin" || loggedInUserRole == "staff"))
            //{
            //    user.IsMentor = request.IsMentor;

            //}

            // Update other fields
            user.Code = string.IsNullOrEmpty(request.Code) ? user.Code : request.Code;
            user.FirstName = string.IsNullOrEmpty(request.FirstName) ? user.FirstName : request.FirstName;
            user.Email = string.IsNullOrEmpty(request.Email) ? user.Email : request.Email;
            user.LastName = string.IsNullOrEmpty(request.LastName) ? user.LastName : request.LastName;
            user.ProfilePicture = string.IsNullOrEmpty(request.ProfilePicture) ? user.ProfilePicture : request.ProfilePicture;
            user.MentorStatus = request.MentorStatus;
            user.UpdatedAt = TimeHelper.NowInVietnam();
            user.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            // Perform the update in the repository
            _unitOfWork.GetRepository<User>().UpdateAsync(user);

            // Commit the changes
            bool isSuccesful = await _unitOfWork.CommitAsync() > 0;

            return isSuccesful;
        }

        public async Task<bool> UpdateUserMentorStatus(int id, string isMentor)
        {
            _logger.LogInformation("Updating mentor status for user ID: {UserId}, IsMentor: {IsMentor}", id, isMentor);

            if (id <= 0)
                throw new BadHttpRequestException("Invalid user ID. User ID must be greater than 0.");

            var user = await _unitOfWork.GetRepository<User>()
                .SingleOrDefaultAsync(predicate: x => x.UserId == id)
                ?? throw new BadHttpRequestException("User not found.");

            // Validate IsMentor value (e.g., allow "true", "false", or null)
            if (isMentor != "Pending" && isMentor != "Active" && isMentor != "Inactive")
                throw new BadHttpRequestException("Invalid mentor status. Must be 'Active', 'Inactive', or 'Pending'.");

            // Update IsMentor field
            user.MentorStatus = isMentor;
            user.UpdatedAt = TimeHelper.NowInVietnam();
            user.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<User>().UpdateAsync(user);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<IPaginate<GetUserResponse>> ViewAllUser(UserFilter filter, PagingModel pagingModel)
        {
            // Define include to include the Role entity
            Func<IQueryable<User>, IIncludableQueryable<User, object>> include = q => q.Include(u => u.Role).Include(u => u.Major);

            // Call GetPagingListAsync with a selector, include, and other parameters
            IPaginate<GetUserResponse> response = await _unitOfWork.GetRepository<User>().GetPagingListAsync(
                selector: x => _mapper.Map<GetUserResponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(u => u.Email),
                include: include,
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }

        public async Task<IPaginate<GetMentorResponse>> ViewAllMentor(MentorFilter filter, PagingModel pagingModel)
        {
            Func<IQueryable<User>, IIncludableQueryable<User, object>> include = q => q.Include(u => u.Role).Include(u => u.Major);
            IPaginate<GetMentorResponse> mentorList = await _unitOfWork.GetRepository<User>().GetPagingListAsync(
                selector: x => _mapper.Map<GetMentorResponse>(x),
                predicate: x => x.MentorStatus.Equals("true"),
                filter: filter,
                include: include,
                page: pagingModel.page,
                size: pagingModel.size
            );
            foreach (var mentor in mentorList.Items)
            {
                mentor.Rating = await GetAverageRatingByMentorId(mentor.UserId);
            }
            return mentorList;
        }

        public async Task<double> GetAverageRatingByMentorId(int id)
        {
            ICollection<ScheduleReponse> schedules = await _unitOfWork.GetRepository<Schedule>().GetListAsync(
                selector: x => _mapper.Map<ScheduleReponse>(x),
                predicate: x => x.MentorId.Equals(id)) ??
                  throw new BadHttpRequestException("MentorNotFound");
            double? averageRating = schedules.Average(s => s.Rating);

            return Math.Round(averageRating ?? 0,2);
        }

        public async Task<int> CountAllUsers()
        {
            ICollection<GetUserResponse> users = await _unitOfWork.GetRepository<User>().GetListAsync(
                selector: x => _mapper.Map<GetUserResponse>(x));
                int count = users.Count();
                return count;
        }

        //public async Task<CountByMonthResponse> CountUsersByMonth(int month, int year)
        //{
        //    ICollection<GetUserResponse> users = await _unitOfWork.GetRepository<User>().GetListAsync(
        //        selector: x => _mapper.Map<GetUserResponse>(x),
        //            predicate: x => x.CreatedAt.HasValue
        //            && x.CreatedAt.Value.Year == year
        //            && x.CreatedAt.Value.Month == month);
        //    CountByMonthResponse count = new CountByMonthResponse
        //    {
        //        Month = month,
        //        Year = year,
        //        Count = users.Count()
        //    };
        //    return count;
        //}
        public async Task<ICollection<CountByMonthResponse>> CountUsersByMonth(int? month, int? year)
        {
            int targetYear = (year == null || year == 0) ? TimeHelper.NowInVietnam().Year : year.Value;
            int startMonth = (month.HasValue && month > 0 && month <= 12) ? month.Value : 1;
            int endMonth = (targetYear == TimeHelper.NowInVietnam().Year) ? TimeHelper.NowInVietnam().Month : 12;
            var result = new List<CountByMonthResponse>();
            for (int m = startMonth; m <= endMonth; m++)
            {
                var users = await _unitOfWork.GetRepository<User>().GetListAsync(
                    selector: x => _mapper.Map<GetUserResponse>(x),
                    predicate: x => x.CreatedAt.HasValue
                                    && x.CreatedAt.Value.Year == targetYear
                                    && x.CreatedAt.Value.Month == m
                );
                result.Add(new CountByMonthResponse
                {
                    Month = m,
                    Year = targetYear,
                    Count = users.Count()
                });
            }
            return result;
        }

        public async Task<ICollection<CountByRoleResponse>> CountUsersByRole(int? month, int? year, int role)
        {
            int targetYear = (year == null || year == 0) ? TimeHelper.NowInVietnam().Year : year.Value;
            int startMonth = (month.HasValue && month > 0 && month <= 12) ? month.Value : 1;
            int endMonth = (targetYear == TimeHelper.NowInVietnam().Year) ? TimeHelper.NowInVietnam().Month : 12;
            var result = new List<CountByRoleResponse>();
            for (int m = startMonth; m <= endMonth; m++)
            {
                var users = await _unitOfWork.GetRepository<User>().GetListAsync(
                    selector: x => _mapper.Map<GetUserResponse>(x),
                    predicate: x => x.CreatedAt.HasValue
                                    && x.CreatedAt.Value.Year == targetYear
                                    && x.CreatedAt.Value.Month == m
                                    && x.RoleId == role
                );
                result.Add(new CountByRoleResponse
                {
                    Month = m,
                    Year = targetYear,
                    Role = role,
                    Count = users.Count()
                });
            }
            return result;
        }

        public async Task<GoogleUserResponse> VerifyGoogleTokenAsync(string token)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(token, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _configuration["Google:ClientId"] } // Your Google Client ID
                });


                // Return user information from the payload
                return new GoogleUserResponse
                {
                    Email = payload.Email,
                    UserId = payload.Subject,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName
                };
            }
            catch (InvalidJwtException)
            {
                return null; // Invalid token
            }
        }
        public async Task<LoginResponse> LoginWithGoogle(LoginGoogleRequest request)
        {
            var googleUser = await VerifyGoogleTokenAsync(request.Token);

            if (googleUser == null)
                throw new BadHttpRequestException("Invalid Google OAuth token");

            var email = googleUser.Email;
            var uid = googleUser.UserId;

            var existingUser = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.Email.Equals(email)
            );

            string accessToken;
            User currentUser;

            if (existingUser != null)
            {
                existingUser.GoogleId = uid;
                _unitOfWork.GetRepository<User>().UpdateAsync(existingUser);
                accessToken = JwtUtil.GenerateJwtToken(existingUser);
                await _unitOfWork.CommitAsync();
                currentUser = existingUser;
            }
            else
            {
                var newUser = new User
                {
                    Email = email,
                    GoogleId = uid,
                    FirstName = googleUser.FirstName,
                    LastName = googleUser.LastName,
                    CreatedAt = TimeHelper.NowInVietnam(),
                    PasswordHash = "Google",
                    CreatedBy = "System",
                    RoleId = 2
                };

                await _unitOfWork.GetRepository<User>().InsertAsync(newUser);
                bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

                if (!isSuccessful)
                {
                    throw new BadHttpRequestException("Registration failed. Please try again.");
                }

                accessToken = JwtUtil.GenerateJwtToken(newUser);
                currentUser = newUser; // Assign newUser to currentUser
            }

            // Return the login response with the access token and user info
            return new LoginResponse
            {
                Message = "Login success",
                AccessToken = accessToken,
                UserInfo = new UserResponse
                {
                    UserId = currentUser.UserId,
                    FirstName = currentUser.FirstName,
                    LastName = currentUser.LastName,
                    Email = email,
                    GoogleId = uid
                }
            };
        }

        public async Task<CreateRecruiterResponse> CreateRecruiter(CreateRecruiterRequest request)
        {
            _logger.LogInformation("Creating new recruiter with email: {Email}", request.Email);

            // Check if email already exists
            var existingUser = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.Email.Equals(request.Email)
            );
            if (existingUser != null)
                throw new BadHttpRequestException($"Email {request.Email} already registered.");

            try
            {
                // Create new User
                var newUser = _mapper.Map<User>(request);
                newUser.PasswordHash = request.Password; // Replace with actual hashing method
                newUser.RoleId = 4; // Set RoleId to 4 (Recruiter)
                newUser.CreatedAt = TimeHelper.NowInVietnam();
                newUser.CreatedBy = "System";
                newUser.EmailVerified = false; // Default value for non-nullable field

                await _unitOfWork.GetRepository<User>().InsertAsync(newUser);
                await _unitOfWork.CommitAsync();
                // Create RecruiterInfo
                var recruiterInfo = _mapper.Map<RecruiterInfo>(request);
                recruiterInfo.UserId = newUser.UserId;
                recruiterInfo.CreatedAt = TimeHelper.NowInVietnam();

                await _unitOfWork.GetRepository<RecruiterInfo>().InsertAsync(recruiterInfo);

                // Commit all changes (single transaction handled by DbContext)
                bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
                if (!isSuccessful)
                    throw new BadHttpRequestException($"Failed to create recruiter with email {request.Email}.");

                _logger.LogInformation("Successfully created recruiter with user ID: {UserId} and recruiter info ID: {RecruiterInfoId}",
                    newUser.UserId, recruiterInfo.RecruiterInfoId);

                // Return response
                return new CreateRecruiterResponse
                {
                    UserId = newUser.UserId,
                    Email = newUser.Email,
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    RecruiterInfoId = recruiterInfo.RecruiterInfoId,
                    CompanyName = recruiterInfo.CompanyName,
                    CreatedAt = newUser.CreatedAt.Value
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating recruiter for email: {Email}", request.Email);
                throw;
            }
        }
    }
}