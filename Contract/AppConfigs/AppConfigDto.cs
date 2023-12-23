using System;
using System.ComponentModel.DataAnnotations;
using Domain.StaticFiles;

namespace Contract.AppConfigs
{
    public class AppConfigDto
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public string Code { get; set; }
        
        public bool EnableNotificationByEmail { get; set; }
        public string? MailHost { get; set; }
        public int MailPort { get; set; }
        public string? MailName { get; set; }
        public string? SendingEmailContent { get; set; }
        public string? Password { get; set; }
        
        public bool  Firebase { get; set; }
        public string? APIKey { get; set; }
        public string? AuthDomain { get; set; }
        public string? MessagingSenderId { get; set; }
        public string? APPID { get; set; }
        public string? ServerKey { get; set; }
        
        
        public bool EnableJobAssignment { get; set; }
        public bool  IsApply { get; set; }
        
        public Guid?   LogoId { get; set; }
        public Guid?   IconId { get; set; }
        
        
        //media
        public string?   LogoURL { get; set; }
        public string?   IconURL { get; set; }



        //Bổ sung 
        public string? SeoTitle { get; set; }
        public string? SeoKeyword { get; set; }
        public string? SeoDescription { get; set; }
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        //Code bổ sung trên header cho web user nếu cần
        public string? HeaderCode { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        //Google login
        public string? GoogleAnalytic { get; set; }
        public string? GoogleAppName { get; set; }
        public string? GoogleAPIKey { get; set; }
        public string? GoogleClientId { get; set; }
        public string? GoogleClientSecret { get; set; }
        public bool GoogleEnable { get; set; }

        // Facebook login
        public string? FacebookAppName { get; set; }
        public string? FacebookAPIKey { get; set; }
        public string? FacebookPixel { get; set; }
        public string? FacebookAppSecret { get; set; }
        public bool FacebookEnable { get; set; }


        //TỌA ĐỘ
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        //HTML KHÁC
        public string? Terms { get; set; }
        public string? Conditions { get; set; }
        public string? Introduction { get; set; }

    }
}