using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClaimsSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace ClaimsSystem.Controllers
{
    //claims controller
    public class ClaimsController : Controller
    {
        private readonly ClaimsSystemContext _context;

        public ClaimsController(ClaimsSystemContext context)
        {
            _context = context;
        }

        //get claims
        public async Task<IActionResult> Index()
        {
            ViewBag.User = HttpContext.Session.GetString("user");
            ViewBag.Role = HttpContext.Session.GetString("role");
            ViewBag.User = HttpContext.Session.GetString("user");
            ViewBag.Role = HttpContext.Session.GetString("role");
            var allClaims = await _context.Claim.Where(ar => ar.Status.Equals("Approved") || ar.Status.Equals("Rejected")).ToListAsync();
            return View(allClaims);
        }

        //get claim/details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claim = await _context.Claim
                .FirstOrDefaultAsync(m => m.ClaimId == id);
            if (claim == null)
            {
                return NotFound();
            }

            return View(claim);
        }

       //get claims or create 
        public IActionResult Create()
        {
            ViewBag.User = HttpContext.Session.GetString("user");
            ViewBag.Role = HttpContext.Session.GetString("role");
            return View();
        }

       //enable properties
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClaimId,User,HoursWorked,HourlyRate,TotalPayment,FileName,Notes,Status")] Claim claim)
        {
            ViewBag.User = HttpContext.Session.GetString("user");
            ViewBag.Role = HttpContext.Session.GetString("role");
            if (ModelState.IsValid)
            {
               
                if (claim.File != null && claim.File.Length > 0)
                {
               
                    if (claim.File.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("", "File size cannot exceed 5MB.");
                        return View(claim);
                    }

                    
                    var validExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                    var extension = Path.GetExtension(claim.File.FileName).ToLower();
                    if (!validExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("", "Invalid file type. Only PDF, DOCX, XLSX, files are allowed.");
                        return View(claim);
                    }

                    string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    string filesFolder = Path.Combine(wwwRootPath, "files");

                    if (!Directory.Exists(filesFolder))
                    {
                        Directory.CreateDirectory(filesFolder);
                    }
                    //file path
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(claim.File.FileName);
                    string filePath = Path.Combine(filesFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await claim.File.CopyToAsync(fileStream);
                    }

                    claim.FileName = "/files/" + fileName;
                }

                _context.Add(new Claim
                {
                    HoursWorked = claim.HoursWorked,
                    HourlyRate = claim.HourlyRate,
                    TotalPayment = claim.HoursWorked * claim.HourlyRate,
                    File = claim.File,
                    FileName = claim.FileName,
                    Notes = claim.Notes,
                    User = HttpContext.Session.GetString("user"),
                    Status = "Pending",

                });
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(claim);
        }

        public async Task<IActionResult> PendingClaims()
        {
            ViewBag.User = HttpContext.Session.GetString("user");
            ViewBag.Role = HttpContext.Session.GetString("role");
            var pendingClaims = await _context.Claim.Where(p => p.Status.Equals("Pending")).ToListAsync();

            return View(pendingClaims);
        }

        // GETClaims/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claim = await _context.Claim.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }
            return View(claim);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var claim = await _context.Claim.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }

            claim.Status = "Rejected";

            _context.Update(claim);
            await _context.SaveChangesAsync();
            // Back to the pending claims page
            return RedirectToAction(nameof(PendingClaims));
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var claim = await _context.Claim.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }

            claim.Status = "Approved";

            _context.Update(claim);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PendingClaims)); // Redirect to pending claims
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AutomateClaimApproval(int id)
        {
            var claim = await _context.Claim.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }
            if (claim.TotalPayment >= 200 && claim.HourlyRate <= 100)
            {
                claim.Status = "Approved";
            }
            else
            {
                claim.Status = "Rejected";
            }
            

            _context.Update(claim);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PendingClaims)); // Redirect to pending claims
        }



        // To protect from overposting attacks, enabling  the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClaimId,User,HoursWorked,HourlyRate,TotalPayment,FileName,Notes,Status")] Claim claim)
        {
            if (id != claim.ClaimId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(claim);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClaimExists(claim.ClaimId))
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
            return View(claim);
        }

        // GET: Claims/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claim = await _context.Claim
                .FirstOrDefaultAsync(m => m.ClaimId == id);
            if (claim == null)
            {
                return NotFound();
            }

            return View(claim);
        }

        // POSTClaims/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var claim = await _context.Claim.FindAsync(id);
            if (claim != null)
            {
                _context.Claim.Remove(claim);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClaimExists(int id)
        {
            return _context.Claim.Any(e => e.ClaimId == id);
        }
    }
}
