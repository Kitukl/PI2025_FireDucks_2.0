using Microsoft.EntityFrameworkCore;
using StudyHub.DAL.Entities;

namespace StudyHub.DAL.Repositories;

public class LecturerRepository : IBaseRepository<Lecturer>
{
    private readonly StudyContext _context;

    public LecturerRepository(StudyContext context)
    {
        _context = context;
    }

    public async Task<Lecturer> CreateAsync(Lecturer item)
    {
        await _context.Lecturers.AddAsync(item);
        await _context.SaveChangesAsync();

        return item;
    }

    public async Task<List<Lecturer>> GetAll()
    {
        return await _context.Lecturers.ToListAsync();
    }

    public async Task<int> UpdateAsync(Lecturer item)
    {
        await _context.Lecturers
            .Where(t => t.Id == item.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Lessons, item.Lessons)
                .SetProperty(p => p.Name, item.Name)
                .SetProperty(p => p.Surname, item.Surname));

        await _context.SaveChangesAsync();

        return item.Id;
    }

    public async Task<int> DeleteAsync(int id)
    {
        await _context.Lecturers
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync();

        await _context.SaveChangesAsync();
        return id;
    }

    public async Task<Lecturer> GetById(int id)
    {
        var lecturer = await _context.Lecturers
            .FirstOrDefaultAsync(u => u.Id == id);

        return lecturer ?? throw new Exception("Lecturer not found");
    }
}