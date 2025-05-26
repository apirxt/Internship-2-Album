using Microsoft.AspNetCore.Mvc;

namespace Album.Models
{
    public class SongMetadata
    {
    }

    [ModelMetadataType(typeof(SongMetadata))]
    public partial class Song
    {
        public Song Create(AlbumdbContext context, int AlbumId)
        {
            DateTime datenow = DateTime.Now;
            this.AlbumId = AlbumId;
            this.CreateBy = "user1";
            this.CreateDate = datenow;
            this.UpdateBy = "user2";
            this.UpdateDate = datenow;
            context.Songs.Add(this);
            //context.SaveChanges();
            return this;
        }

        //public Song Update(AlbumdbContext context)
        //{
        //    DateTime datenow = DateTime.Now;
        //    Song existingSong = context.Songs.AsNoTracking().FirstOrDefault(s => s.Id == this.Id);
        //    List<Song> allSongIds = context.Songs.Where(s => s.AlbumId == this.AlbumId && !s.IsDelete)
        //                                           .AsNoTracking()
        //                                           .ToList();
        //    List<int> thisSongIds = context.Songs.Where(s => s.Id != 0)
        //                                           .Select(s => s.Id)
        //                                           .ToList();
        //    foreach (Song oldSong in allSongIds)
        //    {
        //        if (!thisSongIds.Contains(oldSong.Id))
        //        {
        //            oldSong.IsDelete = true;
        //            oldSong.UpdateBy = "user2";
        //            oldSong.UpdateDate = DateTime.Now;
        //        }
        //    }
        //    if (this.Id != 0 && existingSong.Name != Name)
        //    {
        //        this.UpdateBy = "user2";
        //        this.UpdateDate = datenow;
        //    }
        //    else
        //    {
        //        //this.AlbumId = this.Id;
        //        this.CreateBy = "user2";
        //        this.CreateDate = datenow;
        //        this.UpdateBy = "user2";
        //        this.UpdateDate = datenow;
        //        this.IsDelete = false;
        //    }
        //    return this;
        //}

        public Song Delete(AlbumdbContext context)
        {
            DateTime datenow = DateTime.Now;
            this.IsDelete = true;
            this.UpdateBy = "user3";
            this.UpdateDate = datenow;
            context.Songs.Update(this);
            return this;
        }
    }
}
