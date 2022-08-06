using System;
using System.ComponentModel.DataAnnotations;

namespace TodoApp.Domain.Common
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

    }
}
