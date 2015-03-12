namespace SharpArch.EF
{
    using System;
    using System.Transactions;

    using SharpArch.Domain.PersistenceSupport;
    using System.Data.Entity;

    public class TransactionManager : ITransactionManager
    {
        private readonly DbContext context;

        private TransactionScope transaction;

        public TransactionManager(DbContext context)
        {
            this.context = context;
        }

        public IDisposable BeginTransaction()
        {
            return this.transaction ?? (this.transaction = new TransactionScope());
        }

        public void CommitTransaction()
        {
            this.context.SaveChanges();
            this.transaction.Complete();
            this.ClearTransaction();
        }

        public void RollbackTransaction()
        {
            this.ClearTransaction();
        }

        private void ClearTransaction()
        {
            this.transaction.Dispose();
            this.transaction = null;
        }
    }
}