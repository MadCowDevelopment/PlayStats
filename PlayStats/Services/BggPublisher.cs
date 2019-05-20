using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PlayStats.Services
{
    public class BggPublisher : ReactiveObject
    {
        [Reactive] public int ObjectId { get; set; }
        [Reactive] public string Name { get; set; }
    }
}