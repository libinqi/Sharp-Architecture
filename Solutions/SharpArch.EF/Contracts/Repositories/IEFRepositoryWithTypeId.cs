namespace SharpArch.EF.Contracts.Repositories
{
    using System;
    using System.Collections.Generic;
    using SharpArch.Domain.PersistenceSupport;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Data.Entity;

    public interface IEFRepositoryWithTypeId<T, TIdT> : IRepositoryWithTypedId<T, TIdT> where T : class
    {
        DbContext Context { get; }
        IQueryable<T> All();
        IQueryable<T> Filter(Expression<Func<T, bool>> predicate);
        IQueryable<T> Filter(Expression<Func<T, bool>> filter, out int total, int index = 0, int size = 50);
        bool Contains(Expression<Func<T, bool>> predicate);
        T Find(params object[] keys);
        T Find(Expression<Func<T, bool>> predicate);
        T Create(T t);
        void Delete(T t);
        int Delete(Expression<Func<T, bool>> predicate);
        void Update(T t);
        T FirstOrDefault(Expression<Func<T, bool>> expression);
    }
}
