namespace SSBJr.TecAgro.Inventario.App.Services;

public class PreferencesService : IPreferencesService
{
    private const string TokenKey = "auth_token";
    private const string ServerUrlKey = "server_url";
private const string UserIdKey = "user_id";
    private const string UserNameKey = "user_name";
    private const string DefaultServerUrl = "http://localhost:5000";

    public void SaveToken(string token)
    {
  Preferences.Set(TokenKey, token);
    }

    public string? GetToken()
    {
   return Preferences.Get(TokenKey, null);
    }

    public void ClearToken()
 {
        Preferences.Remove(TokenKey);
 Preferences.Remove(UserIdKey);
 Preferences.Remove(UserNameKey);
    }

    public void SaveServerUrl(string url)
    {
        Preferences.Set(ServerUrlKey, url);
    }

    public string GetServerUrl()
    {
      return Preferences.Get(ServerUrlKey, DefaultServerUrl);
}

    public void SaveUserId(Guid userId)
    {
        Preferences.Set(UserIdKey, userId.ToString());
    }

    public Guid? GetUserId()
    {
   var userIdString = Preferences.Get(UserIdKey, null);
    if (Guid.TryParse(userIdString, out var userId))
        {
     return userId;
        }
        return null;
    }

 public void SaveUserName(string userName)
    {
   Preferences.Set(UserNameKey, userName);
    }

    public string? GetUserName()
    {
      return Preferences.Get(UserNameKey, null);
    }
}
