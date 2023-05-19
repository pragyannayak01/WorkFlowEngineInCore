using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlowEngine.Domain.Dapper;

namespace WorkFlowEngine.IRepository.DapperConfiguration
{
    /// <summary>
    /// Interface IRepository
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IRepositoryBase : IDisposable
    {

    }
    /// <summary>
    /// Interface IRepository
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="CAPS.Repository.Contract.IRepository" />
    public interface IRepositoryBase<out TEntity> : IRepositoryBase where TEntity : class, IEntity
    {
        /// <summary>
        /// Queries the specified SQL command.
        /// </summary>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <param name="param">The parameter.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
        IEnumerable<TEntity> Query(string sqlCommand, object param = null, CommandType? commandType = CommandType.Text);
    }

    /// <summary>
    /// Interface IEntityRepository
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="CAPS.Repository.Contract.IRepository{TEntity}" />
    public interface IEntityRepository<TEntity> : IRepositoryBase<TEntity> where TEntity : class, IEntity
    {
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
        IEnumerable<TEntity> GetAll();
        /// <summary>
        /// Finds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>TEntity.</returns>
        TEntity Find(object key);
        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>System.Int64.</returns>
        long Add(TEntity entity);
        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Update(TEntity entity);
        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Delete(TEntity entity);
    }
}
