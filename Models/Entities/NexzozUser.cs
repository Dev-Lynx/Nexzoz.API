using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexzoz.API.Models.Entities
{
    public class NexzozUser : IdentityUser
    {
        #region Properties
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long? FacebookId { get; set; }
        #endregion


        #region Constructors
        public NexzozUser() { }
        public NexzozUser(string email) 
        {
            Email = email;
        }
        #endregion
    }
}
