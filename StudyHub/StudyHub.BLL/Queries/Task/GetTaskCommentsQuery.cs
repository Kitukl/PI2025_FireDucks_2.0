using MediatR;
using Microsoft.EntityFrameworkCore;
using StudyHub.DAL;
using StudyHub.DAL.Entities;
using StudyHub.DAL.Repositories;

namespace StudyHub.BLL.Queries.CommentQueries;

public record GetTaskCommentsQuery(int TaskId) : IRequest<List<Comments>>;

public class GetTaskCommentsQueryHandler : IRequestHandler<GetTaskCommentsQuery, List<Comments>>
{
    private readonly StudyContext _context;
    public GetTaskCommentsQueryHandler(StudyContext context)
    {
        _context = context;
    }

    public async Task<List<Comments>> Handle(GetTaskCommentsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Comments
            .Include(c => c.Task)
            .Where(c => c.Task.Id == request.TaskId)
            .ToListAsync(cancellationToken);
    }
}