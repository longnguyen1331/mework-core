using System;

namespace Contract.Base
{
    public class FilterBase
    {
        public string? Text { get; set; }
        public DateTime? StartDay { get; set;}
        public DateTime? EndDay { get; set; }
    }
}