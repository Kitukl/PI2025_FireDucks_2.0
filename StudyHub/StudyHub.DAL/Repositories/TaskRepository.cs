using Microsoft.EntityFrameworkCore;
using StudyHub.DAL.Entities;

namespace StudyHub.DAL.Repositories
{
    public class TaskRepository : IBaseRepository<DAL.Entities.Task>
    {
        private readonly StudyContext _context;

        public TaskRepository(StudyContext context)
        {
            _context = context;
        }

        public virtual async System.Threading.Tasks.Task<DAL.Entities.Task> CreateAsync(DAL.Entities.Task item)
        {
            try
            {
                Console.WriteLine($"TaskRepository.CreateAsync called");
                Console.WriteLine($"  UserId: {item.UserId}");
                Console.WriteLine($"  Title: {item.Title}");
                Console.WriteLine($"  Description: {item.Description}");
                Console.WriteLine($"  Deadline: {item.Deadline}");
                Console.WriteLine($"  Status: {item.Status}");
                Console.WriteLine($"  CreationDate: {item.CreationDate}");

                await _context.Tasks.AddAsync(item);
                Console.WriteLine("Calling SaveChangesAsync...");
                await _context.SaveChangesAsync();
                Console.WriteLine($"Task saved successfully with Id: {item.Id}");

                return item;
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"DbUpdateException: {dbEx.Message}");
                Console.WriteLine($"Inner exception: {dbEx.InnerException?.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General exception: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                throw;
            }
        }

        public virtual async System.Threading.Tasks.Task<List<DAL.Entities.Task>> GetAll()
        {
            return await _context.Tasks
                .Include(t => t.User)
                .ToListAsync();
        }

        public virtual async System.Threading.Tasks.Task<int> UpdateAsync(DAL.Entities.Task item)
        {
            try
            {
                Console.WriteLine($"=== TaskRepository.UpdateAsync ===");
                Console.WriteLine($"Id: {item.Id}");
                Console.WriteLine($"Title: {item.Title}");
                Console.WriteLine($"Description: {item.Description}");
                Console.WriteLine($"Status: {item.Status}");
                Console.WriteLine($"Deadline: {item.Deadline}");

                var existingTask = await _context.Tasks.FindAsync(item.Id);

                if (existingTask == null)
                {
                    throw new Exception($"Task with Id {item.Id} not found");
                }

                existingTask.Title = item.Title;
                existingTask.Description = item.Description;
                existingTask.Deadline = item.Deadline;
                existingTask.Status = item.Status;

                _context.Tasks.Update(existingTask);
                var changes = await _context.SaveChangesAsync();

                Console.WriteLine($"✅ Changes saved: {changes}");

                return existingTask.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error updating task: {ex.Message}");
                Console.WriteLine($"Inner: {ex.InnerException?.Message}");
                throw;
            }
        }

        public virtual async System.Threading.Tasks.Task<int> DeleteAsync(int id)
        {
            await _context.Tasks
                .Where(t => t.Id == id)
                .ExecuteDeleteAsync();
            return id;
        }

        public virtual async System.Threading.Tasks.Task<DAL.Entities.Task> GetById(int id)
        {
            var task = await _context.Tasks
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);
            return task ?? throw new Exception("Task not found");
        }

        public async System.Threading.Tasks.Task<List<DAL.Entities.Task>> GetByUser(int userId)
        {
            return await _context.Tasks
                .Include(t => t.User)
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }
    }
}