using MediatR;
using StudyHub.DAL.Repositories;
using StudyHub.DAL.Entities;

namespace StudyHub.BLL.Commands.SupportTickets.Create; 

public class CreateCommandHandler : IRequestHandler<CreateCommand, int>
{
    private readonly SupportTicketRepository _ticketRepository;
    private readonly CategoryRepository _categoryRepository;
    private readonly UserRepository _userRepository;

    public CreateCommandHandler(
        SupportTicketRepository ticketRepository,
        CategoryRepository categoryRepository,
        UserRepository userRepository)
    {
        _ticketRepository = ticketRepository;
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
    }

    public async Task<int> Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var categories = await _categoryRepository.GetAll();
            var category = categories.FirstOrDefault(c => c.Name == request.CategoryName);

            if (category == null)
            {
                category = new Category
                {
                    Name = request.CategoryName,
                    Tickets = new List<SupportTicket>() 
                };
                category = await _categoryRepository.CreateAsync(category);
            }

            var user = await _userRepository.GetById(request.UserId);

            var ticket = new SupportTicket 
            {
                Description = $"Subject: {request.Subject}\n\n{request.Description}",
                Type = request.Type,
                Category = category,
                User = user
            };

            var createdTicket = await _ticketRepository.CreateAsync(ticket);

            Console.WriteLine($"Support ticket created: Id={createdTicket.Id}");

            return createdTicket.Id;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating support ticket: {ex.Message}");
            throw;
        }
    }
}