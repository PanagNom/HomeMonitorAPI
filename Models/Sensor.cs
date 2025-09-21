namespace HomeMonitorAPI.Models
{
    public class Sensor
    {
        public int Id { get; set; }
        public int Temperature { get; set; }
        public int Humidity { get; set; }
        public int Sound { get; set; }
        public int Light { get; set; }
        public Boolean Presence { get; set; }
    }
}
