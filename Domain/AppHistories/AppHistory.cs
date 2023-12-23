using Domain.AppConfigs;
using Domain.Identity.Users;
using System.ComponentModel.DataAnnotations;

namespace Domain.AppHistories
{
    public class AppHistory 
    {
        public AppHistory() 
        { 
            Date = DateTime.Now;
        }
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime Date { set; get; }
        [MaxLength(255)]
        public string IpAddress { get; set; }
        public string Functions { get; set; }
        public string Operation { get; set; }
        public User User { get; set; }
    }
}