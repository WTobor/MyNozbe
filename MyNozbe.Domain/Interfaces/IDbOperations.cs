namespace MyNozbe.Domain.Interfaces
{
    public interface IDbOperations <T>
    {
        T Create(T model);

        void Update(T model);

        T Get(int taskId);
    }
}