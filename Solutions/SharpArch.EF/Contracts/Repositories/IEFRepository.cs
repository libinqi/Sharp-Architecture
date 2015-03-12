namespace SharpArch.EF.Contracts.Repositories
{
    using SharpArch.Domain.PersistenceSupport;
    public interface IEFRepository<T> : IEFRepositoryWithTypeId<T, int>, IRepository<T> where T :class
    {
    }
}
