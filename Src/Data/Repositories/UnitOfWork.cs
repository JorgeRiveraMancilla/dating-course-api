using dating_course_api.Src.Interfaces;

namespace dating_course_api.Src.Data.Repositories
{
    public class UnitOfWork(
        DataContext context,
        IUserRepository userRepository,
        ILikeRepository likeRepository,
        IMessageRepository messageRepository,
        IPhotoRepository photoRepository
    ) : IUnitOfWork
    {
        public IUserRepository UserRepository => userRepository;

        public IMessageRepository MessageRepository => messageRepository;

        public ILikeRepository LikeRepository => likeRepository;
        public IPhotoRepository PhotoRepository => photoRepository;

        public async Task<bool> Complete()
        {
            return 0 < await context.SaveChangesAsync();
        }

        public bool HasChanges()
        {
            return context.ChangeTracker.HasChanges();
        }
    }
}
