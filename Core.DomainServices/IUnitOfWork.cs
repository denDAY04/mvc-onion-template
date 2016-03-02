using System.Threading.Tasks;

namespace Core.DomainServices
{
    public interface IUnitOfWork
    {
        // Repositories for the different models 
        IStudentRepository StudentRepository { get; }

        int Save();
        Task<int> SaveAsync();
    }
}
