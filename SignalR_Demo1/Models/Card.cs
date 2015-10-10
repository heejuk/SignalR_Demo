using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalR_Demo1.Models
{
    public class Card
    {
        public int Id { get; set; }
        public int Pair { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
    }
}