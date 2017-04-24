using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.AccessCache;

namespace NotePad_Sharp.Services
{
    public class FileService
    {
        public async Task SaveAsync(Models.FileInfo model)
        {
            if (model != null)
                await Windows.Storage.FileIO.WriteTextAsync(model.Ref, model.Text);
        }

        public async Task<Models.FileInfo> LoadAsync(Windows.Storage.StorageFile file)
        {
            if(Windows.UI.StartScreen.JumpList.IsSupported())
            {
                var jList = await Windows.UI.StartScreen.JumpList.LoadCurrentAsync();
                jList.SystemGroupKind = Windows.UI.StartScreen.JumpListSystemGroupKind.None;

                while (jList.Items.Count() > 4)
                    jList.Items.RemoveAt(jList.Items.Count() - 1);
                if(!jList.Items.Any(x => x.Arguments == file.Path))
                {
                    var jItem = Windows.UI.StartScreen.JumpListItem.CreateWithArguments(file.Path, file.DisplayName);
                    jList.Items.Add(jItem);
                }
                await jList.SaveAsync();
            }

            var mruList = StorageApplicationPermissions.MostRecentlyUsedList;
            while (mruList.Entries.Count() >= mruList.MaximumItemsAllowed)
                mruList.Remove(mruList.Entries.First().Token);
            if (!mruList.Entries.Any(x => x.Metadata == file.Path))
                mruList.Add(file, file.Path);
            var futureAccessList = StorageApplicationPermissions.FutureAccessList;
            futureAccessList.Add(file);

            return new Models.FileInfo
            {
                Text = await Windows.Storage.FileIO.ReadTextAsync(file),
                Name = file.DisplayName,
                Ref = file,
            };
        }
    }
}
