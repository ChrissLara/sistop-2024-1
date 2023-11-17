﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FileSystemFI.Services;
using Microsoft.Extensions.DependencyInjection;
using FileSystemFI.Extensions;

namespace FileSystemFI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private string? _fileName = "No hay ningún archivo abierto";
    [ObservableProperty] private string? _infoString = "No hay un sistema de archivos montado.";
    [ObservableProperty] private bool _enabledManagementButtons = false;
    [ObservableProperty] private FileSystemManager? _fsm = null;

    [RelayCommand]
    private async Task OpenFile()
    {
        if (Fsm is not null && Fsm.IsInitialized)
            Fsm.Dispose();

        var filesService = App.Current?.Services?.GetService<IFileService>();
        if (filesService is null)
            throw new NullReferenceException("File Service does not exists.");

        var file = await filesService.OpenFileAsync();
        if (file is null) return;
        FileName = file.Path.AbsolutePath;
        ReadFile();
    }

    [RelayCommand]
    public void CloseFile()
    {
        if (Fsm is null || !Fsm.IsInitialized) return;
        Fsm.Dispose();
        FileName = "No hay ningún archivo abierto";
        InfoString = "No hay un sistema de archivos montado.";
        EnabledManagementButtons = false;
    }

    private void ReadFile()
    {
        if (FileName is null) return;
        Fsm = new FileSystemManager();
        Fsm.OpenFileSystem(FileName);
        if (!Fsm.IsInitialized) return;
        InfoString = $"Sistema de archivos: {Fsm.Identifier} {Fsm.Version} " +
                     $"| Tamaño de cluster: {Fsm.ClusterSize} bytes.";
        EnabledManagementButtons = true;
    }
}