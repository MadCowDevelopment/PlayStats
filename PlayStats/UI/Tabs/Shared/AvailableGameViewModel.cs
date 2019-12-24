using System;
using ReactiveUI;

namespace PlayStats.UI.Tabs.Shared
{
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