using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xania.AspNet.Core;

namespace Xania.AspNet.Simulator
{
    public class DirectoryContentProvider : IContentProvider
    {
        private readonly string[] _baseDirectories;

        public DirectoryContentProvider(params string[] baseDirectories)
        {
            if (baseDirectories == null || baseDirectories.Length == 0)
                throw new ArgumentNullException("baseDirectories");

            var nonExisting = baseDirectories.Where(dir => !Directory.Exists(dir)).ToArray();
            if (nonExisting.Any())
                throw new ArgumentException("Following paths specified as base directories do not exists: \n-  " + String.Join("\n-  ", nonExisting));

            _baseDirectories = baseDirectories;
        }

        public bool Exists(string relativePath)
        {
            return _baseDirectories.Select(baseDirectory => Path.Combine(baseDirectory, relativePath))
                .Any(File.Exists);
        }

        public string GetPhysicalPath(string relativePath)
        {
            return _baseDirectories.Select(baseDirectory => Path.Combine(baseDirectory, relativePath))
                .FirstOrDefault(p => Directory.Exists(p) || File.Exists(p)) ?? string.Empty;
        }

        public string GetRelativePath(string physicalPath)
        {
            return _baseDirectories
                .Select(dir => GetRelativePath(new DirectoryInfo(dir), new FileInfo(physicalPath)))
                .FirstOrDefault(relativePath => relativePath != null);
        }

        private string GetRelativePath(DirectoryInfo directoryInfo, FileInfo fileInfo)
        {
            var baseDirs = GetDirectories(directoryInfo).Reverse().ToArray();
            var fileDirs = GetDirectories(fileInfo.Directory).Reverse().ToArray();


            if (baseDirs.Length > fileDirs.Length)
                return null;

            for (var i = 0; i < baseDirs.Length; i++)
            {
                if (!String.Equals(baseDirs[i], fileDirs[i]))
                    return null;
            }

            return String.Join("/", fileDirs.Skip(baseDirs.Length)) + "/" + fileInfo.Name;
        }

        private IEnumerable<string> GetDirectories(DirectoryInfo dir)
        {
            while (dir != null)
            {
                yield return dir.Name;
                dir = dir.Parent;
            }
        }

        public Stream Open(string relativePath)
        {
            foreach (var baseDirectory in _baseDirectories)
            {
                var filePath = Path.Combine(baseDirectory, relativePath);
                if (File.Exists(filePath))
                    return File.OpenRead(filePath);
            }

            throw new FileNotFoundException(String.Format("Path {0} not found in {1}", relativePath, string.Join(",", _baseDirectories)));
        }
    }
}