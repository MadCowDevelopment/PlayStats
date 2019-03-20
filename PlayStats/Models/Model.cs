using ReactiveUI;
using System;

namespace PlayStats.Models
{
    public class Model : ReactiveObject
    {
        public Model(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; }
    }
}