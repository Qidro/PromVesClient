using System;
using System.Collections.Generic;
using System.Text;

namespace PromVesClient.Models
{
    public class MediaWeighing
    {
        public Guid Id { get; set; }

        public Guid WeighingId { get; set; }

        public string? FileNamePhoto { get; set; } = "";

        public string? FilePathPhoto { get; set; } = "";

        public string? FileNameVideo { get; set; } = "";

        public string? FilePathVideo { get; set; } = "";

        public Weighing Weighing { get; set; } = null!;

    }
}
