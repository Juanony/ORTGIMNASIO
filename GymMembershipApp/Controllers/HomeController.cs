using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymMembershipApp.Data;
using GymMembershipApp.Models;

namespace GymMembershipApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var thisMonth = new DateTime(today.Year, today.Month, 1);
            var dueDate = today.AddDays(7);

            // Dashboard statistics
            ViewData["TotalMembers"] = await _context.Members.CountAsync();
            ViewData["ActiveMembers"] = await _context.Members
                .CountAsync(m => m.IsActive && m.MembershipEndDate >= today);
            ViewData["ExpiredMembers"] = await _context.Members
                .CountAsync(m => m.MembershipEndDate < today);
            ViewData["PaymentDueMembers"] = await _context.Members
                .CountAsync(m => m.IsActive && m.MembershipEndDate <= dueDate && m.MembershipEndDate >= today);

            // Today's attendance
            ViewData["TodayCheckIns"] = await _context.Attendances
                .CountAsync(a => a.CheckInTime.Date == today);
            ViewData["CurrentlyInGym"] = await _context.Attendances
                .CountAsync(a => a.CheckInTime.Date == today && a.CheckOutTime == null);

            // Monthly revenue
            ViewData["MonthlyRevenue"] = await _context.Payments
                .Where(p => p.PaymentDate >= thisMonth && p.Status == PaymentStatus.Completed)
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            // Recent check-ins (last 10)
            var recentCheckIns = await _context.Attendances
                .Include(a => a.Member)
                .Where(a => a.CheckInTime.Date == today)
                .OrderByDescending(a => a.CheckInTime)
                .Take(10)
                .ToListAsync();
            ViewData["RecentCheckIns"] = recentCheckIns;

            // Members with payment due
            var paymentDueMembers = await _context.Members
                .Include(m => m.MembershipPlan)
                .Where(m => m.IsActive && m.MembershipEndDate <= dueDate && m.MembershipEndDate >= today)
                .OrderBy(m => m.MembershipEndDate)
                .Take(10)
                .ToListAsync();
            ViewData["PaymentDueList"] = paymentDueMembers;

            // Recent payments (last 5)
            var recentPayments = await _context.Payments
                .Include(p => p.Member)
                .Include(p => p.MembershipPlan)
                .Where(p => p.Status == PaymentStatus.Completed)
                .OrderByDescending(p => p.PaymentDate)
                .Take(5)
                .ToListAsync();
            ViewData["RecentPayments"] = recentPayments;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
