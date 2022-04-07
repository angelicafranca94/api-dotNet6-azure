using Manager.Domain.Entities;
using Manager.Infra.Context;
using Manager.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Manager.Infra.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    private readonly ManagerContext _context;

    public UserRepository(ManagerContext context) : base(context)
    {
        _context = context;
    }
    public async Task<User> GetByEmail(string email)
    {
        return await _context.Users
             .Where(u => u.Email.ToLower() == email.ToLower())
             .AsNoTracking()
             .FirstOrDefaultAsync();
    }
    public async Task<List<User>> SearchByEmail(string email)
    {
        return await _context.Users
             .Where(u => u.Email.ToLower().Contains(email.ToLower()))
             .AsNoTracking()
             .ToListAsync();
    }

    public async Task<List<User>> SearchByName(string name)
    {
        return await _context.Users
            .Where(u => u.Name.ToLower().Contains(name.ToLower()))
            .AsNoTracking()
            .ToListAsync();
    }

}
