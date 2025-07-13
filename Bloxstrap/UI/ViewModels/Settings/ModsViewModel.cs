﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Windows.Win32;
using Windows.Win32.UI.Shell;
using Windows.Win32.Foundation;
using CommunityToolkit.Mvvm.Input;
using Voidstrap.Models.SettingTasks;
using Voidstrap.AppData;
using Voidstrap.Enums.FlagPresets;
using System.Drawing;
using Voidstrap.Enums;
using Voidstrap.Models;
using Voidstrap.UI.Elements.Dialogs;

namespace Voidstrap.UI.ViewModels.Settings
{
    public class ModsViewModel : NotifyPropertyChangedViewModel
    {
        private static readonly Dictionary<string, byte[]> FontHeaders = new()
        {
            { "ttf", new byte[] { 0x00, 0x01, 0x00, 0x00 } },
            { "otf", new byte[] { 0x4F, 0x54, 0x54, 0x4F } },
            { "ttc", new byte[] { 0x74, 0x74, 0x63, 0x66 } }
        };

        public ICommand ManageCustomFontCommand => new RelayCommand(ManageCustomFont);
        public ICommand OpenCompatSettingsCommand => new RelayCommand(OpenCompatSettings);
        public ICommand PreviewCursorCommand => new RelayCommand(PreviewCursor);

        public Visibility ChooseCustomFontVisibility =>
            string.IsNullOrEmpty(TextFontTask.NewState) ? Visibility.Visible : Visibility.Collapsed;

        public Visibility DeleteCustomFontVisibility =>
            string.IsNullOrEmpty(TextFontTask.NewState) ? Visibility.Collapsed : Visibility.Visible;

        public ModPresetTask OldDeathSoundTask { get; } = new("OldDeathSound", "content\\sounds\\ouch.ogg", "Sounds.OldDeath.ogg");
        public ModPresetTask OldAvatarBackgroundTask { get; } = new("OldAvatarBackground", "ExtraContent\\places\\Mobile.rbxl", "OldAvatarBackground.rbxl");
        public FontModPresetTask TextFontTask { get; } = new();
        public EmojiModPresetTask EmojiFontTask { get; } = new();

        public ModPresetTask OldCharacterSoundsTask { get; } = new("OldCharacterSounds", new()
        {
            { "content\\sounds\\action_footsteps_plastic.mp3", "Sounds.OldWalk.mp3" },
            { "content\\sounds\\action_jump.mp3", "Sounds.OldJump.mp3" },
            { "content\\sounds\\action_get_up.mp3", "Sounds.OldGetUp.mp3" },
            { "content\\sounds\\action_falling.mp3", "Sounds.Empty.mp3" },
            { "content\\sounds\\action_jump_land.mp3", "Sounds.Empty.mp3" },
            { "content\\sounds\\action_swim.mp3", "Sounds.Empty.mp3" },
            { "content\\sounds\\impact_water.mp3", "Sounds.Empty.mp3" }
        });

        public EnumModPresetTask<Enums.CursorType> CursorTypeTask { get; } = new("CursorType", new()
        {
            { Enums.CursorType.DotCursor, new() {
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowCursor.png", "Cursor.DotCursor.ArrowCursor.png" },
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowFarCursor.png", "Cursor.DotCursor.ArrowFarCursor.png" },
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowCursorDecalDrag.png", "Cursor.DotCursor.ArrowCursorDecalDrag.png" }
            }},
                { Enums.CursorType.WhiteDotCursor, new() {
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowCursor.png", "Cursor.WhiteDotCursor.ArrowCursor.png" },
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowFarCursor.png", "Cursor.WhiteDotCursor.ArrowFarCursor.png" },
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowCursorDecalDrag.png", "Cursor.WhiteDotCursor.ArrowCursorDecalDrag.png" }
            }},
                { Enums.CursorType.VerySmallWhiteDot, new() {
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowCursor.png", "Cursor.VerySmallWhiteDot.ArrowCursor.png" },
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowFarCursor.png", "Cursor.VerySmallWhiteDot.ArrowFarCursor.png" },
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowCursorDecalDrag.png", "Cursor.VerySmallWhiteDot.ArrowCursorDecalDrag.png" }
            }},
            { Enums.CursorType.StoofsCursor, new() {
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowCursor.png", "Cursor.StoofsCursor.ArrowCursor.png" },
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowFarCursor.png", "Cursor.StoofsCursor.ArrowFarCursor.png" },
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowCursorDecalDrag.png", "Cursor.StoofsCursor.ArrowCursorDecalDrag.png" }
            }},
            { Enums.CursorType.CleanCursor, new() {
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowCursor.png", "Cursor.CleanCursor.ArrowCursor.png" },
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowFarCursor.png", "Cursor.CleanCursor.ArrowFarCursor.png" },
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowCursorDecalDrag.png", "Cursor.CleanCursor.ArrowCursorDecalDrag.png" }
            }},
            { Enums.CursorType.FPSCursor, new() {
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowCursor.png", "Cursor.FPSCursor.ArrowCursor.png" },
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowFarCursor.png", "Cursor.FPSCursor.ArrowFarCursor.png" },
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowCursorDecalDrag.png", "Cursor.FPSCursor.ArrowCursorDecalDrag.png" }
            }},
            { Enums.CursorType.From2006, new() {
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowCursor.png", "Cursor.From2006.ArrowCursor.png" },
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowFarCursor.png", "Cursor.From2006.ArrowFarCursor.png" },
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowCursorDecalDrag.png", "Cursor.From2006.ArrowCursorDecalDrag.png" }
            }},
            { Enums.CursorType.From2013, new() {
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowCursor.png", "Cursor.From2013.ArrowCursor.png" },
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowFarCursor.png", "Cursor.From2013.ArrowFarCursor.png" },
                { "content\\textures\\Cursors\\KeyboardMouse\\ArrowCursorDecalDrag.png", "Cursor.From2013.ArrowCursorDecalDrag.png" }
            }}
        });

        private void ManageCustomFont()
        {
            if (!string.IsNullOrEmpty(TextFontTask.NewState))
            {
                TextFontTask.NewState = string.Empty;
            }
            else
            {
                var dialog = new OpenFileDialog { Filter = $"{Strings.Menu_FontFiles}|*.ttf;*.otf;*.ttc" };

                if (dialog.ShowDialog() != true) return;

                string type = Path.GetExtension(dialog.FileName).TrimStart('.').ToLowerInvariant();
                byte[] fileHeader = File.ReadAllBytes(dialog.FileName).Take(4).ToArray();

                if (!FontHeaders.TryGetValue(type, out var expectedHeader) || !expectedHeader.SequenceEqual(fileHeader))
                {
                    Frontend.ShowMessageBox("Custom Font Invalid", MessageBoxImage.Error);
                    return;
                }

                TextFontTask.NewState = dialog.FileName;
            }

            OnPropertyChanged(nameof(ChooseCustomFontVisibility));
            OnPropertyChanged(nameof(DeleteCustomFontVisibility));
        }

        private void OpenCompatSettings()
        {
            string path = new RobloxPlayerData().ExecutablePath;

            if (File.Exists(path))
                PInvoke.SHObjectProperties(HWND.Null, SHOP_TYPE.SHOP_FILEPATH, path, "Compatibility");
            else
                Frontend.ShowMessageBox(Strings.Common_RobloxNotInstalled, MessageBoxImage.Error);
        }

        private void PreviewCursor()
        {
            var previewDialog = new CursorPreviewDialog();
            if (previewDialog.ShowDialog() == true && previewDialog.SelectedCursor.HasValue)
            {
                SelectedCursor = previewDialog.SelectedCursor.Value;
                OnPropertyChanged(nameof(SelectedCursor));
            }
        }

        public string CustomFontLocation => App.Settings.Prop.CustomFontLocation;

        public Enums.CursorType SelectedCursor
        {
            get => App.Settings.Prop.CursorType;
            set => App.Settings.Prop.CursorType = value;
        }
    }
}