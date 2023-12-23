using AgoraIO.Media;
using Core.Enum;
using Microsoft.Extensions.Configuration;
using Volo.Abp.DependencyInjection;

namespace Application.AppConfigs
{
    public class AgoraRequest
    {
        public int Uid { get; set; } = 0;
        public RtcTokenBuilder2.Role Role { get; set; }
        public string ChannelName { get; set; }
    }

    public class AgoraResponse : AgoraRequest
    {
        public string Token { get; set; }
    }

    public interface IAgoraService
    {
        Task<AgoraResponse> CreateRtcToken(AgoraRequest setting);
    }

    public class AgoraService : ServiceBase, IAgoraService, ITransientDependency
    {
        private readonly IConfiguration _configuration;

        public AgoraService(IConfiguration configuration
           )
        {
            _configuration = configuration;
        }

        public Task<AgoraResponse> CreateRtcToken(AgoraRequest setting)
        {
            string appid = _configuration["AgoraAppSettings:AppId"];
            string appCertificate = _configuration["AgoraAppSettings:AppCertificate"];
            RtcTokenBuilder2 tokenBuilder = new RtcTokenBuilder2();

            string token = tokenBuilder.buildTokenWithUid(appid, appCertificate, setting.ChannelName, setting.Uid, setting.Role, 600, 600);

            AgoraResponse result = new AgoraResponse()
            {
                ChannelName = setting.ChannelName,
                Token = token,
                Role = setting.Role,
                Uid = setting.Uid,
            };
            return Task.FromResult(result);
        }
    }
}