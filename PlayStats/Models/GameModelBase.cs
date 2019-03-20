using ReactiveUI;
using System;

namespace PlayStats.Models
{
    public class GameModelBase : Model
    {
        public GameModelBase(Guid id) : base(id)
        {
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        private double _purchasePrice;
        public double PurchasePrice
        {
            get => _purchasePrice;
            set => this.RaiseAndSetIfChanged(ref _purchasePrice, value);
        }

        private double _sellPrice;
        public double SellPrice
        {
            get => _sellPrice;
            set => this.RaiseAndSetIfChanged(ref _sellPrice, value);
        }

        private bool _isGenuine;
        public bool IsGenuine
        {
            get => _isGenuine;
            set => this.RaiseAndSetIfChanged(ref _isGenuine, value);
        }

        private bool _wantToSell;
        public bool WantToSell
        {
            get => _wantToSell;
            set => this.RaiseAndSetIfChanged(ref _wantToSell, value);
        }

        private bool _isDelivered;
        public bool IsDelivered
        {
            get => _isDelivered;
            set => this.RaiseAndSetIfChanged(ref _isDelivered, value);
        }

        #region BGG data

        private int _objectId;
        public int ObjectId
        {
            get => _objectId;
            set => this.RaiseAndSetIfChanged(ref _objectId, value);
        }

        private string _fullName;
        public string FullName
        {
            get => _fullName;
            set => this.RaiseAndSetIfChanged(ref _fullName, value);
        }

        private int _yearPublished;
        public int YearPublished
        {
            get => _yearPublished;
            set => this.RaiseAndSetIfChanged(ref _yearPublished, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
        }

        private byte[] _thumbnail;
        public byte[] Thumbnail
        {
            get => _thumbnail;
            set => this.RaiseAndSetIfChanged(ref _thumbnail, value);
        }

        private byte[] _image;
        public byte[] Image
        {
            get => _image;
            set => this.RaiseAndSetIfChanged(ref _image, value);
        }

        private string _publisher;
        public string Publisher
        {
            get => _publisher;
            set => this.RaiseAndSetIfChanged(ref _publisher, value);
        }

        private string _designer;
        public string Designer
        {
            get => _designer;
            set => this.RaiseAndSetIfChanged(ref _designer, value);
        }

        #endregion
    }
}