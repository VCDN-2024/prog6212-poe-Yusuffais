using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClaimsSystem.Models;

namespace ClaimsSystem.Controllers
{
    //user controller
    public class UsersController : Controller
    {
        private readonly ClaimsSystemContext _context;

        public UsersController(ClaimsSystemContext context)
        {
            _context = context;
        }

        // get Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.User.ToListAsync());
        }
        public async Task<IActionResult> ManageLecturers()
        {
            return View(await _context.User.Where(ml => ml.UserRole.Equals("Lecturer")).ToListAsync());
        }

        // GETUsers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GETUsers/Create
        public IActionResult Create()
        {
            return View();
        }

       
        // To protect from overposting attacks, enabling the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserID,Name,Surname,Email,Password,UserRole")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        public IActionResult Login()
        {
            return View();
        }

        // To protect from overposting attacks, enabling the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login login)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _context.User.Where(q => q.Email.Equals(login.Email) && q.Password.Equals(login.Password)).ToListAsync();
                    HttpContext.Session.SetString("user", user.ElementAt(0).Email);
                    HttpContext.Session.SetString("role", user.ElementAt(0).UserRole);
                    ViewBag.User = HttpContext.Session.GetString("user");
                    ViewBag.Role = HttpContext.Session.GetString("role");
                    if (HttpContext.Session.GetString("role").Equals("Lecturer"))
                    {
                        return RedirectToAction("Create", "Claims");
                    }
                    else if (HttpContext.Session.GetString("role").Equals("Coordinator") || HttpContext.Session.GetString("role").Equals("Manager"))
                    {
                        return RedirectToAction("PendingClaims", "Claims");
                    }
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Please enter correct details or register.");
                    return View();
                }

//returning view
            }
            return View();
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Remove("user");
            HttpContext.Session.Remove("role");
            return RedirectToAction("Login", "Users");
        }

        // GETUsers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

    
        // To protect from overposting attacks, enabling the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserID,Name,Surname,Email,Password,UserRole")] User user)
        {
            if (id != user.UserID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserID))
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
            return View(user);
        }

        // GETUsers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POSTUsers/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                _context.User.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _context.User.Any(e => e.UserID == id);
        }
    }
}
