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
        .Include(room => room.Location)
        .Include(room => room.Company)
        .Include(room => room.Themes)
        .Include(room => room.Photos)
        .Include(room => room.Reviews)
            .ThenInclude(review => review.User)
        .ToList();

    public EscapeRoom? GetRoomById(int id) => _context.EscapeRooms
        .AsNoTracking()
        .Include(room => room.Location)
        .Include(room => room.Company)
        .Include(room => room.Themes)
        .Include(room => room.Photos)
        .Include(room => room.Reviews)
            .ThenInclude(review => review.User)
        .FirstOrDefault(room => room.Id == id);

    public List<Review> GetAllReviews() => _context.Reviews
        .AsNoTracking()
        .Include(review => review.EscapeRoom)
            .ThenInclude(room => room.Location)
        .Include(review => review.EscapeRoom)
            .ThenInclude(room => room.Company)
        .Include(review => review.EscapeRoom)
            .ThenInclude(room => room.Themes)
        .Include(review => review.User)
        .ToList();

    public List<User> GetAllUsers() => _context.Users
        .AsNoTracking()
        .Include(user => user.Reviews)
            .ThenInclude(review => review.EscapeRoom)
                .ThenInclude(room => room.Location)
        .Include(user => user.Reviews)
            .ThenInclude(review => review.EscapeRoom)
                .ThenInclude(room => room.Company)
        .ToList();

    public List<Location> GetAllLocations() => _context.Locations
        .AsNoTracking()
        .Include(location => location.EscapeRooms)
            .ThenInclude(room => room.Company)
        .Include(location => location.EscapeRooms)
            .ThenInclude(room => room.Reviews)
        .ToList();

    public List<Company> GetAllCompanies() => _context.Companies
        .AsNoTracking()
        .Include(company => company.EscapeRooms)
            .ThenInclude(room => room.Location)
        .Include(company => company.EscapeRooms)
            .ThenInclude(room => room.Reviews)
        .ToList();

    public List<Theme> GetAllThemes() => _context.Themes
        .AsNoTracking()
        .Include(theme => theme.EscapeRooms)
            .ThenInclude(room => room.Location)
        .Include(theme => theme.EscapeRooms)
            .ThenInclude(room => room.Company)
        .Include(theme => theme.EscapeRooms)
            .ThenInclude(room => room.Reviews)
        .ToList();
}