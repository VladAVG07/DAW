using Lab06.Data;
using Lab06.Models;

namespace Lab06.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Users.GetAllAsync(cancellationToken);
    }
}