namespace TheBrain.Etls;

internal static class Consts
{
    //mandatory params
    public const string COMMAND = "command";
    public const string EXCEL_FILE_PATH = "excelFilePath";
    public const string BRAINS_FOLDER_PATH = "brainsFolderPath";

    //optional params
    public const string DB_FILE_NAME = "dbFileName";
    public const string CONTENT_FILE_NAME = "contentFileName";
    public const string OLD_FORMAT_CONTENT_FOLDER_NAME = "oldFormatContentFolderName";
    public const string OLD_FORMAT_CONTENT_FILE_NAME = "oldFormatContentFileName";
    public const string LOG_FILE_PATH = "logFilePath";
    public const string LANG = "lang";

    // commands
    public const string CREATE_EXCEL_COMMAND_PARAM = "createExcelFile";
    public const string UPLOAD_FILES_COMMAND_PARAM = "uploadFilesFromExcelFile";

    // default values
    public const string DEFAULT_DB_FILE_NAME = "Brain.db";
    public const string DEFAULT_LOG_FILE_NAME = "TheBrain.Etls.log";
    public const string DEFAULT_CONTENT_FILE_NAME = "Notes.md";
    public const string DEFAULT_OLD_FORMAT_CONTENT_FOLDER_NAME = "Notes";
    public const string DEFAULT_OLD_FORMAT_CONTENT_FILE_NAME = "notes.html";
    public const string DEFAULT_LANG = "ru";

    // other
    public const string SHEET_NAME = "TheBrain";
    public const string ID_COL = "A";
    public const string NAME_COL = "B";
    public const string CONTENT_COL = "C";
    public const string PARAM_USAGE_INDENT = "    ";
}
