using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FPTAlumniConnect.BusinessTier.Constants
{
    public static class ApiEndPointConstant
    {

        public const string RootEndPoint = "/api";
        public const string ApiVersion = "/v1";
        public const string ApiEndpoint = RootEndPoint + ApiVersion;

        public static class Authentication
        {
            public const string AuthenticationEndpoint = ApiEndpoint + "/auth";
            public const string Login = AuthenticationEndpoint + "/login";
            public const string Register = AuthenticationEndpoint + "/register";
            public const string GoogleLogin = AuthenticationEndpoint + "/login-google";
        }
        public static class User
        {
            public const string UsersEndPoint = ApiEndpoint + "/users";
            public const string UserEndPoint = UsersEndPoint + "/{id}";
            public const string UserLoginEndPoint = UsersEndPoint + "/login";
        }
        public static class Post
        {
            public const string PostsEndPoint = ApiEndpoint + "/posts";
            public const string PostEndPoint = PostsEndPoint + "/{id}";
        }
        public static class PostReport
        {
            public const string PostReportsEndPoint = ApiEndpoint + "/reports";
            public const string PostReportEndPoint = PostReportsEndPoint + "/{id}";
        }
        public static class Comment
        {
            public const string CommentsEndPoint = ApiEndpoint + "/comments";
            public const string CommentEndPoint = CommentsEndPoint + "/{id}";
        }
        public static class Mentorship
        {
            public const string MentorshipsEndPoint = ApiEndpoint + "/mentorships";
            public const string MentorshipEndPoint = MentorshipsEndPoint + "/{id}";
            public const string MentorshipAlumniIdEndPoint = MentorshipsEndPoint + "/alumni/{id}";
            public const string MentorshipStatisticsEndPoint = MentorshipsEndPoint + "/statistics";
            public const string MentorshipAutoCancelEndPoint = MentorshipsEndPoint + "/autocancel";
        }
        public static class Schedule
        {
            public const string SchedulesEndPoint = ApiEndpoint + "/schedules";
            public const string ScheduleEndPoint = SchedulesEndPoint + "/{id}";
        }
        public static class CV
        {
            public const string CVsEndPoint = ApiEndpoint + "/cvs";
            public const string CVEndPoint = CVsEndPoint + "/{id}";
            public const string CVUserEndPoint = CVsEndPoint + "/user/{id}";
        }
        public static class Skill
        {
            public const string SkillsEndPoint = ApiEndpoint + "/skills";
            public const string SkillEndPoint = SkillsEndPoint + "/{id}";
            public const string SkillCVEndPoint = SkillsEndPoint + "/cv/{id}";
        }
        public static class Tag
        {
            public const string TagsEndPoint = ApiEndpoint + "/tags";
            public const string TagEndPoint = TagsEndPoint + "/{id}";
            public const string TagCVEndPoint = TagsEndPoint + "/cv/{id}";
        }
        public static class Event
        {
            public const string EventsEndPoint = ApiEndpoint + "/events";
            public const string EventEndPoint = EventsEndPoint + "/{id}";
        }
        public static class UserJoinEvent
        {
            public const string UserJoinEventsEndPoint = ApiEndpoint + "/user-join-events";
            public const string UserJoinEventEndPoint = UserJoinEventsEndPoint + "/{id}";
            public const string ViewAllUserJoinEventsEndPoint = UserJoinEventsEndPoint + "/view-all";
        }
        public static class EducationHistory
        {
            public const string EducationHistoryEndPoint = "education-history/{id}";
            public const string EducationHistoriesEndPoint = "education-histories";
        }
        public static class PrivacySetting
        {
            public const string PrivacySettingEndPoint = "privacy-setting/{id}";
            public const string PrivacySettingsEndPoint = "privacy-settings";
        }
        public static class NotificationSetting
        {
            public const string NotificationSettingEndPoint = "notification-setting/{id}";
            public const string NotificationSettingsEndPoint = "notification-settings";
        }
        public static class MajorCode
        {
            public const string MajorCodesEndPoint = ApiEndpoint + "/majorcodes";
            public const string MajorCodeEndPoint = MajorCodesEndPoint + "/{id}";
            public const string CountEndpoint = MajorCodesEndPoint + "/counts";
            public const string NamesEndpoint = MajorCodesEndPoint + "/names";
        }
        public static class SpMajorCode
        {
            public const string SpMajorCodeEndPoint = ApiEndpoint + "/spmajorcodes";
            public const string SpMajorCodesEndPoint = SpMajorCodeEndPoint + "/{id}";
        }
        public static class JobPost
        {
            public const string JobPostsEndPoint = ApiEndpoint + "/jobposts";
            public const string JobPostEndPoint = JobPostsEndPoint + "/{id}";
        }
        public static class JobApplication
        {
            public const string JobApplicationsEndPoint = ApiEndpoint + "/jobapplications";
            public const string JobApplicationEndPoint = JobApplicationsEndPoint + "/{id}";
        }
        public static class SocialLink
        {
            public const string SocialLinkEndPoint = "social-link/{id}";
            public const string SocialLinksEndPoint = "social-links";
        }
        public static class MessageGroupChat
        {
            public const string MessagesEndPoint = ApiEndpoint + "/messages-group-chat";
            public const string MessageEndPoint = MessagesEndPoint + "/{id}";
        }
        public static class GroupChat
        {
            public const string GroupChatsEndPoint = ApiEndpoint + "/groupchats";
            public const string GroupChatEndPoint = GroupChatsEndPoint + "/{id}";
        }
        public static class PhoBert
        {
            public const string PhoBertEndpoint = ApiEndpoint + "/phobert";
            public const string FindBestMatchingCVEndpoint = PhoBertEndpoint + "/find-best-matching-cv";
        }
        public static class Education
        {
            public const string EducationsEndPoint = ApiEndpoint + "/educations";
            public const string EducationEndPoint = EducationsEndPoint + "/{id}";
            public const string EducationUserEndPoint = EducationsEndPoint + "/user/{id}";
        }

        public static class WorkExperience
        {
            public const string WorkExperiencesEndPoint = ApiEndpoint + "/work-experiences";
            public const string WorkExperienceEndPoint = WorkExperiencesEndPoint + "/{id}";
            public const string WorkExperienceUserEndPoint = WorkExperiencesEndPoint + "/user/{id}";
        }
        public static class TimeLine
        {
            public const string TimeLinesEndPoint = "timelines";
            public const string TimeLineEndPoint = "timelines/{id}";
        }
    }
}