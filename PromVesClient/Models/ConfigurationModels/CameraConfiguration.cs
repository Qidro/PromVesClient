using System.Collections.Generic;

namespace PromVesClient.Models.ConfigurationModels
{
    public class CameraConfiguration
    {
        public List<CameraSettings> Cameras { get; set; } = [];
    }

    public class CameraSettings
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Ip { get; set; } = string.Empty;
    }
}
