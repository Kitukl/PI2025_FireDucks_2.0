using Microsoft.EntityFrameworkCore;
using StudyHub.DAL.Entities;

namespace StudyHub.DAL.Repositories;

public class SubjectRepository : IBaseRepository<Subject>
{
    private readonly StudyContext _context;

    public SubjectRepository(StudyContext context)
    {
        _context = context;
    }

    public async Task<Subject> CreateAsync(Subject item)
    {
        await _context.Subjects.AddAsync(item);
        await _context.SaveChangesAsync();

        return item;
    }

    public async Task<List<Subject>> GetAll()
    {
        return await _context.Subjects.ToListAsync();
    }

    public async Task<int> UpdateAsync(Subject item)
    {
        await _context.Subjects
            .Where(t => t.Id == item.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Name, item.Name)
                .SetProperty(p => p.Lessons, item.Lessons));

        await _context.SaveChangesAsync();

        return item.Id;
    }

    public async Task<int> DeleteAsync(int id)
    {
        await _context.Subjects
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync();

        await _context.SaveChangesAsync();
        return id;
    }

    public async Task<Subject> GetById(int id)
    {
        var subject = await _context.Subjects
            .FirstOrDefaultAsync(u => u.Id == id);

        return subject ?? throw new Exception("Subject not found");
    }
}