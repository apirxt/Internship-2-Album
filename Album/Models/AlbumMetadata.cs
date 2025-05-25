using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Album.Models
{
    public class AlbumMetadata
    {
    }

    [ModelMetadataType(typeof(AlbumMetadata))]

    public partial class Album
    {
        public static List<Album> GetAll(AlbumdbContext context, string searchString)
        {
            return context.Albums.Where(q => q.IsDelete != true)
                                   .Include(f => f.File)
                                   .Include(s => s.Songs.Where(q => q.IsDelete != true))
                                   .Where(a => string.IsNullOrEmpty(searchString) || a.Name.Contains(searchString))
                                   .ToList();
        }

        public bool Create(AlbumdbContext context, IFormFile? Ifile)
        {
            DateTime datenow = DateTime.Now;

            // ตรวจสอบและสร้างไฟล์
            File? file = null;
            if (Ifile != null && Ifile.Length > 0)
            {
                file = File.Create(context, Ifile);
                if (file != null)
                {
                    FileId = file.Id;
                    this.File = file;
                }
            }

            // ตั้งค่าข้อมูลทั่วไป
            IsDelete = false;
            CreateBy = "user1";
            CreateDate = datenow;
            UpdateBy = "user1";
            UpdateDate = datenow;

            context.Albums.Add(this);
            context.SaveChanges();

            // เพิ่มเพลง
            List<Song> songs = this.Songs.ToList();
            foreach (Song s in songs)
            {
                if (!string.IsNullOrEmpty(s.Name))
                {
                    s.Create(context, this.Id);
                }
            }
            return true;
        }

        public Album? GetById(AlbumdbContext context, int id)
        {
            Album? album = context.Albums.Include(d => d.Songs.Where(q => q.IsDelete != true))
                                           .Include(f => f.File)
                                           .FirstOrDefault(q => q.IsDelete != true && q.Id == id);
            return album;
        }

        public Album Update(AlbumdbContext context, IFormFile? Ifile)
        {
            DateTime datenow = DateTime.Now;
            this.UpdateBy = "user2";
            this.UpdateDate = datenow;

            //อัพไฟล์ใหม่
            if (Ifile != null && Ifile.Length > 0)
            {
                File new_File = File.Create(context, Ifile);
                if (new_File != null)
                {
                    this.FileId = new_File.Id;
                }
            }

            //อัพเพลง
            foreach (Song song in this.Songs)
            {
                if (song.Id > 0 && !string.IsNullOrWhiteSpace(song.Name))
                {
                    song.UpdateBy = "user2";
                    song.UpdateDate = datenow;
                }
                if (song.Id == 0)
                {
                    song.CreateDate = datenow;
                    song.CreateBy = "user2";
                }
            }
            context.Albums.Update(this);
            context.SaveChanges();
            return this;
        }
        public bool Delete(AlbumdbContext context)
        {
            DateTime datenow = DateTime.Now;

            foreach (Song song in this.Songs)
            {
                song.Delete(context);
            }

            if (this.File != null)
            {
                this.File.Delete(context);
            }

            IsDelete = true;
            UpdateBy = "user3";
            UpdateDate = datenow;
            context.Albums.Update(this);
            context.SaveChanges();
            return true;
        }
    }
}
