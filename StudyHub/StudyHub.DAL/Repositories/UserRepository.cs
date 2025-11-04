using Microsoft.EntityFrameworkCore;
using StudyHub.DAL.Entities;

namespace StudyHub.DAL.Repositories;

public class UserRepository : IBaseRepository<User>
{
    private readonly StudyContext _context;

    public UserRepository(StudyContext context)
    {
        _context = context;
    }

    public async Task<User> CreateAsync(User item)
    {
        await _context.Users.AddAsync(item);
        await _context.SaveChangesAsync();

        return item;
    }

    public async Task<User> GetByEmail(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(e => e.Email == email) ?? throw new Exception("Not found user with this email");
    }
    public async Task<List<User>> GetAll()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<int> UpdateAsync(User item)
    {
        await _context.Users
            .Where(u => u.Id == item.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.DaysForNotification, item.DaysForNotification)
                .SetProperty(p => p.Email, item.Email)
                .SetProperty(p => p.IsNotified, item.IsNotified)
                .SetProperty(p => p.Name, item.Name)
                .SetProperty(p => p.Password, item.Password)
                .SetProperty(p => p.Schedule, item.Schedule)
                .SetProperty(p => p.Surname, item.Surname)
                .SetProperty(p => p.Tickets, item.Tickets)
                .SetProperty(p => p.Tasks, item.Tasks));

        await _context.SaveChangesAsync();

        return item.Id;
    }

    public async Task<int> DeleteAsync(int id)
    {
        await _context.Users
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync();

        await _context.SaveChangesAsync();
        return id;
    }

    public async Task<User> GetById(int id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        return user ?? throw new Exception("User not found");
    }
}