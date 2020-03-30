using Domain;
using Service.DomainServices.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.DomainServices
{
    public class UserRepository : IUserRepository
    {
        public static User User { get; set; }
    }
}
