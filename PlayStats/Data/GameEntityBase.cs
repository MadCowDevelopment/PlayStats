﻿namespace PlayStats.Data
{
    public class GameEntityBase : Entity
    {
        public string Name { get; set; }

        public double? PurchasePrice { get; set; }

        public double? SellPrice { get; set; }

        public bool IsGenuine { get; set; }

        public bool WantToSell { get; set; }

        public bool IsDelivered { get; set; }

        #region BGG data

        public int? ObjectId { get; set; }

        public string FullName { get; set; }

        public int YearPublished { get; set; }

        public string Description { get; set; }

        public byte[] Thumbnail { get; set; }

        public byte[] Image { get; set; }

        public string Publishers { get; set; }

        public string Designers { get; set; }

        #endregion
    }
}
