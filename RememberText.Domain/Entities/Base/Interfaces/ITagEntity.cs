namespace RememberText.Domain.Entities.Base.Interfaces
{
    public interface ITagEntity : IRowVersionEntity
    {
        string TagName { get; set; }
        int NormalizedTagId { get; set; }
    }
}
