using System.Linq.Expressions;

namespace Nastice.GoogleAuthenticateLab.Data.Interfaces;

public interface IRepositoryBase<TEntity> where TEntity : class
{
    /// <summary>
    /// 取得全部資料
    /// </summary>
    /// <returns></returns>
    public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate = null);

    /// <summary>
    /// 取得單筆資料
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public TEntity? Get(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// 異步取得單筆資料
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// 生成資料
    /// </summary>
    /// <param name="entity"></param>
    public void Create(TEntity entity);

    /// <summary>
    /// 生成多筆資料
    /// </summary>
    /// <param name="entities"></param>
    public void CreateRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// 更新多筆資料
    /// </summary>
    /// <param name="entity"></param>
    public void Update(TEntity entity);

    /// <summary>
    /// 保存變更
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public int SaveChanges(CancellationToken cancellationToken);

    /// <summary>
    /// 異步保存變更
    /// </summary>
    /// <returns></returns>
    public Task<int> SaveChangesAsync();
}
