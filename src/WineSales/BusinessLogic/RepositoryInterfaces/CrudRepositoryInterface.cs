namespace WineSales.BusinessLogic.RepositoryInterfaces
{
    public interface CrudRepositoryInterface<T>
    {
        void Create(T entity);
        List<T> Read(int id);
        void Update(T entity);
        void Delete(T entity);
    }
}
