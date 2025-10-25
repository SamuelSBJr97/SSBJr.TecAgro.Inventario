namespace SSBJr.TecAgro.Inventario.App.Services;

public interface IPreferencesService
{
    void SaveToken(string token);
    string? GetToken();
    void ClearToken();
  void SaveServerUrl(string url);
    string GetServerUrl();
    void SaveUserId(Guid userId);
 Guid? GetUserId();
 void SaveUserName(string userName);
    string? GetUserName();
}
