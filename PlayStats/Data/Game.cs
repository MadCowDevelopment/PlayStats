using System;

namespace PlayStats.Data
{
    public class Game : Entity
    {
        public string Name { get; set; }

        public Guid ParentId { get; set; }

        public double PurchasePrice { get; set; }

        public double SellPrice { get; set; }

        public bool IsGenuine { get; set; }

        public bool WantToSell { get; set; }

        public bool IsDelivered { get; set; }

        public int Rating { get; set; }

        public int DesireToPlay { get; set; }

        #region BGG data

        public int ObjectId { get; set; }

        public string FullName { get; set; }

        public int YearPublished { get; set; }

        public string Description { get; set; }

        public byte[] Image { get; set; }

        internal void SetProperties(Game game)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
