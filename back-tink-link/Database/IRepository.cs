using System.Linq.Expressions;

public interface IRepository<TEntity> : IDisposable where TEntity : class
{
    #region 'Métodos Criar/Atualizar/Excluir/Salvar'

    TEntity Create(TEntity model);

    List<TEntity> Create(List<TEntity> model);

    bool Update(TEntity model);

    bool Update(List<TEntity> model);

    bool Delete(TEntity model);

    bool Delete(params object[] Keys);

    bool Delete(Expression<Func<TEntity, bool>> where);

    int Save();

    #endregion

    #region 'Métodos de Busca'

    TEntity Find(params object[] Keys);

    TEntity Find(Expression<Func<TEntity, bool>> where);

    TEntity Find(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, object> includes);

    IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> where);

    IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, object> includes);

    #endregion

    #region 'Metodos Assíncronos'

    #region 'Métodos Criar/Atualizar/Excluir/Salvar'

    Task<TEntity> CreateAsync(TEntity model);

    Task<bool> UpdateAsync(List<TEntity> models);

    Task<bool> UpdateAsync(TEntity model);

    Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> where);

    Task<bool> DeleteAsync(TEntity model);

    Task<bool> DeleteAsync(params object[] Keys);

    Task<int> SaveAsync();

    #endregion

    #region 'Métodos de Busca'

    Task<TEntity> GetAsync(params object[] Keys);

    Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where);

    Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where, Func<IQueryable<TEntity>, object> includes);

    #endregion

    #endregion
}