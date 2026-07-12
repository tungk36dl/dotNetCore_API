using MediatR;
using ProjectCore.Application.Dtos.Users;
using ProjectCore.Application.Mappings;
using ProjectCore.Domain.Exceptions;
using ProjectCore.Domain.Interfaces.UserRepository;

namespace ProjectCore.Application.UseCases.Users.Queries.GetUserByUserNameOrEmail;

public sealed class GetUserByUserNameEmailHandler : IRequestHandler<GetUserByUserNameOrEmailQuery, UserDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserByUserNameEmailHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(GetUserByUserNameOrEmailQuery query, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUserNameOrEmailAsync(query.UserNameOrEmail, cancellationToken)
            ?? throw new UserNotFoundException();

        return UserMapper.ToDto(user);
    }
}
