using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VenueBookingSystem.Models;

namespace VenueBookingSystem.Controllers
{
    public class BookingController : Controller
    {

        private readonly ApplicationDbContext _context;
        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string searchString)

        {
            var bookings = _context.Booking
                .Include(i => i.Venue)
                .Include(i => i.Event)
                .AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(i =>
                    i.Venue.VenueName.Contains(searchString) ||
                    i.Event.EventName.Contains(searchString));
            }

            return View(await bookings.ToListAsync());
        }
        public IActionResult Create()
        {
            ViewBag.Venues = _context.Venue.ToList();
            ViewBag.Event = _context.Event.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            ViewBag.Venues = _context.Venue.ToList();
            ViewBag.Event = _context.Event.ToList();

            var selectedEvent = await _context.Event.FirstOrDefaultAsync(e => e.EventID == booking.EventID);

            if (selectedEvent == null)
            {
                ModelState.AddModelError("", "Selected event not found.");
                ViewBag.Venues = _context.Venue.ToList();
                ViewBag.Event = _context.Event.ToList();
                return View(booking);
            }

            // Check manually for double booking
            var conflict = await _context.Booking
                .Include(b => b.Event)
                .AnyAsync(b => b.VenueID == booking.VenueID &&
                               b.Event.EventDate.Date == selectedEvent.EventDate.Date);

            if (conflict)
            {
                ModelState.AddModelError("", "This venue is already booked for that date.");
                ViewBag.Venues = _context.Venue.ToList();
                ViewBag.Event = _context.Event.ToList();
                return View(booking);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(booking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Booking created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    // If database constraint fails (e.g., unique key violation), show friendly message
                    ModelState.AddModelError("", "This venue is already booked for that date.");
                    ViewBag.Venues = _context.Venue.ToList();
                    ViewBag.Event = _context.Event.ToList();
                    return View(booking);
                }
            }

            ViewBag.Venues = _context.Venue.ToList();
            ViewBag.Event = _context.Event.ToList();
            return View(booking);
        }

        public async Task<IActionResult> LogDate(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null) return NotFound();

            return View(booking);
        }

        [HttpPost]

        public async Task<IActionResult> LogDate(int id, Booking model)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null) return NotFound();

            booking.BookingDate = model.BookingDate;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));



        }
        public async Task<IActionResult> Delete(int? id)
        {
            var booking = await _context.Booking
               .Include(b => b.Event)
               .Include(b => b.Venue)
               .FirstOrDefaultAsync(b => b.BookingID == id);



            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(b => b.BookingID == id);

            if (booking == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventName", booking.EventID);
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName", booking.VenueID);
            return View(booking);
        }
        private bool CompanyExists(int id)
        {
            return _context.Booking.Any(e => e.BookingID == id);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookings = await _context.Booking.FindAsync(id);
            if (id == null)
            {
                return NotFound();
            }

            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventName", bookings.EventID);
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName", bookings.VenueID);
            return View(bookings);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,EventId,VenueId,BookingDate")] Booking bookings)
        {
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventName", bookings.EventID);
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName", bookings.VenueID);

            if (id != bookings.BookingID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookings);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(bookings.BookingID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventName", bookings.EventID);
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName", bookings.VenueID);
            return View(bookings);
        }
    }

}