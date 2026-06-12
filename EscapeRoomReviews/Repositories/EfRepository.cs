using EscapeRoomReviews.Data;
using EscapeRoomReviews.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace EscapeRoomReviews.Repositories;

public class EfRepository
{
    private readonly ApplicationDbContext _context;

    public EfRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<EscapeRoom> GetAllRooms() => _context.EscapeRooms
        .AsNoTracking()
        .Where(room => room.DeletedAt == null)
        .Include(room => room.Location)
        .Include(room => room.Company)
        .Include(room => room.Themes)
        .Include(room => room.Photos)
        .Include(room => room.Reviews.Where(review => review.DeletedAt == null))
            .ThenInclude(review => review.User)
        .ToList();

    public EscapeRoom? GetRoomById(int id) => _context.EscapeRooms
        .AsNoTracking()
        .Where(room => room.DeletedAt == null)
        .Include(room => room.Location)
        .Include(room => room.Company)
        .Include(room => room.Themes)
        .Include(room => room.Photos)
        .Include(room => room.Reviews.Where(review => review.DeletedAt == null))
            .ThenInclude(review => review.User)
        .FirstOrDefault(room => room.Id == id);

    public List<Review> GetAllReviews() => _context.Reviews
        .AsNoTracking()
        .Where(review => review.DeletedAt == null)
        .Include(review => review.EscapeRoom)
            .ThenInclude(room => room.Location)
        .Include(review => review.EscapeRoom)
            .ThenInclude(room => room.Company)
        .Include(review => review.EscapeRoom)
            .ThenInclude(room => room.Themes)
        .Include(review => review.User)
        .ToList();

    public List<User> GetAllUsers() => _context.AppUsers
        .AsNoTracking()
        .Where(user => user.DeletedAt == null)
        .Include(user => user.Reviews.Where(review => review.DeletedAt == null))
            .ThenInclude(review => review.EscapeRoom)
                .ThenInclude(room => room.Location)
        .Include(user => user.Reviews.Where(review => review.DeletedAt == null))
            .ThenInclude(review => review.EscapeRoom)
                .ThenInclude(room => room.Company)
        .ToList();

    public List<Location> GetAllLocations() => _context.Locations
        .AsNoTracking()
        .Where(location => location.DeletedAt == null)
        .Include(location => location.EscapeRooms.Where(room => room.DeletedAt == null))
            .ThenInclude(room => room.Company)
        .Include(location => location.EscapeRooms.Where(room => room.DeletedAt == null))
            .ThenInclude(room => room.Reviews.Where(review => review.DeletedAt == null))
        .ToList();

    public List<Company> GetAllCompanies() => _context.Companies
        .AsNoTracking()
        .Where(company => company.DeletedAt == null)
        .Include(company => company.EscapeRooms.Where(room => room.DeletedAt == null))
            .ThenInclude(room => room.Location)
        .Include(company => company.EscapeRooms.Where(room => room.DeletedAt == null))
            .ThenInclude(room => room.Reviews.Where(review => review.DeletedAt == null))
        .ToList();

    public List<Theme> GetAllThemes() => _context.Themes
        .AsNoTracking()
        .Where(theme => theme.DeletedAt == null)
        .Include(theme => theme.EscapeRooms.Where(room => room.DeletedAt == null))
            .ThenInclude(room => room.Location)
        .Include(theme => theme.EscapeRooms.Where(room => room.DeletedAt == null))
            .ThenInclude(room => room.Company)
        .Include(theme => theme.EscapeRooms.Where(room => room.DeletedAt == null))
            .ThenInclude(room => room.Reviews.Where(review => review.DeletedAt == null))
        .ToList();
}