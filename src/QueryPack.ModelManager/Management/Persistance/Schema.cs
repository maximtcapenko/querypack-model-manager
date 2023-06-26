namespace QueryPack.ModelManager.Management.Persistance
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Index(nameof(Version), nameof(ProductId), IsUnique = true)]
    public class Schema : BaseModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public string ProductId { get; set; }
        [Required]
        public string Version { get; set; }
        [Required]
        public string JsonSchema { get; set; }
        [Required]
        public string MigrationId { get; set; }
        [Required]
        public string MigrationCode { get; set; }
        [Required]
        public string MigrationMetaCode { get; set; }
        [Required]
        public Snapshot Snapshot { get; set; }
        public Schema Previous { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsApproved { get; set; }
        public bool IsProvisioned { get; set; }
        public DateTimeOffset? ApprovedAt { get; set; }
        public DateTimeOffset? ProvisionedAt { get; set; }
    }
}