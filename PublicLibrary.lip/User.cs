﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicLibrary.lip
{
    public class User
    { 
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
