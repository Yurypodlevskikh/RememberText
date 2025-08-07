namespace RememberText.Domain.Entities.Base.Interfaces
{
    public interface IRowVersionEntity : IBaseEntity
    {
        byte[] RowVersion { get; set; }
    }
}
