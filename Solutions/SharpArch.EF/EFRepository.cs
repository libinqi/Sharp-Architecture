namespace SharpArch.EF
{
    using SharpArch.Domain.PersistenceSupport;
    using SharpArch.EF.Contracts.Repositories;
    using System.Data.Entity;

    public class EFRepository<T> : EFRepositoryWithTypeId<T, int>,
        IEFRepository<T>,
        ILinqRepository<T>
        where T : class
    {
        public EFRepository(DbContext context) : base(context) { }
    }
}
