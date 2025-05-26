using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Album.Models
{
    public class FileMetadata
    {
        [Required(ErrorMessage = "Please enter the song title.")]
        public IFormFile? Ifile { get; set; } = null;
    }

    [ModelMetadataType(typeof(FileMetadata))]
    public partial class File
    {
        public static File Create(AlbumdbContext context, IFormFile? coverFile)
        {
            if (coverFile == null || coverFile.Length == 0)
            {
                return null; // ไม่ต้องทำอะไร ถ้าไม่มีไฟล์
            }

            string originalFileName = Path.GetFileNameWithoutExtension(coverFile.FileName);
            string extension = Path.GetExtension(coverFile.FileName);
            string sanitizedFileName = Regex.Replace(originalFileName, "[^a-zA-Z0-9_-]", "").ToLower();
            string fileName = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + "_" + sanitizedFileName + extension;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/upload", fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(path)!); // กันพังถ้าโฟลเดอร์ไม่มี

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                coverFile.CopyTo(stream);
            }

            DateTime datenow = DateTime.Now;
            File newFile = new File
            {
                FileName = fileName,
                FilePath = "/upload/" + fileName,
                IsDelete = false,
                CreateBy = "user1",
                CreateDate = datenow,
                UpdateBy = "user2",
                UpdateDate = datenow
            };

            context.Files.Add(newFile);
            context.SaveChanges();
            return newFile;
        }

        public File Delete(AlbumdbContext context)
        {
            DateTime datenow = DateTime.Now;
            this.IsDelete = true;
            this.UpdateBy = "user3";
            this.UpdateDate = datenow;
            context.Files.Update(this);
            return this;
        }
    }
}
