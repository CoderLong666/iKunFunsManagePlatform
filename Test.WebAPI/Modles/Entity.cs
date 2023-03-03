using NodaTime;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;

namespace Test.WebAPI.Modles
{
    public interface IIdentify
    {
        [Key] public Guid Id { get; set; }
    }

    public abstract class Entity : IIdentify
    {
        [Key]
        public Guid Id { get; set; }

        protected Entity()
        {
            Id = Guid.NewGuid();
        }

        public DateTime CreatedAt { get; set; }

        [Required]
        public string CreatedBy { get; set; } = "";

        public DateTime? UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
