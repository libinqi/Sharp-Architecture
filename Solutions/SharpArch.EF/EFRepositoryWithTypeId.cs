namespace SharpArch.EF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SharpArch.Domain.DomainModel;
    using SharpArch.Domain.PersistenceSupport;
    using SharpArch.Domain.Specifications;
    using SharpArch.EF.Contracts.Repositories;
    using System.Linq.Expressions;
    using System.Data.Entity;
    using System.Data.Entity.Core;

    public class EFRepositoryWithTypeId<T, TIdT> : IEFRepositoryWithTypeId<T, TIdT>,
        ILinqRepositoryWithTypedId<T, TIdT> where T : class
    {
        private readonly DbContext context;

        private readonly ITransactionManager transaction;

        public EFRepositoryWithTypeId(DbContext context)
        {
            this.context = context;
            this.transaction = new TransactionManager(context);
        }

        public DbContext Context
        {
            get
            {
                return this.context;
            }
        }

        public virtual ITransactionManager TransactionManager
        {
            get
            {
                return this.transaction;
            }
        }

        public T FirstOrDefault(Expression<Func<T, bool>> expression)
        {
            return All().FirstOrDefault(expression);
        }

        public IQueryable<T> All()
        {
            return Context.Set<T>().AsQueryable();
        }

        public virtual IQueryable<T> Filter(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().Where<T>(predicate).AsQueryable<T>();
        }

        public virtual IQueryable<T> Filter(Expression<Func<T, bool>> filter, out int total, int index = 0,
                                               int size = 50)
        {
            var skipCount = index * size;
            var resetSet = filter != null
                                ? Context.Set<T>().Where<T>(filter).AsQueryable()
                                : Context.Set<T>().AsQueryable();
            resetSet = skipCount == 0 ? resetSet.Take(size) : resetSet.Skip(skipCount).Take(size);
            total = resetSet.Count();
            return resetSet.AsQueryable();
        }

        public virtual T Create(T TObject)
        {
            return Context.Set<T>().Add(TObject);
        }

        public virtual void Delete(T TObject)
        {
            Context.Set<T>().Remove(TObject);
        }

        public virtual void Update(T TObject)
        {
            try
            {
                var entry = Context.Entry(TObject);
                Context.Set<T>().Attach(TObject);
                entry.State = EntityState.Modified;
            }
            catch (OptimisticConcurrencyException ex)
            {
                throw ex;
            }
        }

        public virtual int Delete(Expression<Func<T, bool>> predicate)
        {
            var objects = Filter(predicate);
            foreach (var obj in objects)
                Context.Set<T>().Remove(obj);
            return Context.SaveChanges();
        }

        public bool Contains(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().Any(predicate);
        }

        public virtual T Find(params object[] keys)
        {
            return Context.Set<T>().Find(keys);
        }

        public virtual T Find(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().FirstOrDefault<T>(predicate);
        }

        public T Get(TIdT id)
        {
            return Context.Set<T>().Find(id);
        }

        public IList<T> GetAll()
        {
            return Context.Set<T>().AsQueryable().ToList<T>();
        }

        public T SaveOrUpdate(T entity)
        {
            var entry = Context.Entry(entity);
            if (entry == null || entry.State == EntityState.Added)
            {
                Context.Set<T>().Add(entity);
            }
            else if (entry.State == EntityState.Detached)
            {
                Context.Set<T>().Attach(entity);
                entry.State = EntityState.Modified;
            }

            return entry.Entity;

        }

        public void Delete(TIdT id)
        {
            var entry = Get(id);
            if(entry!=null)
            {
                Context.Set<T>().Remove(entry);
            }
        }

        public void Save(T entity)
        {
            this.SaveOrUpdate(entity);
        }

        public void SaveAndEvict(T entity)
        {
            this.SaveOrUpdate(entity);
            Context.SaveChanges();
        }

        public T FindOne(TIdT id)
        {
            return Context.Set<T>().Find(id);
        }

        public T FindOne(ILinqSpecification<T> specification)
        {
            return specification.SatisfyingElementsFrom(this.All()).SingleOrDefault();
        }

        public IQueryable<T> FindAll()
        {
            return this.All();
        }

        public IQueryable<T> FindAll(ILinqSpecification<T> specification)
        {
            return specification.SatisfyingElementsFrom(this.All());
        }
    }
}
