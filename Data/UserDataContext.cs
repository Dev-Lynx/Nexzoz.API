using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nexzoz.API.Models;
using Nexzoz.API.Models.Entities;

namespace Nexzoz.API.Data
{
    public class UserDataContext : IdentityDbContext<NexzozUser>
    {
        #region Properties

        #region DbSets
        #endregion   
   
        #endregion   

        #region Construtctors
        public UserDataContext(DbContextOptions options) : base(options) {  }
        #endregion

        #region Methods

        #endregion
        
    }
}
