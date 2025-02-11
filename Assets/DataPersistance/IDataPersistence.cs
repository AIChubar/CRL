/// <summary>
/// Interface that should be applied for classes containing data that needs to be saved or loaded.
/// </summary>
public interface IDataPersistence
{
    void LoadData(GameData data);

    void SaveData(ref GameData data);
    
    void NewGame(GameData data);
}
