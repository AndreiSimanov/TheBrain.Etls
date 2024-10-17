using TheBrain.Etls.Commands;
using TheBrain.Etls.Resources.Languages;

namespace TheBrain.Etls.CommandUsages;

class CreateExcelFileUsage : BaseBrainCommandUsage
{
    protected override string GetCommandName()
    {
        return nameof(CreateExcelFile);
    }

    protected override string GetCommandDescription()
    {
        return AppResources.CreateExcelFile;
    }
}