    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using PFG_BackEnd.ModelsDTO;  

    namespace PFG_BackEnd.Service;

    public class TipusProducteService : ServiceCollection
    {
        private readonly AppDbContext AppDbContext;

        public TipusProducteService(AppDbContext context)
        {
            AppDbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<string>> GetAllAsync()
        {
            return await AppDbContext.TipusProducte
                .Select(t => t.NomTipus.Trim())  
                .ToListAsync();  
        }
    }