using CompanyManagement.Models;

namespace CompanyManagement.Services
{
    public class CompanyService : GenericService<Company>
    {
        private readonly CMScontext _context;
        public CompanyService(CMScontext context) : base(context)
        { 
            _context = context;
        }
        public Company GetById(long companyId) {
            return _context.Company.Where(i => i.Id == companyId).FirstOrDefault() ?? new Company();
        }
    }
}
