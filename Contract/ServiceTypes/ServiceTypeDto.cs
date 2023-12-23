using System;
using System.Collections.Generic;

namespace Contract.ServiceTypes
{
    public class ServiceTypeDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }
        public int ODX { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsHighLight { get; set; }
        public bool IsVisibled { get; set; }
        public Guid? ImageId { get; set; }
        public string Slug { get; set; }
    }
}