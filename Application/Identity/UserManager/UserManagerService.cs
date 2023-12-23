using Contract;
using Contract.Identity.UserManager;
using Core.Const;
using Core.Enum;
using Core.Exceptions;
using Domain.Identity.Roles;
using Domain.Identity.Users;
using Domain.UserDepartments;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SqlServ4r.Repository.Departments;
using SqlServ4r.Repository.Positions;
using SqlServ4r.Repository.RoleClaims;
using SqlServ4r.Repository.UserDepartments;
using SqlServ4r.Repository.UserRoles;
using SqlServ4r.Repository.Users;
using SqlServ4r.Repository.UserTokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Volo.Abp.DependencyInjection;

namespace Application.Identity.UserManager
{
    public partial class UserManagerService : ServiceBase, IUserManagerService, ITransientDependency
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly RoleClaimRepository _roleClaimRepository;
        private readonly IConfiguration _configuration;
        private readonly UserRoleRepository _userRoleRepository;
        private readonly UserTokenRepository _userTokenRepository;
        private readonly UserRepository _userRepository;
        private readonly UserDepartmentRepository _userDepartmentRepository;
        private readonly PositionRepository _positionRepository;
        private readonly DepartmentRepository _departmentRepository;

        public UserManagerService(UserManager<User> userManager,
            RoleManager<Role> roleManager,
            RoleClaimRepository roleClaimRepository,
            UserRoleRepository userRoleRepository,
            UserTokenRepository userTokenRepository,
            UserRepository userRepository,
            UserDepartmentRepository userDepartmentRepository,
            PositionRepository positionRepository,
            DepartmentRepository departmentRepository,
            
            IConfiguration configuration
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _roleClaimRepository = roleClaimRepository;
            _userRoleRepository = userRoleRepository;
            _userTokenRepository = userTokenRepository;
            _userRepository = userRepository;
            _userDepartmentRepository = userDepartmentRepository;
            _positionRepository = positionRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<List<UserWithNavigationPropertiesDto>> GetListWithNavigationAsync()
        {
            var users = await _userRepository.GetListWithNavigationProperties();
            return ObjectMapper.Map<List<UserWithNavigationProperties>, List<UserWithNavigationPropertiesDto>>(users);
        }
        public async Task<List<UserWithNavigationPropertiesDto>> GetUserCompanyListWithNavigationAsync(UserCompanyFilter filter)
        {
            var users = await _userRepository.GetUserCompanyListWithNavigationProperties(filter);
            return ObjectMapper.Map<List<UserWithNavigationProperties>, List<UserWithNavigationPropertiesDto>>(users);
        }


        public async Task<UserWithNavigationPropertiesDto> GetWithNavigationProperties(Guid id)
        {
            return ObjectMapper.Map<UserWithNavigationProperties, UserWithNavigationPropertiesDto>(await _userRepository.GetWithNavigationProperties(id));
        }

        public async Task<UserDto> CreateUserWithNavigationPropertiesAsync(CreateUserDto input)
        {
            var existedUser = await _userManager.FindByNameAsync(input.UserName);

            if (existedUser != null)
            {
                throw new GlobalException("Đã tồn tại tài khoản trên hệ thống. Vui lòng liên hệ với quản trị viên dể kích hoạt tài khoản.", HttpStatusCode.BadRequest);
            }

            var dto = await CreateAsync(input);
            await UpdateRolesForUser(input.UserName, input.Roles);
            var userDepartments = new List<UserDepartment>();
            foreach (var item in input.DepartmentIds)
            {
                userDepartments.Add(new UserDepartment() { UserId = dto.Id, DepartmentId = item });
            }
            await _userDepartmentRepository.AddRangeAsync(userDepartments);
            return dto;
        }


        public async Task<UserDto> UpdateUserWithNavigationPropertiesAsync(UpdateUserDto input, Guid id)
        {
            var item = await _userManager.FindByIdAsync(id.ToString());

            if (item == null)
            {
                throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            }

            if (item.UserName != input.UserName)
            {
                var existedUser = await _userManager.FindByNameAsync(input.UserName);

                if (existedUser != null)
                {
                    throw new GlobalException("Đã tồn tại tài khoản trên hệ thống. Vui lòng liên hệ với quản trị viên dể kích hoạt tài khoản.", HttpStatusCode.BadRequest);
                }
            }

            var user = ObjectMapper.Map(input, item);
            user.CreatedBy = item.CreatedBy;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new GlobalException(result.Errors?.FirstOrDefault().Description, HttpStatusCode.BadRequest);
            }


            if (input.IsSetPassword)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var reuslt = await _userManager.ResetPasswordAsync(user, token, input.Password);
                if (!reuslt.Succeeded)
                {
                    throw new GlobalException(reuslt.Errors?.FirstOrDefault().Description, HttpStatusCode.BadRequest);
                }
            }

            await UpdateRolesForUser(user.UserName, input.Roles);
            await _updateUserDepartmentForUser(user.Id, input.DepartmentIds);
            return ObjectMapper.Map<User, UserDto>(item);
        }

        public async Task<List<UserIdentityDto>> GetBasicUserInfosAsync()
        {
            var users = await _userRepository.GetListAsync(x => !x.IsDelete && x.IsActive);
            return ObjectMapper.Map<List<User>, List<UserIdentityDto>>(users);
        }


        public async Task DeleteWithNavigationAsync(Guid id)
        {
            var item = await _userManager.FindByIdAsync(id.ToString());
            if (item == null) throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);


            item.IsActive = false;
            var userResult = await _userManager.UpdateAsync(item);
            if (!userResult.Succeeded)
            {
                throw new GlobalException(HttpMessage.CheckInformation, HttpStatusCode.BadRequest);
            }
        }

        public async Task<List<UserDto>> GetListAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            return ObjectMapper.Map<List<User>, List<UserDto>>(users);
        }


        public async Task<UserDto> CreateAsync(CreateUserDto input)
        {
            try
            {
                (input.PhoneNumber, input.UserCode, input.Email) =
                TrimText(input.PhoneNumber, input.UserCode, input.Email);
                if (input.AvatarURL.IsNullOrWhiteSpace())
                {
                    input.AvatarURL = _configuration["Media:DEFAULT_Avatar_URL"];
                }
                var user = ObjectMapper.Map<CreateUserDto, User>(input);

                var result = await _userManager.CreateAsync(user, input.Password);
                if (!result.Succeeded)
                {
                    throw new GlobalException(result.Errors?.FirstOrDefault().Description, HttpStatusCode.BadRequest);
                }

                return ObjectMapper.Map<User, UserDto>(user);

            }
            catch (Exception ex)
            {
                return null;
            }

        }



        public async Task<UserDto> UpdateAsync(UpdateUserDto input, Guid id)
        {

            (input.PhoneNumber, input.UserCode, input.Email) =
                TrimText(input.PhoneNumber, input.UserCode, input.Email);

            var item = await _userManager.FindByIdAsync(id.ToString());

            if (item == null)
            {
                throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            }

            var user = ObjectMapper.Map(input, item);
            user.CreatedBy = input.CreatedBy == null || input.CreatedBy == Guid.Empty ? item.CreatedBy : input.CreatedBy;
            user.ModifiedBy = input.ModifiedBy == null || input.ModifiedBy == Guid.Empty ? item.ModifiedBy : input.ModifiedBy;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new GlobalException(result.Errors?.FirstOrDefault().Description, HttpStatusCode.BadRequest);
            }

            return ObjectMapper.Map<User, UserDto>(user);
            ;
        }

        private (string Phone, string Code, string? Email) TrimText(string phone, string code, string email)
        {
            return (phone.Trim(), code.Trim(), email?.Trim());
        }



        public async Task DeleteAsync(Guid id)
        {
            var item = await _userManager.FindByIdAsync(id.ToString());
            if (item == null)
            {
                throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            }
            item.IsDelete = true;
            await _userRepository.UpdateAsync(item);
        }

        public async Task<TokenDto> SignInAsync(UserModel input)
        {
            var user = await _userManager.FindByNameAsync(input.UserName);
            if (user == null || !user.IsActive || user.IsDelete)
            {
                throw new GlobalException(HttpMessage.CheckInformation, HttpStatusCode.BadRequest);
            }


            var result = await _userManager.CheckPasswordAsync(user, input.Password);
            if (!result)
            {
                throw new GlobalException(HttpMessage.CheckInformation, HttpStatusCode.BadRequest);
            }

            string accessToken = await GenerateTokenByUser(user);
            string refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken; await _userManager.UpdateAsync(user);

            await _userManager.SetAuthenticationTokenAsync(user, input.LoginProvider ?? "Website", input.DeviceId ?? "AccessToken", input.Token);

            return new TokenDto() { UserId = user.Id.ToString(), AccessToken = accessToken, RefreshToken = refreshToken };
        }


        public async Task<ApiResponseBase<bool>> CreateTokenFireBase(TokenFribaseModel token)
        {
            ApiResponseBase<bool> result = new ApiResponseBase<bool>();
            try
            {
                if (token is null || token.AccessToken is null) throw new GlobalException(HttpMessage.Unauthorized, HttpStatusCode.Unauthorized);
                var principal = GetPrincipalFromExpiredToken(token.AccessToken.Replace("\"", ""));
                var userName = principal.Identity.Name;

                var user = await _userManager.FindByNameAsync(userName);

                await _userManager.SetAuthenticationTokenAsync(user, token.LoginProvider ?? "Website", token.DeviceId ?? "AccessToken", token.FireBaseToken);
                result.Data = true;
                return result;
            }
            catch(Exception ex) {
                result.Data = false;
                result.Message = ex.Message;
                return result;
            }
        }

        public async Task<UserDto> SignUpAsync(CreateUserDto input)
        {
            var user = ObjectMapper.Map<CreateUserDto, User>(input);

            var result = await _userManager.CreateAsync(user, input.Password);
            if (!result.Succeeded)
            {
                throw new GlobalException(result.Errors.FirstOrDefault().Description, HttpStatusCode.BadRequest);
            }

            return ObjectMapper.Map<User, UserDto>(user);
        }

        public async Task<UserDto> UpdateProfile(UpdateUserProfileRequestDto input)
        {
            var item = await _userManager.FindByIdAsync(input.Id.ToString());
            if (item == null) throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            var user = ObjectMapper.Map(input, item);

            await _userRepository.UpdateAsync(item);
            return ObjectMapper.Map<User, UserDto>(item);
        }

        public async Task<bool> SetNewPasswordAsync(NewUserPasswordDto input)
        {
            var user = await _userManager.FindByNameAsync(input.UserName);
            if (user == null)
            {
                throw new GlobalException(HttpMessage.CheckInformation, HttpStatusCode.BadRequest);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, input.NewPassword);
            if (!result.Succeeded)
            {
                throw new GlobalException(result.Errors.FirstOrDefault().Description, HttpStatusCode.BadRequest);
            }

            return true;
        }

        public async Task UpdateRolesForUser(string userName, List<string> roles)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var oldRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, oldRoles);
            await _userManager.AddToRolesAsync(user, roles);
        }

        private async Task _updateUserDepartmentForUser(Guid userId, List<Guid> newDepartmentId)
        {
            var oldDepartmentUsers = await _userDepartmentRepository
                .GetListAsync(x => x.UserId == userId);
            _userDepartmentRepository.RemoveRange(oldDepartmentUsers);

            var newUserDepartments = new List<UserDepartment>();
            foreach (var item in newDepartmentId)
            {
                newUserDepartments.Add(new UserDepartment() { UserId = userId, DepartmentId = item });
            }

            await _userDepartmentRepository.AddRangeAsync(newUserDepartments);
        }

        public Task<UserProfileModel> UpdateUserProfileAsync(UserProfileModel userProfileModel)
        {
            throw new NotImplementedException();
        }


        public async Task<TokenDto> RefreshTokenAsync(TokenModel token)
        {

            if (token is null) throw new GlobalException(HttpMessage.Unauthorized, HttpStatusCode.Unauthorized);


            var principal = GetPrincipalFromExpiredToken(token.AccessToken);
            var userName = principal.Identity.Name;

            var user = await _userManager.FindByNameAsync(userName);

            if (user == null || user.RefreshToken != token.RefreshToken)
                throw new GlobalException(HttpMessage.Unauthorized, HttpStatusCode.Unauthorized);

            var refreshToken = GenerateRefreshToken();
            var accessToken = await GenerateTokenByUser(user);
            user.RefreshToken = refreshToken;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new GlobalException(HttpMessage.Conflict, HttpStatusCode.TooManyRequests);
            }

            return new TokenDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task Logout(UserLogOutModel input)
        {
            var userToken = await _userTokenRepository.FirstOrDefaultAsync(x => x.UserId == input.UserId && x.Name == input.DeviceId);
            var user = await _userManager.FindByIdAsync(input.UserId.ToString());

            if (userToken != null)
            {
                user.RefreshToken = null;
                await _userManager.UpdateAsync(user);

                await _userManager.RemoveAuthenticationTokenAsync(user, userToken.LoginProvider , userToken.Name);
            }
        }

        public async Task LogoutWebsite(WebisteUserLogOutModel token)
        {
            var principal = GetPrincipalFromExpiredToken(token.AccessToken.Replace("\"", ""));
            var userName = principal.Identity.Name;
            var user = await _userManager.FindByNameAsync(userName);
            var userToken = await _userTokenRepository.FirstOrDefaultAsync(x => x.UserId == user.Id && x.Name == token.DeviceId);
            if (userToken != null)
            {
                user.RefreshToken = null;
                await _userManager.UpdateAsync(user);
                await _userManager.RemoveAuthenticationTokenAsync(user, userToken.LoginProvider, userToken.Name);
            }
        }

        private async Task<string> GenerateTokenByUser(User user)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var userRoles = await _userManager.GetRolesAsync(user);
            var userRole = await _userRoleRepository.GetListAsync(x => x.UserId == user.Id);

            var userRoleClaims = _roleClaimRepository.GetRoleClaimsByRoles(userRole.Select(x => x.RoleId).ToList());
            var userClaims = userRoleClaims.Select(x => new Claim(x.ClaimType, x.ClaimValue));

            List<Claim> claims = new List<Claim>();

            claims.AddRange(userClaims);
            foreach (var item in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }

            claims.Add(new Claim(ClaimTypes.PrimarySid, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(ClaimTypes.Surname, user.FirstName + " " + user.LastName));
            claims.Add(new Claim("Code", user.UserCode));

            var tokeOptions = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(168),
                signingCredentials: signinCredentials
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

            return tokenString;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience =
                    false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        public async Task<List<UserDto>> GetListByRoles(UserFilterPagingModel? filter = null)
        {
            var users = await _userRepository.GetListByRoles(filter);

            return ObjectMapper.Map<List<User>, List<UserDto>>(users);
        }


        public async Task<ApiResponseBase<List<UserDto>>> GetListByFilterAsync(UserFilterPagingModel filter)
        {
            ApiResponseBase<List<UserDto>> result = new ApiResponseBase<List<UserDto>>();
            try
            {
                List<UserDto> users = new List<UserDto>();
                //var userRoles = _userRoleRepository.GetQueryable().Where(x=>x.)

                users = await (
                               from u in _userRepository.GetQueryable().Where(x => !x.IsDelete)
                               join u_r in _userRoleRepository.GetQueryable() on u.Id equals u_r.UserId into u_rs
                               from urss in u_rs.DefaultIfEmpty()
                               where (filter.Gender.HasValue ? u.Gender == filter.Gender : 1 == 1)
                                && (filter.RoleIds != null && filter.RoleIds.Count > 0 ? filter.RoleIds.Any(r => r == urss.RoleId)  : 1 == 1)
                                && (filter.DobFrom.HasValue ? u.DOB >= filter.DobFrom : 1 == 1)
                                && (filter.DobTo.HasValue ? u.DOB <= filter.DobTo : 1 == 1)
                                && (!string.IsNullOrEmpty(filter.FilterText) ?
                                    (u.Email.Contains(filter.FilterText)) ||
                                    (u.PhoneNumber.Contains(filter.FilterText)) ||
                                    (u.UserCode.Contains(filter.FilterText)) ||
                                    (u.FirstName + " " + u.LastName).Contains(filter.FilterText) : 1 == 1)
                               orderby u.CreatedDate descending
                               select new UserDto
                               {
                                   Id = u.Id,
                                   UserCode = u.UserCode,
                                   FirstName = u.FirstName,
                                   LastName = u.LastName,
                                   FullName = $"{u.FirstName} {u.LastName}",
                                   DOB = u.DOB,
                                   PhoneNumber = u.PhoneNumber,
                                   Gender = u.Gender,
                                   Email = u.Email,
                               }
                             ).Skip(filter.Skip).Take(filter.Take).ToListAsync();
                result.Data = users;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        private async Task<List<Guid>> GetChildrenDeparment(Guid parentId)
        {
            var departmentIds = new List<Guid>();
            departmentIds.Add(parentId);

            var items = await _departmentRepository.GetListAsync(x => x.ParentCode == parentId);

            foreach (var f in items)
            {
                departmentIds.AddRange(await GetChildrenDeparment(f.Id));
            }

            return departmentIds;
        }
    }
}