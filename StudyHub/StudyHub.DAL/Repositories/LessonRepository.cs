using Microsoft.EntityFrameworkCore;
using StudyHub.DAL.Entities;

namespace StudyHub.DAL.Repositories;

public class LessonRepository : IBaseRepository<Lesson>
{
    private readonly StudyContext _context;

    public LessonRepository(StudyContext context)
    {
        _context = context;
    }

    public async Task<Lesson> CreateAsync(Lesson item)
    {
        await _context.Lessons.AddAsync(item);
        await _context.SaveChangesAsync();

        return item;
    }

    public async Task<List<Lesson>> GetAll()
    {
        return await _context.Lessons.ToListAsync();
    }

    public async Task<int> UpdateAsync(Lesson item)
    {
        await _context.Lessons
            .Where(t => t.Id == item.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Schedules, item.Schedules)
                .SetProperty(p => p.Lecturer, item.Lecturer)
                .SetProperty(p => p.LessonSlot, item.LessonSlot)
                .SetProperty(p => p.Subject, item.Subject));

        await _context.SaveChangesAsync();

        return item.Id;
    }

    public async Task<int> DeleteAsync(int id)
    {
        await _context.Lessons
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync();

        await _context.SaveChangesAsync();
        return id;
    }

    public async Task<Lesson> GetById(int id)
    {
        var lesson = await _context.Lessons
            .FirstOrDefaultAsync(u => u.Id == id);

        return lesson ?? throw new Exception("Lesson not found");
    }
}