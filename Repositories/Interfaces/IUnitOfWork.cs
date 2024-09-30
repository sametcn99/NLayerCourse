namespace App.Repositories.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}
