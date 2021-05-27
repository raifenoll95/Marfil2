using System.ComponentModel.DataAnnotations;
using DevExpress.Web;
using DevExpress.Web.Mvc;

namespace Marfil.Dom.Persistencia.Model.Ficheros
{
    public class GaleriaFicherosModel
    {
        FileManagerSettingsEditing _settingsEditing;
        FileManagerSettingsToolbar _settingsToolbar;
        FileManagerSettingsFolders _settingsFolders;
        FileManagerSettingsFileList _settingsFileList;
        FileManagerSettingsBreadcrumbs _settingsBreadcrumbs;
        MVCxFileManagerSettingsUpload _settingsUpload;

        public GaleriaFicherosModel()
        {
            _settingsEditing = new FileManagerSettingsEditing(null)
            {
                AllowCreate = true,
                AllowMove = true,
                AllowDelete = true,
                AllowRename = true,
                AllowCopy = true,
                AllowDownload = true
            };
            _settingsToolbar = new FileManagerSettingsToolbar(null)
            {
                ShowPath = true,
                ShowFilterBox = true
            };
            _settingsFolders = new FileManagerSettingsFolders(null)
            {
                Visible = true,
                EnableCallBacks = false,
                ShowFolderIcons = true,
                ShowLockedFolderIcons = true
            };
            _settingsFileList = new FileManagerSettingsFileList(null)
            {
                ShowFolders = true,
                ShowParentFolder = true
            };
            _settingsBreadcrumbs = new FileManagerSettingsBreadcrumbs(null)
            {
                Visible = true,
                ShowParentFolderButton = true,
                Position = BreadcrumbsPosition.Top
            };
            _settingsUpload = new MVCxFileManagerSettingsUpload();
            _settingsUpload.Enabled = true;
            _settingsUpload.AdvancedModeSettings.EnableMultiSelect = true;
        }

        [Display(Name = "Settings Editing")]
        public FileManagerSettingsEditing SettingsEditing { get { return _settingsEditing; } }
        [Display(Name = "Settings Toolbar")]
        public FileManagerSettingsToolbar SettingsToolbar { get { return _settingsToolbar; } }
        [Display(Name = "Settings Folders")]
        public FileManagerSettingsFolders SettingsFolders { get { return _settingsFolders; } }
        [Display(Name = "Settings FileList")]
        public FileManagerSettingsFileList SettingsFileList { get { return _settingsFileList; } }
        [Display(Name = "Settings Breadcrumbs")]
        public FileManagerSettingsBreadcrumbs SettingsBreadcrumbs { get { return _settingsBreadcrumbs; } }
        [Display(Name = "Settings Upload")]
        public MVCxFileManagerSettingsUpload SettingsUpload { get { return _settingsUpload; } }
    }
}
