using Microsoft.AspNetCore.Hosting;
using MyProfile.Entity.Model;
using System;

namespace MyProfile.File.Service
{
    using File = System.IO.File;

    public class FileWorkerService
    {
        private IHostingEnvironment hostingEnvironment;

        public FileWorkerService(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }
        /// <summary>
        /// File сохраняется по директории \wwwroot\resources\<nameFolder>\<guid>.<extension>
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="resourceFolder"></param>
        /// <param name="webRootPath">путь к папке wwwroot</param>
        public void CreateFileFromBase64(Resource resource, ResourceFolder resourceFolder)
        {
            string fullPath = hostingEnvironment.WebRootPath + "\\resources\\";

            switch (resourceFolder)
            {
                case ResourceFolder.Users:
                    fullPath += "users\\";
                    resource.FolderName = "users";
                    break;
                case ResourceFolder.Feedback:
                    fullPath += "feedback\\";
                    resource.FolderName = "feedback";
                    break;
                default:
                    break;
            }

            string newName = Guid.NewGuid().ToString();
            resource.Name = newName;
            fullPath += newName;
            fullPath += ".jpg";// resource.Extension;

            //удаляем из строки: data:image/png;base64, тк лишняя информация для создания файла
            var bodyBase64 = resource.BodyBase64.Substring(resource.BodyBase64.IndexOf("base64") + 7, (resource.BodyBase64.Length - (resource.BodyBase64.IndexOf("base64") + 7)));

            byte[] body = Convert.FromBase64CharArray(bodyBase64.ToCharArray(), 0, bodyBase64.Length);
            using (var fileCreate = File.Create(fullPath))
            {
                fileCreate.Write(body, 0, body.Length);
            }

            //нужно, чтобы сравнивать с предыдущей фоткой, те если пользователь обновил фоткуw
            //resource.BodyBase64 = resource.BodyBase64.Substring(0, 25);
            resource.BodyBase64 = null;
            resource.SrcPath = $@"/resources/{resource.FolderName}/{newName}.jpg";
            resource.DateCreate = DateTime.Now.ToUniversalTime();
            resource.DateEdit = DateTime.Now.ToUniversalTime();
            resource.Extension = ".jpg";
        }

        /// <summary>
        /// File сохраняется по директории \wwwroot\resources\<nameFolder>\<guid>.<extension>
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="nameFolder"></param>
        /// <param name="webRootPath">путь к папке wwwroot</param>
        public void UpdateFileFromBase64(Resource resource, ResourceFolder nameFolder)
        {
            //resource.Extension = MimeTypeMap.GetExtension(resource.Extension);
            string fullPath = hostingEnvironment.WebRootPath + "\\resources\\" + resource.FolderName + "\\";// + resource.Name; + resource.Extension;

            #region удаляем старый файл, тк может быть другой тип файла
            var files = System.IO.Directory.GetFiles(fullPath, resource.Name + "*");

            if (files.Length == 1)
            {
                File.Delete(files[0]);
            }
            #endregion
            //удаляем из строки: data:image/png;base64, тк лишняя информация для создания файла
            var bodyBase64 = resource.BodyBase64.Substring(resource.BodyBase64.IndexOf("base64") + 7, (resource.BodyBase64.Length - (resource.BodyBase64.IndexOf("base64") + 7)));
            byte[] body = Convert.FromBase64CharArray(bodyBase64.ToCharArray(), 0, bodyBase64.Length);
            fullPath += resource.Name + resource.Extension;

            using (var fileCreate = File.Create(fullPath))
            {
                fileCreate.Write(body, 0, body.Length);
            }

            resource.BodyBase64 = null;
            resource.DateEdit = DateTime.Now.ToUniversalTime();
        }

    }
}
