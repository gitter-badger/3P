﻿#region header
// ========================================================================
// Copyright (c) 2015 - Julien Caillon (julien.caillon@gmail.com)
// This file (FileExplorer.cs) is part of 3P.
// 
// 3P is a free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// 3P is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with 3P. If not, see <http://www.gnu.org/licenses/>.
// ========================================================================
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using _3PA.Images;
using _3PA.Interop;
using _3PA.Lib;
using _3PA.MainFeatures.NppInterfaceForm;

namespace _3PA.MainFeatures.FileExplorer {
    public class FileExplorer {

        #region fields

        /// <summary>
        /// Form accessor
        /// </summary>
        public static FileExplorerForm Form { get; private set; }

        /// <summary>
        /// Does the form exists and is visible?
        /// </summary>
        public static bool IsVisible {
            get { return Form != null && Form.Visible; }
        }

        #endregion

        #region handling form

        /// <summary>
        /// Use this to redraw the docked form
        /// </summary>
        public static void ApplyColorSettings() {
            if (Form == null) return;
            Form.StyleOvlTree();
            Form.Refresh();
        }

        /// <summary>
        /// Just redraw the file explorer ovl list, it is used to update the "selected" scope when
        /// the user changes the current document
        /// </summary>
        public static void RedrawFileExplorerList() {
            if (Form == null) return;
            Form.Redraw();
        }

        #endregion

        #region public methods

        /// <summary>
        /// Refresh the files list
        /// </summary>
        public static void RebuildItemList() {
            if (!IsVisible) return;
            Form.RefreshOvl();
            Form.FilterByText = "";
        }

        /// <summary>
        /// Start a new search for files
        /// </summary>
        public static void StartSearch() {
            try {
                if (Form == null) return;
                Form.ClearFilter();
                Form.GiveFocustoTextBox();
            } catch (Exception e) {
                ErrorHandler.ShowErrors(e, "Error in StartSearch");
            }
        }

        /// <summary>
        /// Add each files/folders of a given path to the output List of FileObject,
        /// can be set to be recursive,
        /// can be set to not add the subfolders in the results
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="recursive"></param>
        /// <param name="includeFolders"></param>
        /// <returns></returns>
        public static List<FileObject> ListFileOjectsInDirectory(string dirPath, bool recursive = true, bool includeFolders = true) {
            var output = new List<FileObject>();
            if (!Directory.Exists(dirPath))
                return output;

            // get dir info
            var dirInfo = new DirectoryInfo(dirPath);

            // for each file in the dir
            foreach (var fileInfo in dirInfo.GetFiles()) {
                FileType fileType;
                if (!Enum.TryParse(fileInfo.Extension.Replace(".", ""), true, out fileType))
                    fileType = FileType.Unknow;
                output.Add(new FileObject {
                    FileName = fileInfo.Name,
                    BasePath = fileInfo.DirectoryName,
                    FullPath = fileInfo.FullName,
                    Flags = FileFlag.ReadOnly,
                    Size = fileInfo.Length,
                    CreateDateTime = fileInfo.CreationTime,
                    ModifieDateTime = fileInfo.LastWriteTime,
                    Type = fileType
                });
            }

            // for each folder in dir
            if (includeFolders) {
                Regex regex = new Regex(@"\\\.");
                foreach (var directoryInfo in dirInfo.GetDirectories()) {
                    if (!Config.Instance.FileExplorerIgnoreUnixHiddenFolders || !regex.IsMatch(directoryInfo.FullName)) {
                        // recursive
                        if (recursive)
                            output.AddRange(ListFileOjectsInDirectory(directoryInfo.FullName));
                        output.Add(new FileObject {
                            FileName = directoryInfo.Name,
                            BasePath = Path.GetDirectoryName(directoryInfo.FullName),
                            FullPath = directoryInfo.FullName,
                            CreateDateTime = directoryInfo.CreationTime,
                            ModifieDateTime = directoryInfo.LastWriteTime,
                            Type = FileType.Folder
                        });
                    }
                }
            }

            return output;
        }

        #endregion

        #region Dockable dialog
        public static EmptyForm FakeForm { get; private set; }
        public static int DockableCommandIndex;

        public static void Toggle(bool doShow) {
            if ((doShow && !IsVisible) || (!doShow && IsVisible)) {
                Toggle();
            }
        }

        /// <summary>
        /// Toggle the docked form on and off, can be called first and will initialize the form
        /// </summary>
        public static void Toggle() {
            try {
                // initialize if not done
                if (FakeForm == null) {
                    Init();
                } else {
                    Win32.SendMessage(Npp.HandleNpp, !FakeForm.Visible ? NppMsg.NPPM_DMMSHOW : NppMsg.NPPM_DMMHIDE, 0, FakeForm.Handle);
                }
                if (FakeForm == null) return;
                UpdateMenuItemChecked();
            } catch (Exception e) {
                ErrorHandler.ShowErrors(e, "Error in Dockable explorer");
            }
        }

        /// <summary>
        /// Either check or uncheck the menu, depending on the visibility of the form
        /// (does it both on the menu and toolbar)
        /// </summary>
        public static void UpdateMenuItemChecked() {
            if (FakeForm == null) return;
            Win32.SendMessage(Npp.HandleNpp, NppMsg.NPPM_SETMENUITEMCHECK, Plug.FuncItems.Items[DockableCommandIndex]._cmdID, FakeForm.Visible ? 1 : 0);
            Config.Instance.FileExplorerVisible = FakeForm.Visible;
        }

        /// <summary>
        /// Initialize the form
        /// </summary>
        public static void Init() {
            // register fake form to Npp
            FakeForm = new EmptyForm();
            NppTbData nppTbData = new NppTbData {
                hClient = FakeForm.Handle,
                pszName = AssemblyInfo.ProductTitle + " - File explorer",
                dlgID = DockableCommandIndex,
                uMask = NppTbMsg.DWS_DF_CONT_LEFT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR,
                hIconTab = (uint) Utils.GetIconFromImage(ImageResources.FileExplorerLogo).Handle,
                pszModuleName = AssemblyInfo.ProductTitle
            };

            IntPtr ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(nppTbData));
            Marshal.StructureToPtr(nppTbData, ptrNppTbData, false);
            Win32.SendMessage(Npp.HandleNpp, NppMsg.NPPM_DMMREGASDCKDLG, 0, ptrNppTbData);

            Form = new FileExplorerForm(FakeForm);
        }

        public static void ForceClose() {
            if (Form != null)
                Form.Close();
        }

        #endregion
    }

    #region FileObject

    /// <summary>
    /// Object describing a file
    /// </summary>
    public class FileObject {
        public string FileName { get; set; }
        public string BasePath { get; set; }
        public string FullPath { get; set; }
        public DateTime ModifieDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public long Size { get; set; }
        public FileType Type { get; set; }
        public FileFlag Flags { get; set; }
        public string SubString { get; set; }
    }

    /// <summary>
    /// Type of an object file (depends on the file's extension),
    /// corresponds to an icon that appends "Type" to the enum name,
    /// for example the icon for R files is named RType.png
    /// </summary>
    public enum FileType {
        Unknow,
        Df,
        D,
        Folder,
        I,
        Lst,
        P,
        R,
        T,
        W
    }

    /// <summary>
    /// File's flags,
    /// Same as other flag, corresponds to an icon with the same name as in the enumeration
    /// </summary>
    [Flags]
    public enum FileFlag {
        /// <summary>
        /// Is the file starred by the user
        /// </summary>
        Favourite = 1,
        ReadOnly = 2
    }

    #endregion

}