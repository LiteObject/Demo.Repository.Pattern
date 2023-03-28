using System.ComponentModel.DataAnnotations;

namespace Demo.Repository.Pattern.Domain
{
    public abstract class BaseEntity
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the updated on.
        /// </summary>
        public DateTime? UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets the updated by.
        /// </summary>
        public string? UpdatedBy { get; set; }
    }
}
