using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contract.FileFolders;
using Domain.FileFolders;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using SqlServ4r.EntityFramework;
using SqlServ4r.RepGenerationPatten;
using Volo.Abp.DependencyInjection;

namespace SqlServ4r.Repository.FileFolders
{
    public class FileFolderRepository : GenericRepository<FileFolder, Guid>, ITransientDependency
    {
        public FileFolderRepository([NotNull] MeworkCoreContext context) : base(context)
        {
        }

        public async Task<List<FileFolderWithNavigationProperties>> GetMostPopularFolder(int top)
        {
            var foldersWithFile = await _context.FileFolders
                .Include(x => x.Files)
                .OrderByDescending(x=>x.Files.Count)
                .Take(top)
                .Select(x=> new FileFolderWithNavigationProperties()
                {
                    FileFolder = x,
                    FileCount = x.Files.Count(x=>!x.IsDeleted)
                })
                .ToListAsync();
            
            return foldersWithFile;
        }
    }
}