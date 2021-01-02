using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories
{
    public class LikedRepository : ILikedRepository
    {
        private readonly DataContext _context;

        public LikedRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikedParams likedParams)
        {
            var users = _context.Users.OrderBy(x => x.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if (likedParams.Predicate == "liked")
            {
                likes = likes.Where(x => x.SourceUserId == likedParams.UserId);
                users = likes.Select(x => x.LikedUser);
            }
            else if (likedParams.Predicate == "likedBy")
            {
                likes = likes.Where(x => x.LikedUserId == likedParams.UserId);
                users = likes.Select(x => x.SourceUser);
            }

            var likedUsers = users.Select(x => new LikeDto
            {
                Username = x.UserName,
                KnownAs = x.KnownAs,
                Age = x.DateOfBirth.CalculateAge(),
                PhotoUrl = x.Photos.FirstOrDefault(t => t.IsMain).Url,
                City = x.City,
                Id = x.Id,
            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers, likedParams.PageNumber, likedParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                .Include(x => x.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}
