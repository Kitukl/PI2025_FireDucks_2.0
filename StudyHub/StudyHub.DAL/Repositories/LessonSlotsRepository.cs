using Microsoft.EntityFrameworkCore;
using StudyHub.DAL.Entities;

namespace StudyHub.DAL.Repositories;

public class LessonSlotsRepository : IBaseRepository<LessonSlots>
{
    private readonly StudyContext _context;

    public LessonSlotsRepository(StudyContext context)
    {
        _context = context;
    }

    public async Task<LessonSlots> CreateAsync(LessonSlots item)
    {
        await _context.LessonSlots.AddAsync(item);
        await _context.SaveChangesAsync();

        return item;
    }

    public async Task<List<LessonSlots>> GetAll()
    {
        return await _context.LessonSlots.ToListAsync();
    }

    public async Task<int> UpdateAsync(LessonSlots item)
    {
        await _context.LessonSlots
            .Where(t => t.Id == item.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.End, item.End)
                .SetProperty(p => p.Start, item.Start)
                .SetProperty(p => p.Lessons, item.Lessons));

        await _context.SaveChangesAsync();

        return item.Id;
    }

    public async Task<int> DeleteAsync(int id)
    {
        await _context.LessonSlots
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync();

        await _context.SaveChangesAsync();
        return id;
    }

    public async Task<LessonSlots> GetById(int id)
    {
        var lessonSlots = await _context.LessonSlots
            .FirstOrDefaultAsync(u => u.Id == id);

        return lessonSlots ?? throw new Exception("Lesson slot not found");
    }
}