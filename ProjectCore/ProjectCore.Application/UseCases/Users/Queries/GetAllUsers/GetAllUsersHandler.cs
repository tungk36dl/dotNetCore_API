using ProjectCore.Application.Dtos.Users;
using ProjectCore.Application.Mappings;
using ProjectCore.Domain.Interfaces.UserRepository;
using ProjectCore.Domain.ValueObjects.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCore.Application.UseCases.Users.Queries.GetAllUsers
{
    public sealed class GetAllUsersHandler
    {
        private readonly IUserRepository _userRepository;

        public GetAllUsersHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDto>> Handle(CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync(cancellationToken);            
            return users.Select(UserMapper.ToDto);
        }
    }
}
