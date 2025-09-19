namespace ECFautoecole.Interfaces
{
    public interface IRepository<T>
    {
        List<T> GetAll();
        T? GetById(int id);
        T? Add(T model);
        bool Update(T model);
        bool Delete(int id);
    }
}
