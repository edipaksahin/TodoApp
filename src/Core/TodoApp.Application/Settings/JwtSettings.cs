using System;
using System.Collections.Generic;
using System.Text;

namespace TodoApp.Application.Settings
{
    public class JwtSettings
    {
        public string JwtSecretKey { get; set; }
        public int JwtTokenExpireDay { get; set; }
    }
}
