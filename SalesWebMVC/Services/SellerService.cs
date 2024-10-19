using SalesWebMVC.Data;
using SalesWebMVC.Models;
using Microsoft.EntityFrameworkCore;
using SalesWebMVC.Services.Exceptions;
using MySqlConnector.Logging;

namespace SalesWebMVC.Services
{
    public class SellerService
    {
        private readonly SalesWebMVCContext _context;

        public SellerService(SalesWebMVCContext context)
        {
            _context = context;
        }

        public async Task<List<Seller>> FindAllAsync()
        {
            return await _context.Seller.ToListAsync();
        }

        public async Task InsertAsync(Seller seller) {

            _context.Add(seller);
            await _context.SaveChangesAsync();
        }

        public async Task<Seller> FindByIdAsync(int id)
        {
            return await _context.Seller.Include(obj => obj.Department).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task RemoveAsync(int id)
        {
            var seller = _context.Seller.Find(id);
            _context.Seller.Remove(seller);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Seller seller)
        {
            bool hasAny = await _context.Seller.AnyAsync(s => s.Id == seller.Id);
            if (!hasAny) throw new NotFoundException("Id not found");

            try {

                _context.Update(seller);
                await _context.SaveChangesAsync();
            
            } catch(DbUpdateConcurrencyException db)
            {
                throw new DBConcurrencyException(db.Message);
            }
        }
    }
}