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
        return await _context.Schedules.ToListAsync();
    }

    public async Task<int> UpdateAsync(Schedule item)
    {
        await _context.Schedules
            .Where(t => t.Id == item.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Users, item.Users)
                .SetProperty(p => p.Lessons, item.Lessons));

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
        var schedule = await _context.Schedules
            .FirstOrDefaultAsync(u => u.Id == id);

        return schedule ?? throw new Exception("Schedule not found");
    }
}