using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contract.Common.Excels;
using Contract.Identity.UserManager;
using Contract.Uploads;
using Core.Const;
using Core.Enum;
using Core.Exceptions;
using Domain.Departments;
using Domain.Identity.Roles;
using Domain.Identity.Users;
using Domain.Positions;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Application.Helpers;
using Microsoft.AspNetCore.Identity;

namespace Application.Identity.UserManager
{
    public partial class UserManagerService
    {
        public async Task<UserValidatorExcel> CreateUsersFromCSVFileAndDefineRoles(FileDto file)
        {
            if (!File.Exists(file.Path))
            {
                throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            }
            var userValidator = new UserValidatorExcel();
            FileInfo existingFile = new FileInfo(file.Path);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                if (worksheet.Dimension is null)
                {
                    throw new GlobalException(HttpMessage.EmptyContent, HttpStatusCode.BadRequest);
                }

                int rowCount = worksheet.Dimension.End.Row;

                var userRows = new List<UserExcelDto>();


                for (int row = 2; row <= rowCount; row++)
                {
                    var item = ReadUserRowFromExcel(row, worksheet);
                    if (!IsNullExcelRow(item)) break;
                    userRows.Add(item);
                    userValidator.InvalidCells.AddRange(ValidateUserRow(item));
                }

                if (userValidator.InvalidCells.Count < 1)
                {
                    CheckUniqueFieldsInUsersExcel(userRows, userValidator);
                    if (userValidator.InvalidLogics.Count < 1)
                    {
                        var roles = await _roleManager.Roles.ToListAsync();
                        var positions = await _positionRepository.ToListAsync();
                        var departments = await _departmentRepository.ToListAsync();
                        var users = await _userRepository.ToListAsync();

                        await CheckCodesExistsInDb(userRows, userValidator, roles, positions, departments);

                        if (userValidator.InvalidLogics.Count < 1)
                        {
                            var newUsers = new List<CreateUpdateUseDto>();
                            var updateUsers = new List<CreateUpdateUseDto>();
                            (newUsers, updateUsers) = await DifferentiateCreatingOrUpdatingForUser(userRows, userValidator, users);

                            if (userValidator.InvalidLogics.Count < 1)
                            {
                                foreach (var item in newUsers)
                                {
                                    var newUser = ObjectMapper.Map<CreateUpdateUseDto, CreateUserDto>(item);

                                    if (!item.RoleCodes.IsNullOrEmpty())
                                    {
                                        foreach (var code in item.RoleCodes)
                                        {
                                            newUser.Roles.Add(roles.FirstOrDefault(x => x.Code == code).Name);
                                        }
                                    }

                                    if (!item.PositionCode.IsNullOrEmpty())
                                    {
                                        newUser.PositionId = positions.
                                            FirstOrDefault(x => x.Code == item.PositionCode)?.Id;
                                    }

                                    if (!item.DepartmentCodes.IsNullOrEmpty())
                                    {
                                        foreach (var code in item.DepartmentCodes)
                                        {
                                            newUser.DepartmentIds.Add(departments.FirstOrDefault(x => x.Code == code).Id);
                                        }
                                    }
                                    //COMPANY
                                    await CreateUserWithNavigationPropertiesAsync(newUser);
                                }

                                foreach (var item in updateUsers)
                                {
                                    var updateUser = ObjectMapper.Map<CreateUpdateUseDto, UpdateUserDto>(item);
                                    var user = users.FirstOrDefault(x => x.PhoneNumber == item.PhoneNumber);

                                    if (!item.RoleCodes.IsNullOrEmpty())
                                    {
                                        foreach (var code in item.RoleCodes)
                                        {
                                            updateUser.Roles.Add(roles.FirstOrDefault(x => x.Code == code).Name);
                                        }
                                    }

                                    if (!item.PositionCode.IsNullOrEmpty())
                                    {
                                        updateUser.PositionId = positions.
                                            FirstOrDefault(x => x.Code == item.PositionCode)?.Id;
                                    }

                                    if (!item.DepartmentCodes.IsNullOrEmpty())
                                    {
                                        foreach (var code in item.DepartmentCodes)
                                        {
                                            updateUser.DepartmentIds.Add(departments.FirstOrDefault(x => x.Code == code).Id);
                                        }
                                    }

                                    await UpdateUserWithNavigationPropertiesAsync(updateUser, user.Id);
                                }

                                userValidator.IsSuccessful = true;
                            }
                        }
                    }
                }



                return userValidator;
            }

        }

        private async Task<(List<CreateUpdateUseDto> NewUser, List<CreateUpdateUseDto> UpdateUser)>
            DifferentiateCreatingOrUpdatingForUser(List<UserExcelDto> userRows, UserValidatorExcel? userValidator, List<User> users)
        {
            var newUsers = new List<CreateUpdateUseDto>();
            var updateUsers = new List<CreateUpdateUseDto>();

            foreach (var item in userRows)
            {
                var createUpdateUser = new CreateUpdateUseDto();
                createUpdateUser.UserCode = item.UserCode;
                createUpdateUser.FirstName = item.FirstName;
                createUpdateUser.LastName = item.LastName;
                createUpdateUser.Gender = item.Gender.IsNullOrWhiteSpace()
                    ? Gender.Unknown
                    : (Gender)Enum.Parse(typeof(Gender), item.Gender);
                createUpdateUser.DOB = item.DOB.IsNullOrWhiteSpace() ? null : DateTime.ParseExact(item.DOB, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None);
                createUpdateUser.Password = item.Password;
                createUpdateUser.PhoneNumber = item.PhoneNumber;
                createUpdateUser.Email = item.Email;
                createUpdateUser.RoleCodes = item.RoleCodes ?? new List<string>();
                createUpdateUser.DepartmentCodes = item.DepartmentCodes ?? new List<string>();
                createUpdateUser.PositionCode = item.PositionCode;
                createUpdateUser.UserName = item.UserName;
                createUpdateUser.IsActive = Convert.ToBoolean(item.IsActive);
                createUpdateUser.CompanyId = item.CompanyId.ToString();

                if (users.Any(x => x.PhoneNumber == item.PhoneNumber))
                {
                    var user = users.FirstOrDefault(x => x.PhoneNumber == item.PhoneNumber);

                    if (users.Any(x => !item.Email.IsNullOrEmpty() && x.Email == item.Email && x.Id != user.Id))
                    {
                        userValidator.InvalidLogics.Add($"Email :{item.Email} Exists In DB");
                    }

                    if (users.Any(x => x.UserCode == item.UserCode && x.Id != user.Id))
                    {
                        userValidator.InvalidLogics.Add(
                            $"User Code :{item.UserCode} Exists In DB");
                    }

                    updateUsers.Add(createUpdateUser);
                    continue;
                }

                if (users.Any(x => !item.Email.IsNullOrEmpty() && x.Email == item.Email))
                {
                    userValidator.InvalidLogics.Add($"Email :{item.Email} Exists In DB");
                }

                if (users.Any(x => x.UserCode == item.UserCode))
                {
                    userValidator.InvalidLogics.Add($"UserCode :{item.UserCode} Exists In DB");
                }

                newUsers.Add(createUpdateUser);
            }

            return (newUsers, updateUsers);
        }

        private async Task CheckCodesExistsInDb(List<UserExcelDto>? validUsers, UserValidatorExcel? userValidator,
            List<Role> roles, List<Position> positions, List<Department> departments)
        {


            foreach (var item in validUsers.SelectMany(x => x.RoleCodes)?.Distinct())
            {
                if (!roles.Any(x => x.Code == item))
                {
                    userValidator.InvalidLogics.Add($"Code Role :not exits {item} In Excel File");
                }
            }


            foreach (var item in validUsers.Where(x => x.PositionCode != null).
                Select(x => x.PositionCode).Distinct())
            {
                if (!positions.Any(x => x.Code == item))
                {
                    userValidator.InvalidLogics.Add($"Position Code :not exits {item} In Excel File");
                }
            }


            foreach (var item in validUsers.SelectMany(x => x.DepartmentCodes).Distinct())
            {
                var department = departments.FirstOrDefault(x => x.Code == item);
                if (department is null)
                {
                    userValidator.InvalidLogics.Add($"Department Code :not exits {item} In Excel File");
                }
                else
                {
                    var childDepartmentCount = departments.
                        Count(x => x.ParentCode == department.Id);
                    if (childDepartmentCount > 0)
                    {
                        userValidator.InvalidLogics.Add($"Department Code :{item} is not child node");
                    }
                }
            }
        }

        private static void CheckUniqueFieldsInUsersExcel(List<UserExcelDto>? userRows,
            UserValidatorExcel? userValidator)
        {
            var duplicatedEmails = userRows.GroupBy(x => x.Email)
                .Where(group => group.Key != null && @group.Count() > 1)
                .Select(x => x.Key);
            var duplicatedCodes = userRows.GroupBy(x => x.UserCode)
                .Where(group => @group.Count() > 1)
                .Select(x => x.Key);
            ;
            var duplicatedPhones = userRows.GroupBy(x => x.PhoneNumber)
                .Where(group => @group.Count() > 1)
                .Select(x => x.Key);
            ;

            foreach (var item in duplicatedEmails)
            {
                userValidator.InvalidLogics.Add($"Duplicated Email :{item} In Excel File");
            }

            foreach (var item in duplicatedCodes)
            {
                userValidator.InvalidLogics.Add($"Duplicated Code :{item} In Excel File");
            }

            foreach (var item in duplicatedPhones)
            {
                userValidator.InvalidLogics.Add($"Duplicated Phone :{item} In Excel File");
            }
        }


        private UserExcelDto ReadUserRowFromExcel(int row, ExcelWorksheet worksheet)
        {
            var user = new UserExcelDto()
            {
                UserCode = worksheet.Cells[row, 1].Value?.ToString().Trim(),
                FirstName = worksheet.Cells[row, 2].Value?.ToString().Trim(),
                LastName = worksheet.Cells[row, 3].Value?.ToString().Trim(),
                Gender = worksheet.Cells[row, 4].Value?.ToString().Trim(),
                DOB = worksheet.Cells[row, 5].Value?.ToString().Trim(),
                Password = worksheet.Cells[row, 6].Value?.ToString().Trim(),
                UserName = worksheet.Cells[row, 7].Value?.ToString().Trim(),
                PhoneNumber = worksheet.Cells[row, 7].Value?.ToString().Trim(),
                Email = worksheet.Cells[row, 8].Value?.ToString().Trim(),
                RoleCodes = worksheet.Cells[row, 9].Value?.ToString().Trim().Split(',').ToList() ?? new List<string>(),
                DepartmentCodes = worksheet.Cells[row, 10].Value?.ToString().Trim().Split(',').ToList() ?? new List<string>(),
                PositionCode = worksheet.Cells[row, 11].Value?.ToString(),
                IsActive = worksheet.Cells[row, 12].Value?.ToString(),
                CompanyId = worksheet.Cells[row, 13].Value?.ToString(),
                Row = row
            };

            return user;
        }

        private bool IsNullExcelRow(UserExcelDto user)
        {
            if (user.UserCode.IsNullOrWhiteSpace()
                & user.Password.IsNullOrWhiteSpace()
                & user.PhoneNumber.IsNullOrWhiteSpace()
                & user.Email.IsNullOrWhiteSpace())
            {
                return false;
            }

            return true;
        }
        private List<Cell> ValidateUserRow(UserExcelDto userExcelDto)
        {
            UserValidator validator = new UserValidator();


            var invalidCells = new List<Cell>();


            if (!validator.ValidateUserCode(userExcelDto.UserCode))
            {
                invalidCells.Add(new Cell()
                {
                    Row = userExcelDto.Row,
                    Col = 1,
                });
            }


            if (!validator.ValidateName(userExcelDto.FirstName))
            {
                invalidCells.Add(new Cell()
                {
                    Row = userExcelDto.Row,
                    Col = 2,
                });
            }

            if (!validator.ValidateName(userExcelDto.LastName))
            {
                invalidCells.Add(new Cell()
                {
                    Row = userExcelDto.Row,
                    Col = 3,
                });
            }


            if (!validator.ValidateGender(userExcelDto.Gender))
            {
                invalidCells.Add(new Cell()
                {
                    Row = userExcelDto.Row,
                    Col = 4,
                });
            }


            if (!validator.ValidateDate(userExcelDto.DOB))
            {
                invalidCells.Add(new Cell()
                {
                    Row = userExcelDto.Row,
                    Col = 5,
                });
            }

            if (!validator.ValidatePassword(userExcelDto.Password))
            {
                invalidCells.Add(new Cell()
                {
                    Row = userExcelDto.Row,
                    Col = 6,
                });
            }

            if (!validator.ValidatePhone(userExcelDto.PhoneNumber))
            {
                invalidCells.Add(new Cell()
                {
                    Row = userExcelDto.Row,
                    Col = 7,
                });
            }

            if (!validator.ValidateEmail(userExcelDto.Email))
            {
                invalidCells.Add(new Cell()
                {
                    Row = userExcelDto.Row,
                    Col = 8,
                });
            }

            if (!validator.ValidateBool(userExcelDto.IsActive))
            {
                invalidCells.Add(new Cell()
                {
                    Row = userExcelDto.Row,
                    Col = 11,
                });
            }
            return invalidCells;
        }


    }
}