using Microsoft.AspNetCore.Identity;
using Nexzoz.API.Models;
using Nexzoz.API.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nexzoz.API.Data.Interfaces
{
    public interface IAuthRepository
    {
        #region Properties
        Task<NexzozUser> Register(string user, string password);
        Task<NexzozUser> Login(string username, string password);
        Task<bool> UserExists(string email);
        Task<string> GenerateJwt(string username, string password);
        Task<IEnumerable<IdentityError>> TryCreateUser(string email, string password);
        #endregion
    }
}
