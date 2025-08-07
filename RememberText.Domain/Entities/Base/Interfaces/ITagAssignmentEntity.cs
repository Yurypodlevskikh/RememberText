namespace RememberText.Domain.Entities.Base.Interfaces
{
    public interface ITagAssignmentEntity
    {
        int TextId { get; set; }
        int TagId { get; set; }
    }
}
