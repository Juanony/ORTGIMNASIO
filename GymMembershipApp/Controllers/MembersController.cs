using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymMembershipApp.Data;
using GymMembershipApp.Models;

namespace GymMembershipApp.Controllers
{
    [Authorize]
    public class MembersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Members
        public async Task<IActionResult> Index(string searchString, string filter)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["StatusFilter"] = filter;

            var members = _context.Members.Include(m => m.MembershipPlan).AsQueryable();

            // Search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                members = members.Where(m =>
                    m.FirstName.Contains(searchString) ||
                    m.LastName.Contains(searchString) ||
                    m.Email.Contains(searchString) ||
                    m.PhoneNumber.Contains(searchString));
            }

            // Status filter
            if (!string.IsNullOrEmpty(filter))
            {
                switch (filter)
                {
                    case "active":
                        members = members.Where(m => m.IsActive && m.MembershipEndDate >= DateTime.Today);
                        break;
                    case "expired":
                        members = members.Where(m => m.MembershipEndDate < DateTime.Today);
                        break;
                    case "inactive":
                        members = members.Where(m => !m.IsActive);
                        break;
                    case "paymentdue":
                        var dueDate = DateTime.Today.AddDays(7);
                        members = members.Where(m => m.MembershipEndDate <= dueDate && m.MembershipEndDate >= DateTime.Today);
                        break;
                }
            }

            return View(await members.OrderBy(m => m.LastName).ToListAsync());
        }

        // GET: Members/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.MembershipPlan)
                .Include(m => m.Attendances.OrderByDescending(a => a.CheckInTime).Take(10))
                .Include(m => m.Payments.OrderByDescending(p => p.PaymentDate).Take(10))
                .FirstOrDefaultAsync(m => m.Id == id);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: Members/Create
        public IActionResult Create()
        {
            ViewData["MembershipPlanId"] = new SelectList(_context.MembershipPlans.Where(mp => mp.IsActive), "Id", "Name");
            return View();
        }

        // POST: Members/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,PhoneNumber,DateOfBirth,Address,EmergencyContact,EmergencyPhone,Notes,MembershipPlanId,MembershipStartDate")] Member member)
        {
            if (ModelState.IsValid)
            {
                member.RegistrationDate = DateTime.Now;
                member.IsActive = true;

                // Calculate membership end date if plan selected and start date provided
                if (member.MembershipPlanId.HasValue && member.MembershipStartDate.HasValue)
                {
                    var plan = await _context.MembershipPlans.FindAsync(member.MembershipPlanId.Value);
                    if (plan != null)
                    {
                        member.MembershipEndDate = member.MembershipStartDate.Value.AddDays(plan.DurationInDays);
                    }
                }

                _context.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MembershipPlanId"] = new SelectList(_context.MembershipPlans.Where(mp => mp.IsActive), "Id", "Name", member.MembershipPlanId);
            return View(member);
        }

        // GET: Members/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            ViewData["MembershipPlanId"] = new SelectList(_context.MembershipPlans.Where(mp => mp.IsActive), "Id", "Name", member.MembershipPlanId);
            return View(member);
        }

        // POST: Members/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,PhoneNumber,DateOfBirth,MembershipStartDate,MembershipEndDate,IsActive,Address,EmergencyContact,EmergencyPhone,Notes,MembershipPlanId")] Member member)
        {
            if (id != member.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingMember = await _context.Members.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
                    if (existingMember != null)
                    {
                        member.RegistrationDate = existingMember.RegistrationDate;
                    }

                    _context.Update(member);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.Id))
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

            ViewData["MembershipPlanId"] = new SelectList(_context.MembershipPlans.Where(mp => mp.IsActive), "Id", "Name", member.MembershipPlanId);
            return View(member);
        }

        // GET: Members/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.MembershipPlan)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Members/RenewMembership/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RenewMembership(int id, int membershipPlanId)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            var plan = await _context.MembershipPlans.FindAsync(membershipPlanId);
            if (plan == null)
            {
                return NotFound();
            }

            var startDate = member.MembershipEndDate.HasValue && member.MembershipEndDate > DateTime.Today
                ? member.MembershipEndDate.Value
                : DateTime.Today;

            member.MembershipPlanId = membershipPlanId;
            member.MembershipStartDate = startDate;
            member.MembershipEndDate = startDate.AddDays(plan.DurationInDays);
            member.IsActive = true;

            _context.Update(member);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = member.Id });
        }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.Id == id);
        }
    }
}
