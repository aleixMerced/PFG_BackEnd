    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using PFG_BackEnd.Models;
    using PFG_BackEnd.ModelsDTO;  

    namespace PFG_BackEnd.Service;

    public class TipusProducteService : ServiceCollection
    {
        private readonly AppDbContext AppDbContext;
        private readonly IWebHostEnvironment env;


        public TipusProducteService(AppDbContext context, IWebHostEnvironment _env)
        {
            AppDbContext = context ?? throw new ArgumentNullException(nameof(context));
            env = _env ?? throw new ArgumentNullException(nameof(_env));
        }

        public async Task<List<Tipusproducte>> GetAllAsync()
        {
            return await AppDbContext.TipusProducte
                .AsNoTracking()
                .OrderBy(t => t.NomTipus)
                .ToListAsync();  
        }
        private int GetLastID()
        {
            if (!AppDbContext.Productes.Any())
                return 0;

            return AppDbContext.TipusProducte
                .OrderByDescending(t => t.IdTipus)
                .Select(t => t.IdTipus)
                .First();
        }
        
        public async Task<Tipusproducte> CrearTipusProducteAsync(TipusProducteNewDTO dto)
        {
            string? relativePath = null;
            int id = GetLastID() + 1;

            if (dto.Imatge != null && dto.Imatge.Length > 0)
            {
                var uploadsRoot = Path.Combine(env.WebRootPath, "uploads", "img", "TipusProducte");
                Directory.CreateDirectory(uploadsRoot);

                var extension = Path.GetExtension(dto.Imatge.FileName);
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsRoot, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Imatge.CopyToAsync(stream);
                }

                // el que guardaràs a la BD (FotoTipus)
                relativePath = $"/uploads/img/TipusProducte/{fileName}";
            }

            var tipus = new Tipusproducte()
            {
                IdTipus       = id,
                NomTipus = dto.NomTipus.Trim(),
                FotoTipus = relativePath
            };

            AppDbContext.TipusProducte.Add(tipus);
            await AppDbContext.SaveChangesAsync();

            return tipus;
        }

        public async Task<Tipusproducte> ActualitzarTipusProducteAsync(TipusProducteNewDTO dto)
        {
            if (dto.IdTipus is null)
                throw new ArgumentException("Falta l'idtipus per actualitzar");
            
            var tipus = await AppDbContext.TipusProducte
                .FirstOrDefaultAsync(t => t.IdTipus == dto.IdTipus);

            if (tipus == null)
                throw new KeyNotFoundException("No s'ha trobat el tipus");
            
            tipus.NomTipus = dto.NomTipus;
            
            if (dto.Imatge is not null && dto.Imatge.Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(tipus.FotoTipus))
                {
                    var oldPath = Path.Combine(
                        env.WebRootPath,
                        tipus.FotoTipus.TrimStart('/', '\\'));

                    if (File.Exists(oldPath))
                        File.Delete(oldPath);
                }

                var uploadsRoot = Path.Combine(env.WebRootPath, "uploads", "img", "TipusProducte");
                Directory.CreateDirectory(uploadsRoot);

                var extension = Path.GetExtension(dto.Imatge.FileName);
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsRoot, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Imatge.CopyToAsync(stream);
                }

                var relativePath = $"/uploads/img/TipusProducte/{fileName}";
                tipus.FotoTipus = relativePath;
            }

            await AppDbContext.SaveChangesAsync();
            return tipus;

        }

        public async Task<Tipusproducte> DeleteTipusByIDAsync(int id)
        {
            var tipus = await AppDbContext.TipusProducte
                .FirstOrDefaultAsync(p => p.IdTipus == id);

            if (tipus == null)
            {
                throw new KeyNotFoundException($"No s'ha trobat el producte amb id {id}");
            }
            var productes = await AppDbContext.Productes
                .Where(p => p.IdTipus == id)
                .ToListAsync();

            foreach (var prod in productes)
                prod.IdTipus = null;
            
            // Eliminar la imatge del disc si existeix
            if (!string.IsNullOrWhiteSpace(tipus.FotoTipus))
            {
                var imgPath = Path.Combine(
                    env.WebRootPath,
                    tipus.FotoTipus.TrimStart('/', '\\'));

                if (File.Exists(imgPath))
                {
                    File.Delete(imgPath);
                }
            }

            AppDbContext.TipusProducte.Remove(tipus);
            await AppDbContext.SaveChangesAsync();

            return tipus;
        }
        
        public async Task<TipusProducteDTO> GetByIDAsync(int id)
        {
            var dto = await AppDbContext.TipusProducte
                .Where(t => t.IdTipus == id)
                .Select(t => new TipusProducteDTO()
                {
                    IdTipus      = t.IdTipus,
                    NomTipus        = t.NomTipus.Trim(),
                    ImatgeTipus     = t.FotoTipus     

                })
                .FirstOrDefaultAsync();

            if (dto == null)
                throw new KeyNotFoundException($"No s'ha trobat cap tipus amb id {id}.");

            return dto;
        }
        
    }