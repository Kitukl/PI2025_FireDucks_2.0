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

    public async Task<DAL.Entities.Task> CreateAsync(DAL.Entities.Task item)
    {
        await _context.Tasks.AddAsync(item);
        await _context.SaveChangesAsync();

        return item;
    }

    public async Task<List<DAL.Entities.Task>> GetAll()
    {
        return await _context.Tasks.ToListAsync();
    }

    public async Task<int> UpdateAsync(DAL.Entities.Task item)
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

    public async Task<int> DeleteAsync(int id)
    {
        await _context.Tasks
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync();

        await _context.SaveChangesAsync();
        return id;
    }

    public async Task<DAL.Entities.Task> GetById(int id)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(u => u.Id == id);

        return task ?? throw new Exception("Task not found");
    }
}