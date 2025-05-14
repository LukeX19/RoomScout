using RoomScout.DataAccess.Interfaces;

namespace RoomScout.DataAccess.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T>
    {
        protected readonly ICollection<T> _entities;

        public BaseRepository(ICollection<T> entities)
        {
            _entities = entities;
        }

        public async Task<ICollection<T>> GetAllAsync()
        {
            return await Task.FromResult(_entities.ToList());
        }
    }
}
