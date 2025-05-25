using Album.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.EntityFrameworkCore;

namespace Album.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly AlbumdbContext _context;
        public AlbumsController(AlbumdbContext context)
        {
            _context = context;
        }

        // GET: Albums
        public IActionResult Index(string searchString)
        {
            List<Models.Album> albums = Models.Album.GetAll(_context, searchString);
            return View(albums);
        }

        // GET: Albums/Create
        public IActionResult Create()
        {
            Models.Album album = new Models.Album
            {
                Songs = new List<Song> { new Song() } // ใส่เปล่าก็ได้ เพื่อให้ EditorFor แสดง
            };
            return View(album);
        }

        // POST: Albums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Models.Album album, string actionType, IFormFile Ifile, string? actionDelete)
        {
            Console.WriteLine($"[DEBUG] actionType: {actionType}");
            Console.WriteLine($"[DEBUG] Songs.Count: {album?.Songs?.Count}");

            if (actionType == "AddSong")
            {
                if (album.Songs == null)
                    album.Songs = new List<Song>();

                album.Songs.Add(new Song());

                // เคลียร์ ModelState เพื่อให้ EditorFor render state ล่าสุด
                ModelState.Clear();

                return View(album);
            }

            //if (actionDelete == "DeleteSong")
            //{
            //    album.Songs.Remove(new Song());
            //    return View(album);
            //}

            if (ModelState.IsValid)
            {
                album.Create(_context, Ifile);
                return RedirectToAction(nameof(Index));
            }
            return View(album);
        }

        // GET: Albums/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Models.Album album = new Models.Album().GetById(_context, id.Value);
            if (album == null)
            {
                return NotFound();
            }
            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Models.Album album, string? actionType, string? OldCoverPhotoPath, string? actionDelete)
        {
            if (actionType == "AddSong")
            {
                album.Songs.Add(new Song());
                if (album.File == null && !string.IsNullOrEmpty(OldCoverPhotoPath))
                {
                    album.File = new Models.File { FilePath = OldCoverPhotoPath };
                }
                return View(album);
            }

            if (actionDelete == "DeleteSong")
            {
                Song son = new Song();
                son.IsDelete = true;
                return View(album);
            }

            IFormFile newIfile = Request.Form.Files["Ifile"];

            if (ModelState.IsValid)
            {
                album.Update(_context, newIfile);
                return RedirectToAction(nameof(Index));
            }
            return View(album);
        }

        // GET: Albums/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var album = _context.Albums
                .Include(a => a.File)
                .Include(a => a.Songs.Where(s => s.IsDelete != true))
                .FirstOrDefaultAsync(m => m.Id == id);
            if (album == null)
            {
                return NotFound();
            }
            return View(album);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Models.Album items = new Models.Album().GetById(_context, id);
            if (items != null)
            {
                items.Delete(_context);

            }
            return RedirectToAction(nameof(Index));
        }
    }
}
