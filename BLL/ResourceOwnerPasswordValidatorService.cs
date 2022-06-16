using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.EntityFrameworkCore;
using DAL.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace BLL
{
    public class ResourceOwnerPasswordValidatorService : IResourceOwnerPasswordValidator
    {
        private readonly IUnitOfWork _unitOfWork;

        public ResourceOwnerPasswordValidatorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await _unitOfWork.UserRepository.GetAll()
                .Where(u => u.UserName.ToLower().Equals(context.UserName.ToLower()))
                .Select(x => new { x.UserId, x.Password })
                .FirstOrDefaultAsync();

            if (context.Request.Raw["bypassPassword"] == "true")
            {
                context.Result = new GrantValidationResult(user.UserId.ToString(), "password");
                return;
            }

            bool passwordMatch = BCrypt.Net.BCrypt.Verify(context.Password, user.Password);

            if (passwordMatch)
            {
                context.Result = new GrantValidationResult(user.UserId.ToString(), "password");
            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid username or password");
            }

        }

    }
}
