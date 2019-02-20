using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Nexzoz.API.Data.Interfaces;
using Nexzoz.API.Models;
using Nexzoz.API.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nexzoz.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        #region Properties

        #region Internals
        UserManager<NexzozUser> UserManager { get; }
        IJwtFactory JwtFactory { get; }
        JwtIssuerOptions JwtOptions { get; }
        IMapper Mapper { get; set; }
        #endregion

        #endregion

        #region Constructors
        public AuthRepository(UserManager<NexzozUser> userManager, IJwtFactory jwtFactory, 
            IOptions<JwtIssuerOptions> jwtOptions, IMapper mapper)
        {
            UserManager = userManager;
            JwtFactory = jwtFactory;
            JwtOptions = jwtOptions.Value;
            Mapper = mapper;
        }
        #endregion

        #region Methods

        #region IAuthRepository Implementation
        public async Task<NexzozUser> Register(string email, string password)
        {
            NexzozUser user = new NexzozUser(email);
            user.UserName = email;
            await UserManager.CreateAsync(user);
            await UserManager.UpdateAsync(user);
            return user;
        }

        public async Task<NexzozUser> Login(string email, string password)
        {
            var user = await UserManager.FindByEmailAsync(email);
            
            if (user == null) return null;
            if (!(await UserManager.CheckPasswordAsync(user, password)))
                return null;
            return user;
        }

        public async Task<IEnumerable<IdentityError>> TryCreateUser(string email, string password)
        {
            NexzozUser user = new NexzozUser(email);
            user.UserName = email;
            var result = await UserManager.CreateAsync(user, password);
            return result.Errors;
        }

        public async Task<bool> UserExists(string email) => await UserManager.FindByEmailAsync(email) != null;

        public async Task<string> GenerateJwt(string username, string password)
        {
            ClaimsIdentity identity = await GetClaimsIdentity(username, password);
            var response = new
            {
                id = identity.Claims.Single(c => c.Type == "id").Value,
                auth_token = await JwtFactory.GenerateEncodedToken(username, identity),
                expires_in = (int)JwtOptions.ValidFor.TotalSeconds
            };

            return JsonConvert.SerializeObject(response);
        }
        #endregion

        async Task<ClaimsIdentity> GetClaimsIdentity(string username, string password)
        {
            var user = await UserManager.FindByNameAsync(username);
            if (await UserManager.CheckPasswordAsync(user, password))
                return await Task.Run(() => JwtFactory.GenerateClaimsIdentity(username, user.Id));
            return await Task.FromResult<ClaimsIdentity>(null);
        }

        #endregion
    }
}
