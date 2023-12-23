using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Contract.DocumentFiles;
using Domain.DocumentFiles;
using Domain.FileFolders;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using SqlServ4r.EntityFramework;
using SqlServ4r.RepGenerationPatten;
using Volo.Abp.DependencyInjection;

namespace SqlServ4r.Repository.FileDocuments
{
    public class DocumentFileRepository : GenericRepository<DocumentFile, Guid>, IDocumentFileRepository,
        ITransientDependency
    {
        public DocumentFileRepository([NotNull] MeworkCoreContext context) : base(context)
        {
        }

        public async Task<List<DocumentFileWithNavProperties>> GetFilesWithNavProperties(DocumentFileFilter filter)
        {

            var folderIds = new List<Guid?>();
            if (filter.DocumentFolderId.HasValue)
            {
                var result =await GetChildFolderIdAsync(filter.DocumentFolderId.Value);
                folderIds = result.Cast<Guid?>().ToList(); 
                folderIds.Add(filter.DocumentFolderId.Value);
               
            }
            
            var query = from file in _context.DocumentFiles.Where(x => 
                        !x.IsDeleted).OrderByDescending(x=>x.CreationDate)
                    .WhereIf(filter.StartDay.HasValue && filter.EndDay.HasValue
                        , x => x.CreationDate >= filter.StartDay && x.CreationDate <= filter.EndDay)
                    .WhereIf(folderIds.Count > 0 , x=>folderIds.Contains(x.DocumentFolderId))
                    .WhereIf(!filter.Text.IsNullOrWhiteSpace(),
                        x => x.Name.Contains(filter.Text)|| x.Code.Contains(filter.Text))
                    .WhereIf(filter.FileTypeId.HasValue, x => x.DocumentTypeId == filter.FileTypeId)
                select new DocumentFileWithNavProperties
                {
                    File = file,
                    DocumentType = file.DocumentType
                };


            return await query.ToListAsync();
        }

        
      

        
        private async Task<List<Guid>> GetChildFolderIdAsync(Guid parentID)
        {
            var folders = await _context.FileFolders.AsNoTracking().ToListAsync();
            var childIds = new List<Guid>();
            childIds = HandleRecursiveChildFolderId(parentID,childIds,folders);
            return childIds;
        }
        
        
        
        
        private  List<Guid> HandleRecursiveChildFolderId(Guid parentId,List<Guid> ChildFolderIds,List<FileFolder> folders)
        {
           var childIds = folders.Where(x => x.ParentCode == parentId)
               .Select(x=>x.Id);
           ChildFolderIds.AddRange(childIds);
           foreach (var item in childIds)
           {
               HandleRecursiveChildFolderId(item,ChildFolderIds,folders);
           }

           return ChildFolderIds;
        }
    }
}