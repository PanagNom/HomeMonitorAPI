namespace HomeMonitorAPI.Data.Interfaces
{
    public interface ISensorRepository
    {
        Task<Models.Sensor?> GetSensorData();
        Task<Models.Sensor> GetSensorByIdAsync(int id);
        Task AddSensorAsync(Models.Sensor sensor);
        //Task UpdateSensorAsync(Models.Sensor sensor);
        Task DeleteSensorAsync(int id);
    }
}
