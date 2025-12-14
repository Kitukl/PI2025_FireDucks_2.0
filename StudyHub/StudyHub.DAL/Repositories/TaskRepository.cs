using Microsoft.EntityFrameworkCore;
using StudyHub.DAL.Entities;

namespace StudyHub.DAL.Repositories;

public class TaskRepository : IBaseRepository<DAL.Entities.Task>
{
    private readonly StudyContext _context;

    public TaskRepository(StudyContext context)
    {
        _context = context;
    }

    public virtual async Task<DAL.Entities.Task> CreateAsync(DAL.Entities.Task item)
    {
        await _context.Tasks.AddAsync(item);
        await _context.SaveChangesAsync();

        return item;
    }

    public virtual async Task<List<DAL.Entities.Task>> GetAll()
    {
        return await _context.Tasks.Include(t => t.User).ToListAsync();
    }

    public virtual async Task<int> UpdateAsync(DAL.Entities.Task item)
    {
        await _context.Tasks
            .Where(u => u.Id == item.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.CommentsList, item.CommentsList)
                .SetProperty(p => p.Deadline, item.Deadline)
                .SetProperty(p => p.Description, item.Description)
                .SetProperty(p => p.Status, item.Status)
                .SetProperty(p => p.Title, item.Title)
                .SetProperty(p => p.Title, item.Title)
                .SetProperty(p => p.User, item.User));

        await _context.SaveChangesAsync();

        return item.Id;
    }

    public virtual async Task<int> DeleteAsync(int id)
    {
        await _context.Tasks
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync();

        await _context.SaveChangesAsync();
        return id;
    }

    public virtual async Task<DAL.Entities.Task> GetById(int id)
    {
        var task = await _context.Tasks
            .Include(u => u.User)
            .FirstOrDefaultAsync(u => u.Id == id);

        return task ?? throw new Exception("Task not found");
    }

    public async Task<List<DAL.Entities.Task>> GetByUser(int userId)
    {
        return await _context.Tasks
            .Where(t => t.User.Id == userId)
            .Include(t => t.User)
            .ToListAsync();
    }
}