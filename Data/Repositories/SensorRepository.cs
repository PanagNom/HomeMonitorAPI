using HomeMonitorAPI.Data.Interfaces;
using HomeMonitorAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeMonitorAPI.Data.Repositories
{
    public class SensorRepository(HomeMonitorDbContext context) : ISensorRepository
    {
        private readonly HomeMonitorDbContext context = context;

        public async Task AddSensorAsync(Sensor sensor)
        {
            ArgumentNullException.ThrowIfNull(sensor);

            await this.context.Sensors.AddAsync(sensor);
            await this.context.SaveChangesAsync();
        }

        public async Task DeleteSensorAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }

            Sensor? temp = await this.context.Sensors.FindAsync(id) ?? throw new KeyNotFoundException($"Sensor with ID {id} not found.");
            this.context.Sensors.Remove(temp);
            await this.context.SaveChangesAsync();
        }

        public async Task<Sensor> GetSensorByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }

            Sensor? temp = await this.context.Sensors.FindAsync(id) ?? throw new KeyNotFoundException($"Sensor with ID {id} not found.");
            return temp;
        }

        public async Task<Sensor?> GetSensorData()
        {
            return await this.context.Sensors.OrderBy(s => s.Id).FirstOrDefaultAsync();
        }
    }
}