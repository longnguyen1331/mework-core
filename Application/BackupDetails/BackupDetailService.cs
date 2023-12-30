using Contract;
using Contract.BackupDetails;
using Domain.BackupDetails;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using SqlServ4r.Repository.BackupDetails;
using SqlServ4r.Repository.Backups;
using Volo.Abp.DependencyInjection;

namespace Application.BackupDetails
{
    public class BackupDetailService : ServiceBase, IBackupDetailService, ITransientDependency
    {
        private readonly BackupDetailRepository _backupDetailRepository;
        private readonly BackupRepository _backupRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;


        public BackupDetailService(IConfiguration configuration, BackupDetailRepository backupDetailRepository, BackupRepository backupRepository, IWebHostEnvironment environment)
        {
            _backupDetailRepository = backupDetailRepository;
            _configuration = configuration;
            _backupRepository = backupRepository;
            _environment = environment;
        }


        public async Task<ApiResponseBase<bool>> CreateAsync(CreateUpdateBackupDetailDto input)
        {
            ApiResponseBase<bool> result = new ApiResponseBase<bool>() { Data = false };
            try
            {
                var backup = await _backupRepository.FirstOrDefaultAsync(x => x.Id == input.BackupId);
                string folderPath = Path.Combine(_environment.WebRootPath, _configuration["Media:BACKUP_FOLDER"]);
                string destinationPath  = Path.Combine(folderPath, $"{DateTime.Now.ToFileTimeUtc}.bak");

                if (!File.Exists(folderPath))
                {
                     System.IO.Directory.CreateDirectory(folderPath);
                }

                if (backup == null || string.IsNullOrEmpty(backup.DbName) || string.IsNullOrEmpty(backup.Server) || string.IsNullOrEmpty(backup.UserName) || string.IsNullOrEmpty(backup.Password)) //check if a database name has been entered
                {
                    result.Message = "Cấu hình thông tin kết nối dữ liệu sai hoặc thiếu thông tin";
                }
                else
                {
                    //SqlConnection objconnection = new SqlConnection(backup.ConnectionString);
                    //ServerConnection con = new ServerConnection(objconnection.DataSource.ToString());
                    //Server server = new Server(con);
                    //Backup source = new Backup();
                    //source.Action = BackupActionType.Database;
                    //source.Database = backup.DbName;    
                    //BackupDeviceItem destination = new BackupDeviceItem($"backup_{DateTime.Now.ToFileTimeUtc}", DeviceType.File);
                    //source.Devices.Add(destination);
                    //source.SqlBackup(server);

                    Backup sqlBackup = new Backup();
                    //Specify the type of backup, the description, the name, and the database to be backed up.
                    sqlBackup.Action = BackupActionType.Database;
                    sqlBackup.BackupSetDescription = "BackUp of:" + backup.DbName + "on" + DateTime.Now.ToShortDateString();
                    sqlBackup.BackupSetName = "FullBackUp";
                    sqlBackup.Database = backup.DbName;

                    //Declare a BackupDeviceItem
                    BackupDeviceItem deviceItem = new BackupDeviceItem(destinationPath, DeviceType.File);
                    //Define Server connection
                    ServerConnection connection = new ServerConnection(backup.Server, backup.UserName, backup.Password);
                    //To Avoid TimeOut Exception
                    Server sqlServer = new Server(connection);
                    sqlServer.ConnectionContext.StatementTimeout = 60 * 60;
                    Database db = sqlServer.Databases[backup.DbName];

                    sqlBackup.Initialize = true;
                    sqlBackup.Checksum = true;
                    sqlBackup.ContinueAfterError = true;

                    //Add the device to the Backup object.
                    sqlBackup.Devices.Add(deviceItem);
                    //Set the Incremental property to False to specify that this is a full database backup.
                    sqlBackup.Incremental = false;

                    sqlBackup.ExpirationDate = DateTime.Now.AddDays(3);
                    //Specify that the log must be truncated after the backup is complete.
                    sqlBackup.LogTruncation = BackupTruncateLogType.Truncate;

                    sqlBackup.FormatMedia = false;
                    //Run SqlBackup to perform the full database backup on the instance of SQL Server.
                    sqlBackup.SqlBackup(sqlServer);
                    //Remove the backup device from the Backup object.
                    sqlBackup.Devices.Remove(deviceItem);

                    // thực hiện backup 
                    input.FullFilePath = destinationPath;
                    var backupDetai = ObjectMapper.Map<CreateUpdateBackupDetailDto, BackupDetail>(input);
                    await _backupDetailRepository.AddAsync(backupDetai);
                    result.Data = true;
                }
            }
            catch (Exception ex)
            {
                result.Data = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ApiResponseBase<BackupDetailsearchResponseDto>> GetListAsync(BackupDetailFilterPagingDto filter)
        {
            ApiResponseBase<BackupDetailsearchResponseDto> result = new ApiResponseBase<BackupDetailsearchResponseDto>();

            try
            {
                result.Data = new BackupDetailsearchResponseDto();

                var BackupDetails = _backupDetailRepository
                    .GetQueryable()
                    .Include(x => x.User)
                    .AsQueryable()
                    .Where(x => (filter.FromDate.HasValue ? x.CreatedDate >= filter.FromDate.Value : true)
                    && (filter.ToDate.HasValue ? x.CreatedDate <= filter.ToDate.Value : true));

                result.Data.TotalItem = BackupDetails.Count();

                if (filter.Take > 0)
                    BackupDetails = BackupDetails.OrderByDescending(x => x.CreatedDate).Skip(filter.Skip).Take(filter.Take);

                result.Data.Result = ObjectMapper.Map<List<BackupDetail>, List<BackupDetailDto>>(await BackupDetails.ToListAsync());
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }
    }
}