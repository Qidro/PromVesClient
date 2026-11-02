namespace PromVesClient.DTO
{
    public class MediaWeighingDto
    {
        public string VagonNumber { get; set; } = string.Empty;
        public DateTime WeighingDateTime { get; set; }
        public string PhotoName { get; set; } = string.Empty;
        public string PhotoPath { get; set; } = string.Empty;
        public string VideoName { get; set; } = string.Empty;
        public string VideoPath { get; set; } = string.Empty;
    }
}
