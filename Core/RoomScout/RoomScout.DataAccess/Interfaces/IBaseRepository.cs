namespace RoomScout.DataAccess.Interfaces
{
    public interface IBaseRepository<T>
    {
        Task<ICollection<T>> GetAllAsync();
    }
}
