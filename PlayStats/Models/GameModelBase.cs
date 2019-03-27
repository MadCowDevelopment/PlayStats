using ReactiveUI.Fody.Helpers;
using System;

namespace PlayStats.Models
{
    public class GameModelBase : Model
    {
        protected GameModelBase(Guid id) : base(id)
        {
        }

        [Reactive] public string Name { get; set; }
        [Reactive] public double PurchasePrice { get; set; }
        [Reactive] public double SellPrice { get; set; }
        [Reactive] public bool IsGenuine { get; set; }
        [Reactive] public bool WantToSell { get; set; }
        [Reactive] public bool IsDelivered { get; set; }

        #region BGG data
        [Reactive] public int ObjectId { get; set; }
        [Reactive] public string FullName { get; set; }
        [Reactive] public int YearPublished { get; set; }
        [Reactive] public string Description { get; set; }
        [Reactive] public byte[] Thumbnail { get; set; }
        [Reactive] public byte[] Image { get; set; }
        [Reactive] public string Publisher { get; set; }
        [Reactive] public string Designer { get; set; }
        #endregion
    }
}