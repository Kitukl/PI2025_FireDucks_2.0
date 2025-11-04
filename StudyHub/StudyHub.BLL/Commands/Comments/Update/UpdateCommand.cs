using MediatR;
namespace StudyHub.BLL.Commands.Comments.Update;

public record UpdateCommand(DAL.Entities.Comments comment) : IRequest<int>;