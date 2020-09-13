using Rsa.Repository.Abstractions;
using System.Threading.Tasks;

namespace Rsa.Repository.Implementation
{
    internal class UnitOfWork : IUnitOfWork
    {
        public RsaContext RsaContext { get; }
        public UnitOfWork(RsaContext rsaContext)
        {
            RsaContext = rsaContext;
        }
        public async Task Commit()
        {
            await RsaContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            RsaContext.Dispose();
        }
    }
}
