using MediatR;

namespace StudyHub.BLL.Commands.SupportTickets.Create;

public record CreateCommand(
    int UserId,
    string Type,
    string CategoryName,
    string Subject,
    string Description
) : IRequest<int>;