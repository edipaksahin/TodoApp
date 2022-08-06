using System;

namespace TodoApp.Application.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public string Key { get; set; }

        public EntityNotFoundException() : base($"Entity was not found.") { }
        public EntityNotFoundException(string name, Guid key) : this(name, key.ToString()) { }
        public EntityNotFoundException(string name, int key) : this(name, key.ToString()) { }
        public EntityNotFoundException(string name, string key) : base($"Entity \"{name}\" ({key}) was not found.")
        {
            Key = key.ToString();
        }
    }
}
