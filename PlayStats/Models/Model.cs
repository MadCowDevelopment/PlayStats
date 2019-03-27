using ReactiveUI;
using System;

namespace PlayStats.Models
{
    public class Model : ReactiveObject
    {
        protected Model(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}