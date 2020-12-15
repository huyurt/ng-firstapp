using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data.Repositories;
using API.Interfaces;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public UnitOfWork(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public IUserRepository UserRepository => new UserRepository(_mapper, _context);

        public IMessageRepository MessageRepository => new MessageRepository(_mapper, _context);

        public ILikedRepository LikedRepository => new LikedRepository(_context);

        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}
