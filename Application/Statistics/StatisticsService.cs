using SqlServ4r.Repository.FileFolders;
using Volo.Abp.DependencyInjection;

namespace Application.Statistics
{
    public class StatisticsService : ServiceBase,ITransientDependency
    {
        private readonly FileFolderRepository _fileFolderRepository;
        public StatisticsService(
            FileFolderRepository fileFolderRepository
           
        )
        {
            _fileFolderRepository = fileFolderRepository;
          
        }

    }
}