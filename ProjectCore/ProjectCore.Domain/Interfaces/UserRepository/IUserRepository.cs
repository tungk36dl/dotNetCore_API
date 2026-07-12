using ProjectCore.Domain.Entities;
using ProjectCore.Domain.ValueObjects.User;

namespace ProjectCore.Domain.Interfaces.UserRepository;

public interface IUserRepository
{
    // Dùng CancellationToken giúp hủy bỏ thao tác khi không cần thiết nữa (như khi timeout, hủy request...) giúp tránh lãng phí tài nguyên.
    // CancellationToken có thể được truyền từ tầng trên (như Application Layer) xuống để kiểm soát vòng đời của các thao tác bất đồng bộ.
    // Mặc định CancellationToken.None nếu không được cung cấp.
    // Tham khảo thêm về CancellationToken: https://learn.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads
    // Chỉ dùng CancellationToken trong các phương thức async, không dùng trong các phương thức đồng bộ.
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<User?> GetByUserNameAsync(
        UserName userName,
        CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(
        Email email,
        CancellationToken cancellationToken = default);

    Task<User?> GetByUserNameOrEmailAsync(
        string userNameOrEmail,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(
        Email email,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByUserNameAsync(
        UserName userName,
        CancellationToken cancellationToken = default);


    Task AddAsync(User user, CancellationToken cancellationToken = default);


    // Các hàm này chỉ mang tính đánh dấu thay đổi trạng thái của Entity trong DbContext,
    // chứ chưa  thực hiện thao tác gì với DB nên không cần async, CancellationToken
    void Update(User user);

    void Remove(User user);
    
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<(IEnumerable<User> Data, int TotalCount)> GetDataAsync(UserSearch seach, CancellationToken cancellationToken = default);
}
