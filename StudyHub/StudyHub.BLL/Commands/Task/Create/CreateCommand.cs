using MediatR;
using StudyHub.DAL.Entities;

namespace StudyHub.BLL.Commands.Task.Create
{
    public record CreateCommand(
        int UserId,
        string Title,
        string Description,
        DateTime Deadline,
        Status Status
    ) : IRequest<int>;
}