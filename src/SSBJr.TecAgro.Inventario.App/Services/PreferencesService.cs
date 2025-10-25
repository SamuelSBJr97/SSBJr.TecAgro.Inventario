namespace SSBJr.TecAgro.Inventario.App.Services;

public class PreferencesService : IPreferencesService
{
    private const string TokenKey = "auth_token";
    private const string ServerUrlKey = "server_url";
    private const string UserIdKey = "user_id";
    private const string UserNameKey = "user_name";
    
  // URLs para diferentes plataformas quando usando Docker
    private static string GetDefaultServerUrl()
    {
#if ANDROID
        // Android Emulator usa IP especial para acessar localhost do host
      return "http://10.0.2.2:5000";
#elif IOS
      // iOS Simulator pode usar localhost
      return "http://localhost:5000";
#elif WINDOWS
     // Windows usa localhost
      return "http://localhost:5000";
#elif MACCATALYST
        // MacCatalyst usa localhost
        return "http://localhost:5000";
#else
  return "http://localhost:5000";
#endif
    }

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
        // Se já tiver uma URL salva, usa ela
    var savedUrl = Preferences.Get(ServerUrlKey, null);
        if (!string.IsNullOrEmpty(savedUrl))
        {
      return savedUrl;
     }
        
        // Caso contrário, usa a URL padrão para a plataforma
    var defaultUrl = GetDefaultServerUrl();
        
        // Salva a URL padrão para próximas execuções
        SaveServerUrl(defaultUrl);
        
        return defaultUrl;
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
