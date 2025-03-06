using System;
using System.Threading.Tasks;
using EVisaTicketSystem.Core.Interfaces;
using EVisaTicketSystem.Interfaces;

namespace EVisaTicketSystem.Core.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;

        public UnitOfWork(
            DataContext context,
            IUserRepository userRepository,
            ITicketRepository ticketRepository,
            ITicketTypeRepository ticketTypeRepository,
            IOfficeRepository officeRepository) // Added OfficeRepository parameter
        {
            _context = context;
            UserRepository = userRepository;
            TicketRepository = ticketRepository;
            TicketTypeRepository = ticketTypeRepository;
            OfficeRepository = officeRepository; // Added OfficeRepository assignment
        }

        public IUserRepository UserRepository { get; }
        public ITicketRepository TicketRepository { get; }
        public ITicketTypeRepository TicketTypeRepository { get; }
        public IOfficeRepository OfficeRepository { get; } // Added OfficeRepository property

        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}
