using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;
using DAL.Model;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BLL
{
    public class UserProfileService : IProfileService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserProfileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            Guid userId = new Guid(context.Subject.GetSubjectId());
            List<Claim> claims = new List<Claim>();

            var user = await _unitOfWork.UserRepository
                .GetAllIncluding()
                .Where(u => u.UserId == userId)
                .Select(u =>
                new
                {
                    u.UserId,
                    u.UserName,
                    u.Firstname,
                    u.LastName,
                    u.Email,
                    u.Gender,
                    role = u.UserRoles.Select(ur => ur.Role.RoleCode)
                })
                .FirstAsync();
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(ClaimTypes.GivenName, user.Firstname));
            claims.Add(new Claim(ClaimTypes.Surname, user.LastName));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.Gender, user.Gender));

            foreach (string role in user.role)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }


            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            string sub = context.Subject.GetSubjectId();
            User user = await _unitOfWork.UserRepository.GetByIdAsync(new Guid(sub));
            if (user != null) context.IsActive = true;
        }
    }
}
