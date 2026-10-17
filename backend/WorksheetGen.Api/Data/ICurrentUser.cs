namespace WorksheetGen.Api.Data
{
    public interface ICurrentUser
    {
        Guid Id { get; }
    }

    public class DevCurrentUser : ICurrentUser
    {
        public Guid Id => DevUser.Id;
    }
}
