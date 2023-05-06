namespace ConfigHelper
{
    public interface IMSGConfigHelper
    {
        string GedPath { get; set; }
        string MSGGenDB01 { get; set; }

        string MSGGenDB01Local { get; set; }

        string DNA_Match_File_FileName { get; set; }

        string DNA_Match_File_Path { get; set; }

        bool DNA_Match_File_IsEncrypted { get; set; }

        string FTMConString { get; set; }

        string PlaceConString { get; set; }

        string CacheData_FileName { get; set; }

        string CacheData_Path { get; set; }

        bool CacheData_IsEncrypted { get; set; }

    }


}
