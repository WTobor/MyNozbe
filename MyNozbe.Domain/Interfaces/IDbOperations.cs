namespace MyNozbe.Domain.Interfaces
{
    public interface IDbOperations <T>
    {
        int Add(T model);

        void Update(T model);

        T Get(int taskId);
    }
}