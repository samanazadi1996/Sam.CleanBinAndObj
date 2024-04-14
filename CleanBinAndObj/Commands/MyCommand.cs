using System.IO;
using System.Linq;

namespace CleanBinAndObj
{
    [Command(PackageIds.MyCommand)]
    internal sealed class MyCommand : BaseCommand<MyCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await CleanBinAndObjAsync();
            await ShowMessageBoxAsync();
        }

        private async Task CleanBinAndObjAsync()
        {
            var activeProject = await VS.Solutions.GetActiveProjectAsync();
            if (activeProject != null)
            {
                CleanDirectory(Path.GetDirectoryName(activeProject.FullPath));
            }
            else
            {
                var projects = await VS.Solutions.GetAllProjectsAsync();
                foreach (var project in projects)
                {
                    CleanDirectory(Path.GetDirectoryName(project.FullPath));
                }
            }
        }

        private void CleanDirectory(string directoryPath)
        {
            try
            {
                var directories = Directory.GetDirectories(directoryPath, "*", SearchOption.AllDirectories);
                var binAndObjDirectories = directories.Where(dir =>
                    Path.GetFileName(dir).Equals("obj", StringComparison.OrdinalIgnoreCase) ||
                    Path.GetFileName(dir).Equals("bin", StringComparison.OrdinalIgnoreCase)).ToArray();
                foreach (var dir in binAndObjDirectories)
                {
                    Directory.Delete(dir, true);
                }
            }
            catch { }
        }

        private async Task ShowMessageBoxAsync()
        {
            await VS.MessageBox.ShowAsync("Clean 'bin' and 'obj'", "The 'bin' and 'obj' directories have been cleaned successfully.", buttons: Microsoft.VisualStudio.Shell.Interop.OLEMSGBUTTON.OLEMSGBUTTON_OK);
        }
    }
}
