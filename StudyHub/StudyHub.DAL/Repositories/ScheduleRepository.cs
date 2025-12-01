using Microsoft.EntityFrameworkCore;
using StudyHub.DAL.Entities;

namespace StudyHub.DAL.Repositories;

public class ScheduleRepository : IBaseRepository<Schedule>
{
    private readonly StudyContext _context;

    public ScheduleRepository(StudyContext context)
    {
        _context = context;
    }

    public async Task<Schedule> CreateAsync(Schedule item)
    {
        await _context.Schedules.AddAsync(item);
        await _context.SaveChangesAsync();

        return item;
    }

    public async Task<List<Schedule>> GetAll()
    {
        return await _context.Schedules
            .Include(s => s.Lessons)
                .ThenInclude(l => l.Subject)
            .Include(s => s.Lessons)
                .ThenInclude(l => l.Lecturer)
            .Include(s => s.Lessons)
                .ThenInclude(l => l.LessonSlot)
            .ToListAsync();
    }

    public async Task<int> UpdateAsync(Schedule item)
    {
        _context.Schedules.Update(item);

        await _context.SaveChangesAsync();

        return item.Id;
    }

    public async Task<int> DeleteAsync(int id)
    {
        await _context.Schedules
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync();

        await _context.SaveChangesAsync();
        return id;
    }

    public async Task<Schedule> GetById(int id)
    {
        Schedule schedule = await _context.Schedules
            .Include(s => s.Lessons)
                .ThenInclude(l => l.Subject)
            .Include(s => s.Lessons)
                .ThenInclude(l => l.Lecturer)
            .Include(s => s.Lessons)
                .ThenInclude(l => l.LessonSlot)
            .FirstOrDefaultAsync(u => u.Id == id);


        if (schedule == null)
        {
            throw new ArgumentException($"Schedule with id {id} not found");
        }

        return schedule;

    }
}