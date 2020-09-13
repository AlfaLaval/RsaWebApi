using Microsoft.Extensions.Logging;
using Rsa.Repository.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rsa.Repository.Implementation
{
    internal class Repository : IRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<Repository> _logger;
        public Repository(IUnitOfWork unitOfWork,ILogger<Repository> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public void Add<T>(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(Add)} entity must not be null");
            }

            _unitOfWork.RsaContext.Add(entity);

        }

        public IQueryable<T> GetAll<T>()
        {
            return _unitOfWork.RsaContext.Set<T>();
        }

        public void Update<T>(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(Update)} entity must not be null");
            }

            _unitOfWork.RsaContext.Update(entity);

        }
    }
}
