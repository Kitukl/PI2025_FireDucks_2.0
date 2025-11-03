using Microsoft.EntityFrameworkCore;
using StudyHub.DAL.Entities;

namespace StudyHub.DAL.Repositories;

public class CategoryRepository : IBaseRepository<Category>
{
    private readonly StudyContext _context;

    public CategoryRepository(StudyContext context)
    {
        _context = context;
    }

    public async Task<Category> CreateAsync(Category item)
    {
        await _context.Categories.AddAsync(item);
        await _context.SaveChangesAsync();

        return item;
    }

    public async Task<List<Category>> GetAll()
    {
        return await _context.Categories.ToListAsync();
    }

    public async Task<int> UpdateAsync(Category item)
    {
        await _context.Categories
            .Where(t => t.Id == item.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Tickets, item.Tickets)
                .SetProperty(p => p.Name, item.Name));

        await _context.SaveChangesAsync();

        return item.Id;
    }

    public async Task<int> DeleteAsync(int id)
    {
        await _context.Categories
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync();

        await _context.SaveChangesAsync();
        return id;
    }

    public async Task<Category> GetById(int id)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(u => u.Id == id);

        return category ?? throw new Exception("Category not found");
    }
}