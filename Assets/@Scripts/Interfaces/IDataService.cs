using System.Collections.Generic;

public interface IDataService 
{
    GameData Save(GameData data, bool overwrite = true);
    GameData Load(string name);
    void Delete(string name);
    void DeleteAll();
    bool Contains(string name);
    IEnumerable<string> ListSaves();
}
