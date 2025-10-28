namespace StudyHub.DAL.Repositories;

public interface IBaseRepository<T>
{
    public Task<T> CreateAsync(T item);
    public Task<T> GetAll();
    public Task<T> UpdateAsync(T item);
    public Task<T> DeleteAsync(int id);
    public Task<T> GetById(int id);
    
}