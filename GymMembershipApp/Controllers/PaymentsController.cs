using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymMembershipApp.Data;
using GymMembershipApp.Models;

namespace GymMembershipApp.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Payments
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, int? memberId, PaymentStatus? status)
        {
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");
            ViewData["MemberId"] = memberId;
            ViewData["Status"] = status;

            var payments = _context.Payments
                .Include(p => p.Member)
                .Include(p => p.MembershipPlan)
                .AsQueryable();

            // Filter by date range
            if (startDate.HasValue)
            {
                payments = payments.Where(p => p.PaymentDate.Date >= startDate.Value.Date);
            }
            if (endDate.HasValue)
            {
                payments = payments.Where(p => p.PaymentDate.Date <= endDate.Value.Date);
            }

            // Filter by member
            if (memberId.HasValue)
            {
                payments = payments.Where(p => p.MemberId == memberId.Value);
            }

            // Filter by status
            if (status.HasValue)
            {
                payments = payments.Where(p => p.Status == status.Value);
            }

            // Get members for dropdown
            ViewData["Members"] = new SelectList(
                await _context.Members.OrderBy(m => m.LastName).ToListAsync(),
                "Id",
                "FullName",
                memberId);

            var paymentsList = await payments.OrderByDescending(p => p.PaymentDate).ToListAsync();

            // Calculate totals
            ViewData["TotalAmount"] = paymentsList.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount);

            return View(paymentsList);
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Member)
                .Include(p => p.MembershipPlan)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Payments/Create
        public async Task<IActionResult> Create(int? memberId)
        {
            var payment = new Payment();

            if (memberId.HasValue)
            {
                var member = await _context.Members
                    .Include(m => m.MembershipPlan)
                    .FirstOrDefaultAsync(m => m.Id == memberId.Value);

                if (member != null)
                {
                    payment.MemberId = member.Id;
                    payment.MembershipPlanId = member.MembershipPlanId;

                    if (member.MembershipPlan != null)
                    {
                        payment.Amount = member.MembershipPlan.Price;
                    }
                }
            }

            ViewData["MemberId"] = new SelectList(_context.Members.OrderBy(m => m.LastName), "Id", "FullName", memberId);
            ViewData["MembershipPlanId"] = new SelectList(_context.MembershipPlans.Where(mp => mp.IsActive), "Id", "Name", payment.MembershipPlanId);

            return View(payment);
        }

        // POST: Payments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberId,Amount,PaymentMethod,Status,TransactionReference,Notes,MembershipPlanId")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                payment.PaymentDate = DateTime.Now;
                _context.Add(payment);
                await _context.SaveChangesAsync();

                // If payment is completed and has a membership plan, extend member's membership
                if (payment.Status == PaymentStatus.Completed && payment.MembershipPlanId.HasValue)
                {
                    var member = await _context.Members.FindAsync(payment.MemberId);
                    var plan = await _context.MembershipPlans.FindAsync(payment.MembershipPlanId.Value);

                    if (member != null && plan != null)
                    {
                        var startDate = member.MembershipEndDate.HasValue && member.MembershipEndDate > DateTime.Today
                            ? member.MembershipEndDate.Value
                            : DateTime.Today;

                        member.MembershipPlanId = plan.Id;
                        member.MembershipStartDate = startDate;
                        member.MembershipEndDate = startDate.AddDays(plan.DurationInDays);
                        member.IsActive = true;

                        _context.Update(member);
                        await _context.SaveChangesAsync();
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["MemberId"] = new SelectList(_context.Members.OrderBy(m => m.LastName), "Id", "FullName", payment.MemberId);
            ViewData["MembershipPlanId"] = new SelectList(_context.MembershipPlans.Where(mp => mp.IsActive), "Id", "Name", payment.MembershipPlanId);
            return View(payment);
        }

        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            ViewData["MemberId"] = new SelectList(_context.Members.OrderBy(m => m.LastName), "Id", "FullName", payment.MemberId);
            ViewData["MembershipPlanId"] = new SelectList(_context.MembershipPlans.Where(mp => mp.IsActive), "Id", "Name", payment.MembershipPlanId);
            return View(payment);
        }

        // POST: Payments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MemberId,Amount,PaymentDate,PaymentMethod,Status,TransactionReference,Notes,MembershipPlanId")] Payment payment)
        {
            if (id != payment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.Id))
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

            ViewData["MemberId"] = new SelectList(_context.Members.OrderBy(m => m.LastName), "Id", "FullName", payment.MemberId);
            ViewData["MembershipPlanId"] = new SelectList(_context.MembershipPlans.Where(mp => mp.IsActive), "Id", "Name", payment.MembershipPlanId);
            return View(payment);
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Member)
                .Include(p => p.MembershipPlan)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Payments/Pending
        public async Task<IActionResult> Pending()
        {
            var pendingPayments = await _context.Payments
                .Include(p => p.Member)
                .Include(p => p.MembershipPlan)
                .Where(p => p.Status == PaymentStatus.Pending)
                .OrderBy(p => p.PaymentDate)
                .ToListAsync();

            return View(pendingPayments);
        }

        // GET: Payments/DueMembers
        public async Task<IActionResult> DueMembers()
        {
            var dueDate = DateTime.Today.AddDays(7);
            var dueMembers = await _context.Members
                .Include(m => m.MembershipPlan)
                .Where(m => m.IsActive && m.MembershipEndDate.HasValue && m.MembershipEndDate <= dueDate && m.MembershipEndDate >= DateTime.Today)
                .OrderBy(m => m.MembershipEndDate)
                .ToListAsync();

            return View(dueMembers);
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.Id == id);
        }
    }
}
