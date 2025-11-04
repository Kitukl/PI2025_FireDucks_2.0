using MediatR;
using StudyHub.DAL.Entities;

namespace StudyHub.BLL.Commands.Task.Create;

public record CreateCommand(int userId, string title, string description, DateTime deadline, Status status) : IRequest<int>;
