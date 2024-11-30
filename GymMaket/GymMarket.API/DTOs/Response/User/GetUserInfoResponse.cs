using GymMarket.API.DTOs.User;

namespace GymMarket.API.DTOs.Response.User
{
    public class GetUserInfoResponse : ApiResponse
    {
        public GetUserInfoDto? UserInfo { get; set; }
    }
}
