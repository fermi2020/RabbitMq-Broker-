namespace BackOffice.Messaging.Send
{
    public interface IUpdateSender
    {
        void SendEntity<TEntity>(TEntity entity);
    }
}