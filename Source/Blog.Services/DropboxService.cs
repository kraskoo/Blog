namespace Blog.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DropNet;
    using DropNet.Models;
    using Models;

    /// <summary>
    /// The 'RestSharp' package must be v105.0.1, otherwise throw exception (it's a bug)
    /// </summary>
    public class DropboxService
    {
        internal const string ApiName = "DropBox";
        private static string Key;
        private static string Secrete;
        private static string Token;
        private readonly DropNetClient client;
        private readonly Dictionary<string, KeyValuePair<string, DateTime>> urlsByPathMedia;

        public DropboxService(string key, string secrete, string token)
        {
            if (string.IsNullOrEmpty(Key))
            {
                SetupDropBoxProperties(
                    new ApiConnection
                    {
                        ApiName = ApiName,
                        Key = key,
                        Secrete = secrete,
                        Token = token
                    });
            }

            this.client = new DropNetClient(Key, Secrete, Token)
            {
                UseSandbox = true
            };

            this.urlsByPathMedia = new Dictionary<string, KeyValuePair<string, DateTime>>();
        }

        public void UploadFile(string pathToFile, byte[] byteRepresentation, bool isOverride = false)
        {
            pathToFile = this.TrimLastSlashIfExists(pathToFile);
            if (!isOverride && this.IsFileExists(pathToFile))
            {
                throw new ArgumentException("The file with given name already exist in current api folder.");
            }

            var subDirectory = this.SplitPath(ref pathToFile);
            if (!this.IsDirectoryExists(subDirectory))
            {
                throw new ArgumentException("Path doesn't exist.");
            }

            this.client.UploadFile(subDirectory, pathToFile, byteRepresentation, isOverride);
        }

        public void RenameFolder(string pathToFolder, string oldName, string newNameFolder)
        {
            var oldDirectory = $"{pathToFolder}{oldName}";
            var newDirectory = $"{pathToFolder}{newNameFolder}";
            this.CreateDirectory(newDirectory);
            var files = this.GetFiles(oldDirectory);
            foreach (var file in files)
            {
                var fileName = $"{file}";
                this.SplitPath(ref fileName);
                this.client.Copy(file, $"{newDirectory}/{fileName}");
            }

            this.client.Delete(oldDirectory);
        }

        public string GetUrlToApiFile(string path)
        {
            path = this.TrimLastSlashIfExists(path);
            if (!this.IsFileExists(path))
            {
                throw new ArgumentException("Path doesn't exist.");
            }

            var containsKey = this.urlsByPathMedia.ContainsKey(path);
            if (!containsKey)
            {
                var media = this.client.GetMedia(path);
                this.urlsByPathMedia.Add(path, new KeyValuePair<string, DateTime>(media.Url, media.ExpiresDate));
            }

            if (containsKey && DateTime.UtcNow.AddSeconds(3) >= this.urlsByPathMedia[path].Value)
            {
                var media = this.client.GetMedia(path);
                this.urlsByPathMedia[path] = new KeyValuePair<string, DateTime>(media.Url, media.ExpiresDate);
            }

            return this.urlsByPathMedia[path].Key;
        }

        public bool CreateDirectory(string path)
        {
            path = this.TrimLastSlashIfExists(path);
            var directories = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string currentPath = $"/{directories[0]}";
            if (!this.IsDirectoryExists(currentPath))
            {
                this.client.CreateFolder(currentPath);
            }

            var isCreated = true;
            for (int index = 1; index < directories.Length; index++)
            {
                currentPath = $"/{string.Join("/", directories.Take(index + 1))}";
                isCreated = !this.IsDirectoryExists(currentPath);
                if (isCreated)
                {
                    this.client.CreateFolder(currentPath);
                }
            }

            return isCreated;
        }

        public IEnumerable<string> GetFiles(string path, bool withPath = true)
        {
            path = this.TrimLastSlashIfExists(path);
            var metaData = this.GetFiles(this.ApiPathResult(path));
            foreach (var data in metaData)
            {
                yield return withPath ? data.Path : data.Name;
            }
        }

        public IEnumerable<string> GetDirectories(string path, bool justInCurrentDirectory = true, bool withPath = true)
        {
            path = this.TrimLastSlashIfExists(path);
            var metaData = this.GetDirectories(this.ApiPathResult(path));
            if (!justInCurrentDirectory)
            {
                var directories = new HashSet<MetaData>();
                foreach (var data in metaData)
                {
                    this.GetDirectories(data, directories);
                }

                foreach (var data in directories)
                {

                    yield return withPath ? data.Path : data.Name;
                }
            }
            else
            {
                foreach (var data in metaData)
                {
                    yield return withPath ? data.Path : data.Name;
                }
            }
        }

        public bool IsDirectoryExists(string searchedDir)
        {
            this.EnsurePathStartsWithSlash(searchedDir);
            searchedDir = this.TrimLastSlashIfExists(searchedDir);
            var path = this.SplitPath(ref searchedDir);
            var result = this.GetDirectories(this.ApiPathResult(path));
            foreach (var data in result)
            {
                if (data.Name == searchedDir)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsFileExists(string searchedFile)
        {
            this.EnsurePathStartsWithSlash(searchedFile);
            searchedFile = this.TrimLastSlashIfExists(searchedFile);
            string path = this.SplitPath(ref searchedFile);
            var result = this.GetFiles(this.ApiPathResult(path));
            foreach (var data in result)
            {
                if (data.Name == searchedFile)
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerable<MetaData> GetDirectories(IEnumerable<MetaData> metaData)
        {
            foreach (var data in metaData)
            {
                if (data.Is_Dir)
                {
                    yield return data;
                }
            }
        }

        private IEnumerable<MetaData> GetFiles(IEnumerable<MetaData> metaData)
        {
            foreach (var data in metaData)
            {
                if (!data.Is_Dir)
                {
                    yield return data;
                }
            }
        }

        private IEnumerable<MetaData> ApiPathResult(string directory)
        {
            var searchIn = this.client.GetMetaData(directory, string.Empty, true);
            foreach (var content in searchIn.Contents)
            {
                yield return content;
            }
        }

        private string TrimLastSlashIfExists(string searchedDir)
        {
            if (searchedDir.EndsWith("/"))
            {
                searchedDir = searchedDir.Substring(0, searchedDir.Length - 1);
            }

            return searchedDir;
        }

        private string SplitPath(ref string searched)
        {
            var lastSubDirectory = searched.LastIndexOf('/');
            var path = searched.Substring(0, lastSubDirectory + 1);
            searched = searched.Substring(lastSubDirectory + 1);
            return path;
        }

        private void EnsurePathStartsWithSlash(string path)
        {
            if (path[0] != '/')
            {
                throw new ArgumentException("Path must start with '/'");
            }
        }

        private static void SetupDropBoxProperties(ApiConnection dropBox)
        {
            Key = dropBox.Key;
            Secrete = dropBox.Secrete;
            Token = dropBox.Token;
        }

        private void GetDirectories(MetaData data, HashSet<MetaData> directories)
        {
            var queue = new Queue<MetaData>();
            queue.Enqueue(data);
            directories.Add(data);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var folders = this.GetDirectories(this.ApiPathResult(current.Path));
                foreach (var folder in folders)
                {
                    if (!directories.Contains(folder))
                    {
                        directories.Add(folder);
                        queue.Enqueue(folder);
                    }
                }
            }
        }
    }
}