using Microsoft.EntityFrameworkCore;

namespace OpenAPIASPNET.Contexts.Models
{
    [PrimaryKey("Id")]
    public class Events
    {
        public Guid Id { get; set; }
        public Guid User { get; set; }
        public byte EventCode { get; set; }
        public string? EventDescription { get; set; }
        public DateTime EventTime { get; set; }
    }
}
