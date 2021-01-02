using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly ILikedRepository _likedRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LikesController(ILikedRepository likedRepository, IUnitOfWork unitOfWork)
        {
            _likedRepository = likedRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _likedRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null)
                return NotFound();

            if (sourceUser.UserName == username)
                return BadRequest("KEndini beğenemezsin.");

            var userLike = await _likedRepository.GetUserLike(sourceUserId, likedUser.Id);

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id,
            };

            sourceUser.LikedUsers.Add(userLike);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("Hata oluştu");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery] LikedParams likedParams)
        {
            likedParams.UserId = User.GetUserId();
            var users = await _likedRepository.GetUserLikes(likedParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users);
        }
    }
}
