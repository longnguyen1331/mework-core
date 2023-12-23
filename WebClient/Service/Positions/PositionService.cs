using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contract.Positions;
using WebClient.RequestHttp;

namespace WebClient.Service.Positions
{
    public class PositionService : IPositionService
    {
        public PositionService()
        {
            
        }
        
        public async Task<List<PositionDto>> GetListAsync()
        {
            return await RequestClient.GetAPIAsync<List<PositionDto>>("position");
        }

        public async Task<PositionDto> CreateAsync(CreateUpdatePositionDto input)
        {
            return await RequestClient.PostAPIAsync<PositionDto>("position",input);

        }

        public async Task<PositionDto> UpdateAsync(CreateUpdatePositionDto input, Guid id)
        {
            return await RequestClient.PutAPIAsync<PositionDto>($"position/{id}" , input);

        }

        public async Task DeleteAsync(Guid id)
        { 
            await RequestClient.DeleteAPIAsync<Task>($"position/{id}");
        }
    }
}