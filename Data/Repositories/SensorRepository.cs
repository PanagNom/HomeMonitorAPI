using HomeMonitorAPI.Data.Interfaces;
using HomeMonitorAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeMonitorAPI.Data.Repositories
{
    public class SensorRepository : ISensorRepository
    {
        private readonly HomeMonitorDbContext _context;
        public SensorRepository(HomeMonitorDbContext context)
        {
            _context = context;
        }

        public async Task AddSensorAsync(Sensor sensor)
        {
            if (sensor == null)
            {
                throw new ArgumentNullException(nameof(sensor));
            }

            await _context.Sensors.AddAsync(sensor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSensorAsync(int id)
        {
            if(id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }

            Sensor? temp = await _context.Sensors.FindAsync(id);

            if(temp == null)
            {
                throw new KeyNotFoundException($"Sensor with ID {id} not found.");
            }

            _context.Sensors.Remove(temp);
            await _context.SaveChangesAsync();
        }

        public async Task<Sensor> GetSensorByIdAsync(int id)
        {
            if(id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }
            Sensor? temp = await _context.Sensors.FindAsync(id);

            if(temp == null)
            {
                throw new KeyNotFoundException($"Sensor with ID {id} not found.");
            }
            return temp;
        }

        public async Task<Sensor?> GetSensorData()
        {
            return await _context.Sensors.OrderBy(s => s.Id).FirstOrDefaultAsync();
        }
    }
}
