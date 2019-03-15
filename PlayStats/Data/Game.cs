using System;

namespace PlayStats.Data
{
    public class Game : Entity
    {
        // BGG object id.
        public int ObjectId { get; set; }

        public string Name { get; set; }

        public Guid ParentId { get; set; }

        public double PurchasePrice { get; set; }

        public double SellPrice { get; set; }

        public bool IsGenuine { get; set; }           
        
        public bool WantToSell { get; set; }

        public bool IsDelivered { get; set; }

        public int Rating { get; set; }

        public int DesireToPlay { get; set; }        
    }
}
