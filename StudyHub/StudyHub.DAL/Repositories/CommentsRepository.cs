using Microsoft.EntityFrameworkCore;
using StudyHub.DAL.Entities;

namespace StudyHub.DAL.Repositories;

public class CommentsRepository : IBaseRepository<Comments>
{
    private readonly StudyContext _context;

    public CommentsRepository(StudyContext context)
    {
        _context = context;
    }

    public virtual async Task<Comments> CreateAsync(Comments item)
    {
        await _context.Comments.AddAsync(item);
        await _context.SaveChangesAsync();

        return item;
    }

    public virtual async Task<List<Comments>> GetAll()
    {
        return await _context.Comments.ToListAsync();
    }

    public virtual async Task<int> UpdateAsync(Comments item)
    {
        await _context.Comments
            .Where(t => t.Id == item.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Description, item.Description)
                .SetProperty(p => p.Task, item.Task));

        await _context.SaveChangesAsync();

        return item.Id;
    }

    public virtual async Task<int> DeleteAsync(int id)
    {
        await _context.Comments
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync();

        await _context.SaveChangesAsync();
        return id;
    }

    public virtual async Task<Comments> GetById(int id)
    {
        var comment = await _context.Comments
            .FirstOrDefaultAsync(u => u.Id == id);

        return comment ?? throw new Exception("Comment not found");
    }
}