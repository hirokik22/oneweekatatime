using System;
using System.Data.Common;
using System.Text.Json.Serialization;

namespace OneWeekAtATimeBack.Model.Entities;

public class Roomie
{

  public int RoomieID { get; set; }
   public string  RoomieName { get; set; }
   public int LoginID { get; set; }
}
