﻿using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface ISendEmailRepository : IRepository<SendEmail>
    {
        List<string> GetEmailByFactoryCode(string factoryCode);
    }
}