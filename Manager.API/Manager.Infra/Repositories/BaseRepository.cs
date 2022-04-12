using Manager.Domain.Entities;
using Manager.Infra.Context;
using Manager.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Manager.Infra.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : Base
{
    private readonly ManagerContext _context;

    public BaseRepository(ManagerContext context)
    {
        _context = context;
    }

    public virtual async Task<T> Create(T entity)
    {
        _context.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> Update(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task Delete(long id)
    {
        var obj = await GetById(id);

        if (obj != null)
        {
            _context.Remove(obj);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<T> GetById(long id)
    {
        return await _context.Set<T>()
              .AsNoTracking()
              .Where(x => x.Id == id)
              .FirstOrDefaultAsync();

    }

    public virtual async Task<List<T>> GetAll()
    {
        return await _context.Set<T>()
            .AsNoTracking()
            .ToListAsync();
    }




}
