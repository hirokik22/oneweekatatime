using System;
using System.Data.Common;
using System.Text.Json.Serialization;

namespace OneWeekAtATimeBack.Model.Entities;

public class Login
{
    public int LoginID { get; set; }
   public string  Email { get; set; }
   public int PasswordHash { get; set; }

}
