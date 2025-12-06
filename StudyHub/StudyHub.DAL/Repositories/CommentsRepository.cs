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
        try
        {
            Console.WriteLine($"Creating comment for TaskId: {item.TaskId}, UserId: {item.UserId}");
            Console.WriteLine($"Description: {item.Description}");
            Console.WriteLine($"CreationDate: {item.CreationDate}");

            await _context.Comments.AddAsync(item);
            var changes = await _context.SaveChangesAsync();

            Console.WriteLine($"Comment saved: {changes} changes, Id: {item.Id}");

            return item;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating comment: {ex.Message}");
            Console.WriteLine($"Inner: {ex.InnerException?.Message}");
            throw;
        }
    }

    public virtual async Task<List<Comments>> GetAll()
    {
        return await _context.Comments
            .Include(c => c.User)
            .Include(c => c.Task)
            .ToListAsync();
    }

    public virtual async Task<int> UpdateAsync(Comments item)
    {
        try
        {
            var existingComment = await _context.Comments.FindAsync(item.Id);

            if (existingComment == null)
            {
                throw new Exception($"Comment with Id {item.Id} not found");
            }

            existingComment.Description = item.Description;

            _context.Comments.Update(existingComment);
            var changes = await _context.SaveChangesAsync();

            Console.WriteLine($"Comment updated: {changes} changes");

            return existingComment.Id;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating comment: {ex.Message}");
            throw;
        }
    }

    public virtual async Task<int> DeleteAsync(int id)
    {
        await _context.Comments
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync();

        return id;
    }

    public virtual async Task<Comments> GetById(int id)
    {
        var comment = await _context.Comments
            .Include(c => c.User)
            .Include(c => c.Task)
            .FirstOrDefaultAsync(c => c.Id == id);

        return comment ?? throw new Exception("Comment not found");
    }

    public virtual async Task<List<Comments>> GetByTaskId(int taskId)
    {
        return await _context.Comments
            .Include(c => c.User)
            .Where(c => c.TaskId == taskId)
            .OrderByDescending(c => c.CreationDate)
            .ToListAsync();
    }
}