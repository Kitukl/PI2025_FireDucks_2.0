namespace StudyHub.DAL.Repositories;

public interface IBaseRepository<T>
{
    public Task<T> CreateAsync(T item);

    public Task<List<T>> GetAll();

    public Task<int> UpdateAsync(T item);

    public Task<int> DeleteAsync(int id);

    public Task<T> GetById(int id);
}