using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using postgre.Data;
using postgre.Models;

namespace postgre.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public HomeController(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ActionResult> List()
        {
            return Ok(await _context.MyTables
                .ProjectTo<MyTableDtoModel>(_mapper.ConfigurationProvider)
                .ToListAsync());
        }

        [HttpPost("Create")]
        public async Task<ActionResult> Create(MyTableDtoModel model)
        {
            var record = _mapper.Map<MyTableDomainModel>(model);

            _context.MyTables.Add(record);
            await _context.SaveChangesAsync();
            return Ok(await _context.MyTables
                .ProjectTo<MyTableDtoModel>(_mapper.ConfigurationProvider)
                .ToListAsync());
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var record = new MyTableDomainModel
            {
                Id = id,
            };

            _context.MyTables.Remove(record);
            await _context.SaveChangesAsync();
            return Ok(await _context.MyTables
                .ProjectTo<MyTableDtoModel>(_mapper.ConfigurationProvider)
                .ToListAsync());
        }

        [HttpPost("UpdateName")]
        public async Task<ActionResult> UpdateName(MyTableDtoModel model)
        {
            var record = _mapper.Map<MyTableDomainModel>(model);

            _context.Entry(record).Property(x => x.Name).IsModified = true;
            await _context.SaveChangesAsync();
            return Ok(await _context.MyTables
                .ProjectTo<MyTableDtoModel>(_mapper.ConfigurationProvider)
                .ToListAsync());
        }
    }
}
