using Contract.Identity.UserManager;
using Core.Enum;
using Domain.Identity.Users;
using Domain.Positions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using SqlServ4r.EntityFramework;
using SqlServ4r.RepGenerationPatten;
using System.Linq.Dynamic.Core;
using Volo.Abp.DependencyInjection;

namespace SqlServ4r.Repository.Users
{
    public class UserRepository : GenericRepository<User, Guid>, ITransientDependency, IUserRepository
    {
        public UserRepository([NotNull] MeworkCoreContext context) : base(context)
        {
        }

        public (bool ExistEmail, bool ExistPhoneNumber, bool ExistUserCode) CheckDuplicateInformation(string email, string phone, string userCode)
        {

            var query = from user in _context.Users
                        select new ValueTuple<bool, bool, bool>(_context.Users.Any(x => x.Email == email && x.Email != null),
                            _context.Users.Any(x => x.PhoneNumber == phone),
                            _context.Users.Any(x => x.UserCode == userCode));

            return query.FirstOrDefault();
        }

        public (bool ExistEmail, bool ExistPhoneNumber, bool ExistUserCode) CheckDuplicateInformation(string email, string phone, string userCode, Guid id)
        {
            var query = from user in _context.Users
                        select new ValueTuple<bool, bool, bool>(_context.Users.Where(x => x.Id != id).Any(x => x.Email == email && x.Email != null),
                            _context.Users.Where(x => x.Id != id).Any(x => x.PhoneNumber == phone),
                            _context.Users.Where(x => x.Id != id).Any(x => x.UserCode == userCode));

            return query.FirstOrDefault();
        }

        public async Task<List<UserWithNavigationProperties>> GetListWithNavigationProperties()
        {
            var rolePatient = await _context.Roles.AsNoTracking().FirstOrDefaultAsync(s => s.Code == RoleClaimEnum.PATIENT.ToString());

            var query = from user in _context.Users.Where(x => !x.IsDelete)
                        where _context.UserRoles.Count(x => x.UserId == user.Id && x.RoleId == rolePatient.Id) == 0
                        select new UserWithNavigationProperties
                        {
                            User = user,
                            RoleNames = (from roleUser in _context.UserRoles
                                         join role in _context.Roles on roleUser.RoleId equals role.Id
                                         where roleUser.UserId == user.Id
                                         select role.Name).ToList(),
                            Position = _context.Positions.FirstOrDefault(x => x.Id == user.PositionId),
                            Departments = (from department in _context.Departments
                                           join departmentUser in _context.UserDepartments
                                           on department.Id equals departmentUser.DepartmentId
                                           where departmentUser.UserId == user.Id
                                           select department).ToList()
                        };


            return await query.ToListAsync();

        }

        public async Task<List<UserWithNavigationProperties>> GetUserCompanyListWithNavigationProperties(UserCompanyFilter filter)
        {

            var query = from user in _context.Users.Where(x => !x.IsDelete )

                        where !user.IsDelete 
                        && (!string.IsNullOrEmpty(filter.UserName) ? user.UserName.Trim().ToLower().Contains(filter.UserName.Trim().ToLower()) : 1 == 1)
                        && (!string.IsNullOrEmpty(filter.UserCode) ? user.UserCode.Trim().ToLower().Contains(filter.UserCode.Trim().ToLower()) : 1 == 1)
                        && (!string.IsNullOrEmpty(filter.FirstName) ? user.FirstName.Trim().ToLower().Contains(filter.FirstName.Trim().ToLower()) : 1 == 1)
                        && (!string.IsNullOrEmpty(filter.LastName) ? user.LastName.Trim().ToLower().Contains(filter.LastName.Trim().ToLower()) : 1 == 1)
                        select new UserWithNavigationProperties
                        {
                            User = user,
                            RoleNames = (from roleUser in _context.UserRoles
                                         join role in _context.Roles on roleUser.RoleId equals role.Id
                                         where roleUser.UserId == user.Id
                                         select role.Name).ToList(),
                        };


            if (filter != null && filter.Take > 0)
            {
                int count = query.Count();
                var data = await query.Skip(filter.Skip).Take(filter.Take).ToListAsync();
                if(count> 0)
                {
                    data.FirstOrDefault().Count = count;
                }
                return data;
            }

            return await query.ToListAsync();

        }


        public async Task<List<User>> GetListByRoles(UserFilterPagingModel? filter = null)
        {
            bool isAdminUser = (
                from roleUser in _context.UserRoles
                join role in _context.Roles on roleUser.RoleId equals role.Id
                where (
                    filter != null
                        ? !string.IsNullOrEmpty(filter.RoleName) ? roleUser.UserId == filter.CreatedBy && role.Name == RoleClaimEnum.ADMIN.ToString() : true
                        : true
                )
                select role.Name
            ).Any();
            var query = from user in _context.Users.Where(x => !x.IsDelete)
                        where (
                            from roleUser in _context.UserRoles
                            join role in _context.Roles on roleUser.RoleId equals role.Id
                            where (
                                filter != null
                                    ? !string.IsNullOrEmpty(filter.RoleName) ? roleUser.UserId == user.Id && role.Name == filter.RoleName : true
                                    : true
                            )
                            select role.Name
                        ).Any()
                        && (
                            filter != null && filter.CreatedBy != null
                                ? (
                                    user.CreatedBy == filter.CreatedBy
                                    || isAdminUser == true
                                )
                                : true
                        )
                         && (
                            filter != null && !string.IsNullOrEmpty(filter.FullName)
                                ? (
                                    (user.FirstName + " " + user.LastName).Contains(filter.FullName)
                                )
                                : true
                        )
                        && (
                            filter != null && !string.IsNullOrEmpty(filter.UserCode)
                                ? (
                                    (user.UserCode).Contains(filter.UserCode)
                                )
                                : true
                        )

                        && (
                            filter != null && filter.Gender != null
                                ? (
                                    (user.Gender).Equals(filter.Gender)
                                )
                                : true
                        )

                     
                        select user;


            List<User> result = new List<User>();
            if (filter != null && filter.Take > 0)
            {
                int count = query.Count();
                result =  await query.Skip(filter.Skip).Take(filter.Take).ToListAsync();
            }
            else
            {
                result = await query.ToListAsync();
            }

            return result;

        }
        public async Task<List<UserBasicInfoDto>> GetUserBasicInfoWithNavigationProperties(string? text)
        {

            var rolePatient = await _context.Roles.AsNoTracking().FirstOrDefaultAsync(s => s.Code == RoleClaimEnum.PATIENT.ToString());

            var query = from user in _context.Users.Where(x => !x.IsDelete && x.IsActive)
                    .WhereIf(!text.IsNullOrWhiteSpace(), x => x.FirstName.Contains(text) || x.LastName.Contains(text))
                        where _context.UserRoles.Count(x => x.UserId == user.Id && x.RoleId == rolePatient.Id) == 0
                        select new UserBasicInfoDto()
                        {
                            Id = user.Id,
                            FullName = user.FirstName + " " + user.LastName,
                            Gender = user.Gender,
                            DOB = user.DOB,
                            UserCode = user.UserCode,
                            PhoneNumber = user.PhoneNumber,
                            AvatarURL = user.AvatarURL,
                            Position = user.Position.Name,
                            Departments = (from department in _context.Departments
                                           join departmentUser in _context.UserDepartments
                                               on department.Id equals departmentUser.DepartmentId
                                           where departmentUser.UserId == user.Id
                                           select department).Select(x => x.Name).ToList()
                        };

            return await query.ToListAsync();
        }
        public async Task<UserWithNavigationProperties> GetWithNavigationProperties(Guid id)
        {
            var query = from user in _context.Users.Where(x => x.Id == id)
                        select new UserWithNavigationProperties
                        {
                            User = user,
                            RoleNames = (from roleUser in _context.UserRoles
                                         join role in _context.Roles on roleUser.RoleId equals role.Id
                                         where roleUser.UserId == user.Id
                                         select role.Name).ToList(),
                            Position = _context.Positions.FirstOrDefault(x => x.Id == user.PositionId) ?? new Position(),
                            Departments = (from department in _context.Departments
                                           join departmentUser in _context.UserDepartments
                                               on department.Id equals departmentUser.DepartmentId
                                           where departmentUser.UserId == user.Id
                                           select department).ToList()

                        };

            return await query.FirstOrDefaultAsync() ?? new UserWithNavigationProperties();
        }
    }
}