using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Positions;
using Contract.Positions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/position/")]
    [Authorize]
    public class PositionController : IPositionService
    {
        private PositionService _positionService;
        public PositionController(PositionService positionService)
        {
            _positionService = positionService;
        }
        
        [HttpPost]
        public async Task<PositionDto> CreateAsync(CreateUpdatePositionDto input)
        {
            return await _positionService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<PositionDto> UpdateAsync(CreateUpdatePositionDto input, Guid id)
        {
           return  await _positionService.UpdateAsync(input,id);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _positionService.DeleteAsync(id);
        }

        [HttpGet]
        public async Task<List<PositionDto>> GetListAsync()
        {
            return await _positionService.GetListAsync();
        }
    }
}