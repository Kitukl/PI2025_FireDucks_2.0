using Microsoft.EntityFrameworkCore;
using StudyHub.DAL.Entities;

namespace StudyHub.DAL.Repositories;

public class SupportTicketRepository : IBaseRepository<SupportTicket>
{
    private readonly StudyContext _context;

    public SupportTicketRepository(StudyContext context)
    {
        _context = context;
    }

    public async Task<SupportTicket> CreateAsync(SupportTicket item)
    {
        await _context.Tickets.AddAsync(item);
        await _context.SaveChangesAsync();

        return item;
    }

    public async Task<List<SupportTicket>> GetAll()
    {
        return await _context.Tickets.ToListAsync();
    }

    public async Task<int> UpdateAsync(SupportTicket item)
    {
        await _context.Tickets
            .Where(t => t.Id == item.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Description, item.Description)
                .SetProperty(p => p.Type, item.Type)
                .SetProperty(p => p.Category, item.Category)
                .SetProperty(p => p.User, item.User));

        await _context.SaveChangesAsync();

        return item.Id;
    }

    public async Task<int> DeleteAsync(int id)
    {
        await _context.Tickets
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync();

        await _context.SaveChangesAsync();
        return id;
    }

    public async Task<SupportTicket> GetById(int id)
    {
        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(u => u.Id == id);

        return ticket ?? throw new Exception("Ticket not found");
    }
}