using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contract.Positions
{
    public interface IPositionService
    {
        Task<PositionDto> CreateAsync(CreateUpdatePositionDto input);
        Task<PositionDto> UpdateAsync(CreateUpdatePositionDto input,Guid id);
        Task DeleteAsync(Guid id);
        Task<List<PositionDto>> GetListAsync();

    }
}