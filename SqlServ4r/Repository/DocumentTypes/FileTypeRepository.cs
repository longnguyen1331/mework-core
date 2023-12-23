using System;
using Domain.DocumentTypes;
using JetBrains.Annotations;
using SqlServ4r.EntityFramework;
using SqlServ4r.RepGenerationPatten;
using Volo.Abp.DependencyInjection;

namespace SqlServ4r.Repository.DocumentTypes
{
    public class DocumentTypeRepository : GenericRepository<DocumentType, Guid>, ITransientDependency
    {
        public DocumentTypeRepository([NotNull] MeworkCoreContext context) : base(context)
        {
        }
    }
}