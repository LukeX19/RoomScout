using Newtonsoft.Json;
using RoomScout.DataAccess.Models;

namespace RoomScout.DataAccess
{
    public class DataContext
    {
        public ICollection<Hotel> Hotels { get; private set; } = new List<Hotel>();
        public ICollection<Booking> Bookings { get; private set; } = new List<Booking>();

        public async Task InitializeAsync(string hotelsFilePath, string bookingsFilePath)
        {
            Hotels = await LoadFromFileAsync<List<Hotel>>(hotelsFilePath) ?? new List<Hotel>();
            Bookings = await LoadFromFileAsync<List<Booking>>(bookingsFilePath) ?? new List<Booking>();
        }

        private async Task<T?> LoadFromFileAsync<T>(string filePath)
        {
            using var reader = new StreamReader(filePath);
            var json = await reader.ReadToEndAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
