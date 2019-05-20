using System.Collections.Generic;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PlayStats.Services
{
    public class BggGameDetail : ReactiveObject
    {
        [Reactive] public int ObjectId { get; set; }
        [Reactive] public string FullName { get; set; }
        [Reactive] public int YearPublished { get; set; }
        [Reactive] public string Description { get; set; }
        [Reactive] public byte[] Thumbnail { get; set; }
        [Reactive] public byte[] Image { get; set; }
        [Reactive] public IEnumerable<BggPublisher> Publishers { get; set; }
        [Reactive] public IEnumerable<BggDesigner> Designers { get; set; }
    }
}