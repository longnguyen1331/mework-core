using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BlazorDateRangePicker;
using Blazorise;
using Contract;
using Core.Enum;
using Core.Extension;
using FluentDateTimeOffset;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Radzen;
using Radzen.Blazor;
using WebClient.Exceptions;
using WebClient.Identity;
using WebClient.LanguageResources;
using WebClient.RequestHttp;
using WebClient.Setting;

namespace WebClient.Shared
{

    public abstract  class BaseBlazorPage : ComponentBase,IDisposable
    {
        
        [Inject] private NavigationManager _navigationManager { get; set;}
        [Inject] private NotificationService _notificationService { get; set;}

        public   IEnumerable<int> PageSizeOptions = new int[] { 5,10, 20, 30 };

        [CascadingParameter]
        public Task<AuthenticationState> AuthState { get; set; }
        
        [CascadingParameter]
        public JsonStringLocalizer  L { get; set; } 
        protected IMapper ObjectMapper { get;}
        public bool IsDisable  { get; set; }

        public BaseBlazorPage()
        {
            
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            ObjectMapper = config.CreateMapper();
            
        }
        

        public async Task<bool> InvokeAsync(Func<Task> action, ActionType type, bool showNotification = false)
        {

            try
            {
                IsDisable = true;
                await action();

                if (showNotification)
                {
                    switch (type)
                    {
                        case ActionType.Create:
                        {
                            _notificationService.Notify(new NotificationMessage
                                {Severity = NotificationSeverity.Success, Summary = "Create Success", Duration = 4000});
                            break;
                        }
                        case ActionType.Update:
                        {
                            _notificationService.Notify(new NotificationMessage
                            {
                                Severity = NotificationSeverity.Success, Summary = "Update Success",
                                Duration = 4000
                            });
                            break;
                        }
                        case ActionType.Get:
                        {
                            _notificationService.Notify(new NotificationMessage
                            {
                                Severity = NotificationSeverity.Success, Summary = "Success", Duration = 4000
                            });
                            break;
                        }
                        case ActionType.Send:
                        {
                            _notificationService.Notify(new NotificationMessage
                            {
                                Severity = NotificationSeverity.Success, Summary = "Sent Successfully", Duration = 4000
                            });
                            break;
                        }
                        case ActionType.GetList:
                        {
                            _notificationService.Notify(new NotificationMessage
                            {
                                Severity = NotificationSeverity.Success, Summary = "Get Success", Duration = 4000
                            });
                            break;
                        }
                        case ActionType.Delete:
                        {
                            _notificationService.Notify(new NotificationMessage
                                {Severity = NotificationSeverity.Success, Summary = "Delete Success", Duration = 4000});
                            break;
                        }
                        case ActionType.SignIn:
                        {
                            _notificationService.Notify(new NotificationMessage
                            {
                                Severity = NotificationSeverity.Success, Summary = "Sign In Success", Duration = 4000
                            });
                            break;
                        }
                        case ActionType.SignOut:
                        {
                            _notificationService.Notify(new NotificationMessage
                            {
                                Severity = NotificationSeverity.Success, Summary = "Sign Out Success", Duration = 4000
                            });
                            break;
                        }
                        case ActionType.SignUp:
                        {
                            _notificationService.Notify(new NotificationMessage
                            {
                                Severity = NotificationSeverity.Success, Summary = "Sign Up Success", Duration = 4000
                            });
                            break;
                        }

                        case ActionType.Review:
                        {
                            _notificationService.Notify(new NotificationMessage
                            {
                                Severity = NotificationSeverity.Success, Summary = "Review Success",
                                Duration = 4000
                            });
                            break;
                        }

                        case ActionType.UploadFile:
                        {
                            _notificationService.Notify(new NotificationMessage
                            {
                                Severity = NotificationSeverity.Success, Summary = "Upload File Succeeded ",
                                Duration = 4000
                            });
                            break;
                        }

                        case ActionType.Reset:
                        {
                            _notificationService.Notify(new NotificationMessage
                            {
                                Severity = NotificationSeverity.Success, Summary = "Reset Succeeded ", Duration = 4000
                            });
                            break;
                        }
                        
                        case ActionType.LoadData:
                        {
                            break;
                        }

                    }
                }


                return true;
            }
            catch (Exception e)
            {
                var exceptionType = e.GetType();

                if (exceptionType == typeof(BadRequestException))
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error, Summary = "Error Summary", Detail = e.Message,
                        Duration = 4000
                    });

                }

                if (exceptionType == typeof(UnauthorizedOperationException))
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Warning, Summary = "Error Summary", Detail = e.Message,
                        Duration = 4000
                    });
                }
                
                if (exceptionType == typeof(UnauthorizedException))
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Warning,
                        Summary =
                            "Your session has ended. The system will logout automatically after 4 seconds",
                        Detail = e.Message, Duration = 4000
                    });
                    Thread.Sleep(4000);
                    
                    _navigationManager.NavigateTo($"login?ToURl={_navigationManager.Uri}", true);
                }

                if (exceptionType == typeof(ServerErrorException))
                {
                    _navigationManager.NavigateTo("server-error", true);
                }

                if (exceptionType == typeof(DbConnectionException))
                {
                    _navigationManager.NavigateTo("connection-error", true);
                }

                if (exceptionType == typeof(ConflictException))
                {
                    _notificationService.Notify(new NotificationMessage
                        {Severity = NotificationSeverity.Warning, Summary = "", Detail = e.Message, Duration = 4000});
                }

                if (exceptionType == typeof(TooManyRequests))
                {
                    // too many request
                }

                if (exceptionType == typeof(NotFoundException))
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Warning, Summary = "Not Found", Detail = e.Message,
                        Duration = 4000
                    });
                }

                if (exceptionType == typeof(FailedOperation))
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Warning, Summary = "The Failed Operation", Detail = e.Message,
                        Duration = 4000
                    });

                }

                if (exceptionType == typeof(HttpRequestException))
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error, Summary = "Can't connect to server APi ", Detail = "Can't connect to Api server. Please check the network signal on both sides",Duration = 5000,
                    });
                }

                if (exceptionType == typeof(TaskCanceledException))
                {
                    // _notificationService.Notify(new NotificationMessage
                    // {
                    //     Severity = NotificationSeverity.Warning, Summary = "Can't connect to server APi ", Detail = "Thời gian yêu cầu quá lâu.vui lòng reload thử lại",Duration = 5000,
                    // });
 
                }

                // _navigationManager.NavigateTo("server-error",true);

            }finally
            {
                IsDisable = false;
            }

            return false;

        }

        public void NotifyMessage(NotificationSeverity type,string message,int duration)
        {
            _notificationService.Notify(new NotificationMessage { Severity = type, Summary = message, Duration = duration });
        }


        protected async Task<Dictionary<string, DateRange>> GetDateRangePickers()
        {
            
            var now = new DateTimeOffset(DateTime.Now.Date);
            
          var ranges =  new Dictionary<string, DateRange>()
          {
              {
                  DateRangeType.Today.GetDescriptionOrName(),
                  new DateRange()
                  {
                      Start = now,
                      End = now.Add(new TimeSpan(23,59,59))
                  }
              },
              {
                  DateRangeType.Yesterday.GetDescriptionOrName(),
                  new DateRange()
                  {
                      Start = now.AddDays(-1),
                      End = now.AddDays(-1).Add(new TimeSpan(23,59,59))
                  }
              },
              {
                  DateRangeType.Last7Days.GetDescriptionOrName(),
                  new DateRange()
                  {
                      Start = now.AddDays(-7).Add(new TimeSpan(23,59,59)),
                      End = now
                  }
              },
              {
                  DateRangeType.Last30Days.GetDescriptionOrName(),
                  new DateRange()
                  {
                      Start = now.AddDays(-30).Add(new TimeSpan(23,59,59)),
                      End = now
                  }
              },
              {
                  DateRangeType.ThisMonth.GetDescriptionOrName(),
                  new DateRange()
                  {
                      Start = now.FirstDayOfMonth(),
                      End = now.LastDayOfMonth()
                  }
              },
              {
                  DateRangeType.LastMonth.GetDescriptionOrName(),
                  new DateRange()
                  {
                      Start = now.AddMonths(-1).FirstDayOfMonth(),
                      End = now.AddMonths(-1).LastDayOfMonth()
                  }
              },
              {
                  DateRangeType._3MonthsAgo.GetDescriptionOrName(),
                  new DateRange()
                  {
                      Start = now.AddMonths(-3).FirstDayOfMonth(),
                      End = now.Add(new TimeSpan(23,59,59))
                  }
              }
              
          
          };

          
          

          return ranges;

        }

        protected async Task<(Dictionary<string, DateRange>DateRanges,DateTimeOffset StartDay,DateTimeOffset EndDay)>
            GetDateRangePickersWithDefault()
        {
           var dateRanges = await GetDateRangePickers();
            
            var dateRange = dateRanges.
                FirstOrDefault(x => x.Key ==  DateRangeType._3MonthsAgo.GetDescriptionOrName()).Value;

            return (dateRanges, dateRange.Start, dateRange.End);
        }

        protected (DateTime?,DateTime?) GetDateTimeFromOffSet(DateTimeOffset? fromDateOffset, DateTimeOffset? toDateTimeOffset)
        {
            if (!fromDateOffset.HasValue || !toDateTimeOffset.HasValue) return (null, null);
            return (fromDateOffset.Value.DateTime, toDateTimeOffset.Value.DateTime);
        }
        
        

        public async Task<string> GetUserNameAsync()
        {
            var authState = await AuthState;
            var user = authState.User;

            return user.Identity.Name;
        }
        
        public async Task<Guid> GetUserIdAsync()
        {
            var authState = await AuthState;
            var user = authState.User;
            return Guid.Parse(user.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.PrimarySid)?.Value);
        }

        public async Task<List<string>> GetUserRolesAsync()
        {
            var authState = await AuthState;
            var user = authState.User;
            var roleClaims = user.Claims
                .Where(x => x.Type == ClaimTypes.Role);
            return roleClaims.Select(x => x.Value).ToList();
        }



        public async Task<bool> IsAuthenticatedAsync()
        {
            var authState = await AuthState;
            var user = authState.User;
            if (user.Identity.IsAuthenticated)
            {
                return true;
            }

            return false;
        }

        public async Task<byte[]> GetByteDataAsync(IBrowserFile file)
        {
            using (var ms = new MemoryStream())
            {
                await file.OpenReadStream(BlazorSetting.Document_FILE_LENGTH_LIMIT).CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
              

                return ms.GetAllBytes();
            }
        }


        public void Dispose()
        {
            RequestClient.CancelToken();
        }
    }
}