using System;
using System.IO;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;

namespace Watchman
{
    [Injectable(typeof(IFileSystemHelper), Lifetime.Singleton)]
    public class FileSystemHelper : IFileSystemHelper
    {
        private readonly IContext _Context;

        public FileSystemHelper(IContext context)
        {
            _Context = context;
        }

        public void CreateFolder(string path)
        {
            (_Context.Target + "/" + path).EnsureFolder();
        }

        public void Delete(string path)
        {
            var p = (_Context.Target + "/" + path).FullPath();
            if (p.IsFile())
            {
                File.Delete(p);
            }
            if (p.IsFolder())
            {
                Directory.Delete(p, true);
            }
        }

        public void WriteFile(string path)
        {
            var s = (_Context.Source + "/" + path).FullPath();
            var t = (_Context.Target + "/" + path).FullPath();

            t.Folder().EnsureFolder();

            try
            {
                using (var inputStream = new FileStream(s, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var buffer = new byte[4 * 1024];
                    var count = 0;
                    using (var outputStream = new FileStream(t, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    {
                        while ((count = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            outputStream.Write(buffer, 0, count);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}