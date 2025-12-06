namespace StudyHub.BLL.Services
{
    public class AuthStateService
    {
        public int? UserId { get; private set; }
        public string? UserName { get; private set; }
        public string? UserSurname { get; private set; }
        public string? UserEmail { get; private set; }
        public string? UserGroupName { get; private set; }
        public bool IsAuthenticated => UserId.HasValue;

        public event Action? OnAuthStateChanged;

        public void SetAuthenticatedUser(int userId, string name, string surname, string email, string groupName)
        {
            UserId = userId;
            UserName = name;
            UserSurname = surname;
            UserEmail = email;
            UserGroupName = groupName;
            OnAuthStateChanged?.Invoke();
        }

        public void Logout()
        {
            UserId = null;
            UserName = null;
            UserSurname = null;
            UserEmail = null;
            UserGroupName = null;
            OnAuthStateChanged?.Invoke();
        }
    }
}