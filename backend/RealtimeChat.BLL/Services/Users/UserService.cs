using RealtimeChat.BLL.Contracts;
using RealtimeChat.BLL.Exceptions;
using RealtimeChat.DAL.Entities;
using RealtimeChat.DAL.Interfaces;

namespace RealtimeChat.BLL.Services.Users;

public class UserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository userRepository)
    {
        _repository = userRepository;
    }

    public async Task<User> GetByIdOrThrowAsync(long id)
    {
        return await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException(ErrorCode.UserNotFound);
    }
}
