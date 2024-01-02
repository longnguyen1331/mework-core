using System;
using System.ComponentModel.DataAnnotations;
using Domain.StaticFiles;

namespace Domain.AppConfigs
{
    public class AppConfig
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        
        public bool EnableNotificationByEmail { get; set; }
        public string? MailHost { get; set; }
        public int MailPort { get; set; }
        public string? MailName { get; set; }
        public string? Password { get; set; }
        public string? SendingEmailContent { get; set; }
        
        public bool  Firebase { get; set; }
        public string? APIKey { get; set; }
        public string? AuthDomain { get; set; }
        public string? MessagingSenderId { get; set; }
        public string? APPID { get; set; }
        public string? ServerKey { get; set; }
        
        
        public bool EnableJobAssignment { get; set; }
        public bool  IsApply { get; set; }
        
        
        //media
        public Guid?   LogoId { get; set; }
        public Guid?   IconId { get; set; }
        
        
        public StaticFile? LogoFile { get; set; }
        public StaticFile? IconFile { get; set; }

        //Bổ sung 
        public string? SeoTitle { get; set; }
        public string? SeoKeyword { get; set; }
        [MaxLength(250)]
        public string? SeoDescription { get; set; }
        [MaxLength(150)]
        public string? PrimaryColor { get; set; }
        [MaxLength(150)]
        public string? SecondaryColor { get; set; }
        //Code bổ sung trên header cho web user nếu cần
        [MaxLength(450)]
        public string? HeaderCode { get; set; }
        [MaxLength(150)]
        public string? Phone { get; set; }
        [MaxLength(150)]
        public string? Email { get; set; }
        [MaxLength(250)]
        public string? Address { get; set; }
        //Google login
        [MaxLength(150)]
        public string? GoogleAnalytic { get; set; }
        [MaxLength(150)]
        public string? GoogleAppName { get; set; }
        [MaxLength(150)]
        public string? GoogleAPIKey { get; set; }
        [MaxLength(150)]
        public string? GoogleClientId { get; set; }
        [MaxLength(150)]
        public string? GoogleClientSecret { get; set; }
        public bool GoogleEnable { get; set; }

        // Facebook login
        [MaxLength(150)]
        public string? FacebookAppName { get; set; }
        [MaxLength(150)]
        public string? FacebookAPIKey { get; set; }
        [MaxLength(150)]
        public string? FacebookPixel { get; set; }
        [MaxLength(150)]
        public string? FacebookAppSecret { get; set; }
        public bool FacebookEnable { get; set; }


        //TỌA ĐỘ
        [MaxLength(150)]
        public string? Latitude { get; set; }
        [MaxLength(150)]
        public string? Longitude { get; set; }
        //HTML KHÁC
        public string? Terms { get; set; }
        public string? Conditions { get; set; }
        public string? Introduction { get; set; }
    }
}