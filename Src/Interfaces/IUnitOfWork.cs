namespace dating_course_api.Src.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IMessageRepository MessageRepository { get; }
        ILikeRepository LikeRepository { get; }
        IPhotoRepository PhotoRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}
