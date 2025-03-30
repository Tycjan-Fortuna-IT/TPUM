namespace Data.API
{
    public interface IDataRepository<T> where T : IIdentifiable
    {
        public abstract IEnumerable<T> GetAll();

        public abstract T? Get(Guid id);

        public abstract void Add(T item);

        public abstract bool RemoveById(Guid id);

        public abstract bool Remove(T item);

        public abstract bool Update(Guid id, T item);
    }
}
