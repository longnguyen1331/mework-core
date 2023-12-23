using System;

namespace Contract.AppHistories
{
    public class CreateUpdateAppHistoryDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime Date { set; get; }
        public string IpAddress { get; set; }
        public string Functions { get; set; }
        public string Operation { get; set; }
    }
}