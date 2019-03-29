// © 2019 Baker Hughes, a GE company.  All rights reserved.
// This document contains confidential and proprietary information owned by Baker Hughes, a GE company.
// Do not use, copy or distribute without permission.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using PlayStats.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PlayStats.UI
{
    public class AddPlayViewModel : ReactiveObject
    {
        private readonly IRepository _repository;

        public AddPlayViewModel(IRepository repository)
        {
            _repository = repository;
            repository.Games
                .Sort(SortExpressionComparer<GameModel>.Ascending(p=>p.Name))
                .Transform(p=> new AvailableGameViewModel(p.Id, p.Name))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _availableGames).Subscribe();

            Save = ReactiveCommand.Create(SavePlay);

            SelectedDate = DateTime.Today;
            PlayerCount = 1;
        }
        
        [Reactive] public AvailableGameViewModel SelectedGame { get; set; }
        [Reactive] public DateTime SelectedDate { get; set; }
        [Reactive] public DateTime? SelectedTime { get; set; }
        [Reactive] public int PlayerCount { get; set; }
        [Reactive] public string Description { get; set; }

        public ICommand Save { get; }

        private readonly ReadOnlyObservableCollection<AvailableGameViewModel> _availableGames;
        public IEnumerable<AvailableGameViewModel> AvailableGames => _availableGames;

        private void SavePlay()
        {
            var play = _repository.CreatePlay(SelectedGame.Id);
            play.Comment = Description;
            play.Date = SelectedDate;
            play.Duration = new TimeSpan(0, SelectedTime.Value.Hour, SelectedTime.Value.Minute);
            play.PlayerCount = PlayerCount;

            _repository.AddOrUpdate(play);
        }
    }

    public class AvailableGameViewModel : ReactiveObject
    {
        public AvailableGameViewModel(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; }
        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}