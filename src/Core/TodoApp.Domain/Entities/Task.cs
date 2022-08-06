using TodoApp.Domain.Common;

namespace TodoApp.Domain.Entities
{
    public class Task : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }

        public void SetCompleted()
        {
            IsCompleted = true;
        }
    }
}
