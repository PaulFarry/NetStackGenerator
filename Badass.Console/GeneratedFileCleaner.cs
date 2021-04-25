﻿using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Badass.Model;
using Serilog;

namespace Badass.Console
{
    public class GeneratedFileCleaner
    {
        private readonly IFileSystem _fileSystem;
        private readonly Settings _settings;

        public GeneratedFileCleaner(IFileSystem fileSystem, Settings settings)
        {
            _fileSystem = fileSystem;
            _settings = settings;
        }

        public void ClearGeneratedFiles()
        {
            if (!_fileSystem.Directory.Exists(_settings.RootDirectory))
            {
                Log.Error("Unable to removed generated files - root folder does not exist.");
                return;
            }

            ClearClientFiles();
            ClearSqlFiles();
            ClearCsharpFiles();

            Log.Information("Finished clearing generated files");
        }

        private void ClearClientFiles()
        {
            if (_fileSystem.Directory.Exists(_settings.RootDirectory))
            {
                var reactPath = _fileSystem.Path.Combine(_settings.RootDirectory, Generator.ReactComponentDirectory);
                if (_fileSystem.Directory.Exists(reactPath))
                {
                    var tsxFiles = _fileSystem.Directory.GetFiles(reactPath, "*.tsx", SearchOption.AllDirectories);
                    ClearClientFileList(tsxFiles);
                
                    var tsFiles = _fileSystem.Directory.GetFiles(reactPath, "*.ts", SearchOption.AllDirectories);
                    ClearClientFileList(tsFiles);
                }
            }
        }

        private void ClearSqlFiles()
        {
            if (_fileSystem.Directory.Exists(DbFolder))
            {
                var sqlFiles = _fileSystem.Directory.GetFiles(DbFolder, "*.sql", SearchOption.AllDirectories);
                foreach (var file in sqlFiles)
                {
                    var contents = _fileSystem.File.ReadAllLines(file);
                    if (contents != null && contents.Any() && IsGeneratedSqlFile(contents.First()))
                    {
                        _fileSystem.File.Delete(file);
                    }
                }
            }
        }

        private void ClearCsharpFiles()
        {
            var csharpFiles = _fileSystem.Directory.GetFiles(_settings.RootDirectory, "*.cs", SearchOption.AllDirectories).ToList();
            if (_fileSystem.Directory.Exists(DbFolder))
            {
                var dataCsFiles = _fileSystem.Directory.GetFiles(DbFolder, "*.cs", SearchOption.AllDirectories);
                csharpFiles.AddRange(dataCsFiles);
            }
            foreach (var file in csharpFiles.Distinct())
            {
                if (_fileSystem.File.Exists(file))
                {
                    var contents = _fileSystem.File.ReadAllLines(file);
                    if (contents != null && contents.Any() && IsGeneratedCsFile(contents.First()))
                    {
                        _fileSystem.File.Delete(file);
                    }
                }
            }
        }

        private void ClearClientFileList(string[] files)
        {
            foreach (var file in files)
            {
                var contents = _fileSystem.File.ReadAllLines(file);
                if (contents != null && contents.Any() && IsGeneratedTypescriptFile(contents.First()))
                {
                    _fileSystem.File.Delete(file);
                }
            }
        }

        private bool IsGeneratedTypescriptFile(string firstLine)
        {
            return firstLine.Trim() == "// generated by a tool";
        }

        private bool IsGeneratedSqlFile(string firstLine)
        {
            return firstLine.Trim() == "-- generated by a tool";
        }

        private bool IsGeneratedCsFile(string firstLine)
        {
            return firstLine.Trim() == "// generated by a tool";
        }

        private string DbFolder
        {
            get
            {
                if (!string.IsNullOrEmpty(_settings.DataDirectory))
                {
                    return _fileSystem.Path.Combine(_settings.RootDirectory, _settings.DataDirectory);
                }

                return _fileSystem.Path.Combine(_settings.RootDirectory, "Database");
            }
        }
    }
}
