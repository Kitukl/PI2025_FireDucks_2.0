using MediatR;
using StudyHub.DAL.Entities;
using StudyHub.DAL.Repositories;

namespace StudyHub.BLL.Queries.UserQueries;

public record GetUserByIdQuery(int Id) : IRequest<User>;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, User>
{
    private readonly UserRepository _userRepository;

    public GetUserByIdQueryHandler(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetById(request.Id);
    }
}