using System;
using System.Threading.Tasks;

namespace Rsa.Repository.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        RsaContext RsaContext { get; }
        Task Commit();
    }
}
