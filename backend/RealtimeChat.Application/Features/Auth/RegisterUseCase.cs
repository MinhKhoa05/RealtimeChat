using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Auth;

public class RegisterUseCase
{
    private readonly IAppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUseCase(IAppDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task ExecuteAsync(RegisterRequest request, CancellationToken ct)
    {
        if (await _context.Users.AnyAsync(x=> x.Email == request.Email, ct))
        {
            throw new Exception("Email already exists.");
        }

        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = passwordHash,
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(ct);
    }
}

public class RegisterRequest
{
    public string Name {get; set;} = null!;
    public string Email {get; set;} = null!;
    public string Password {get; set;} = null!;
}