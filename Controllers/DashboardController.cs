using CompanyManagement.ModelHelper;
using CompanyManagement.Models;
using CompanyManagement.ViewModel;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Net.Sockets;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace TicketManagementSystem.Controllers
{
    public class DashboardController : Controller
    {
        public CMScontext _context;

        public DashboardController (CMScontext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CustomerDashboard()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var sql = @"
                SELECT 
                    COALESCE(SUM(CASE WHEN ActiveStatus = 1 THEN 1 ELSE 0 END), 0) AS ActiveCount, 
                    COALESCE(SUM(CASE WHEN ActiveStatus = 2 THEN 1 ELSE 0 END), 0) AS ProcessingCount,
                    COALESCE(SUM(CASE WHEN ActiveStatus = 3 THEN 1 ELSE 0 END), 0) AS ClosedCount,
                    COALESCE(SUM(CASE WHEN ActiveStatus = 4 THEN 1 ELSE 0 END), 0) AS DiscardCount,
                    COALESCE(COUNT(*), 0) AS TotalCount
                FROM Ticket
                WHERE IssuedBy = @UserId";  // Filter by logged-in user's ID
                // Filter by logged-in user's ID

            using (var con = new SqlConnection(AppSetting.ConnectionString))
            {
                var result = con.QuerySingle(sql, new { UserId = vmuser.Id }); // Pass logged-in user's ID

                // Check if result is null or if all counts are zero
                if (result == null || result.ActiveCount == 0 && result.ProcessingCount == 0 && result.ClosedCount == 0 && result.DiscardCount == 0)
                {
                    ViewBag.NoTicketHistory = true; // Set a flag for no ticket history
                    return View(); // Return to the view without ticket summaries
                }

                var ticketSummaries = new List<TicketSummary>
                {
                    new TicketSummary { StatusCount = result.ActiveCount, ActiveStatusString = "Active" },
                    new TicketSummary { StatusCount = result.ProcessingCount, ActiveStatusString = "Processing" },
                    new TicketSummary { StatusCount = result.ClosedCount, ActiveStatusString = "Closed" },
                    new TicketSummary { StatusCount = result.DiscardCount, ActiveStatusString = "Discard" },
                    new TicketSummary { StatusCount = result.TotalCount, ActiveStatusString = "Total Tickets" }
                };
                ViewBag.TicketSummaries = ticketSummaries;
                return View(ticketSummaries);
            }
        }


        public async Task<IActionResult> ActiveTickets()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");
            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var userId = vmuser.Id;
            var tickets = await _context.Ticket
                .Where(t => t.ActiveStatus == 1 && t.IssuedBy == userId) // Assuming 1 is the code for Active status
                .Select(t => new TicketVm
                {
                    TicketId = t.Id,
                    TicketNo = t.TicketNo,
                    IssuedBy = t.IssuedBy,
                    IssuedDate = t.IssuedDate,
                    ProductId = t.ProductId,
                    ProductName = _context.Product
                                    .Where(p => p.Id == t.ProductId)
                                    .Select(p => p.ProductName)
                                    .FirstOrDefault(),
                    ServiceTypeId = t.ServiceTypeId,
                    ServiceTypeName = _context.ServiceType
                                        .Where(s => s.Id == t.ServiceTypeId)
                                        .Select(s => s.ServiceTypeName)
                                        .FirstOrDefault(),
                    TicketTypeId = t.TicketTypeId,
                    TicketTypeName = _context.TicketType
                                            .Where(tt => tt.Id == t.TicketTypeId)
                                            .Select(tt => tt.TicketTypeName)
                                            .FirstOrDefault(),
                    TicketSubject = t.TicketSubject,
                    TicketDetails = t.TicketDetails,
                    CompleteBy = t.CompleteBy,
                    CompleteDate = t.CompleteDate,
                    ManagementTicketStatus = t.ManagementTicketStatus,
                    CustomerStatus = t.CustomerStatus,
                    ActiveStatus = t.ActiveStatus,
                    IssuedByName = _context.User
                                        .Where(u => u.Id == t.IssuedBy)
                                        .Select(u => u.UserName)
                                        .FirstOrDefault(),
                    
                    
                    AssignedName = t.Assigned.HasValue
                                    ? _context.User
                                        .Where(u => u.Id == t.Assigned.Value)
                                        .Select(u => u.UserName)
                                        .FirstOrDefault()
                                    : "Not Assigned",
                    CompleteByName = t.CompleteBy.HasValue
                                    ? _context.User
                                        .Where(u => u.Id == t.CompleteBy.Value)
                                        .Select(u => u.UserName)
                                        .FirstOrDefault()
                                    : null
                })
                .ToListAsync();

            return View(tickets);
        }


        public async Task<IActionResult> ProcessingTickets()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");
            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var userId = vmuser.Id;
            var tickets = await _context.Ticket
                .Where(t => t.ActiveStatus == 2 && t.IssuedBy == userId) // Assuming 1 is the code for Active status
                .Select(t => new TicketVm
                {
                    TicketId = t.Id,
                    TicketNo = t.TicketNo,
                    IssuedBy = t.IssuedBy,
                    IssuedDate = t.IssuedDate,
                    ProductId = t.ProductId,
                    ProductName = _context.Product
                                    .Where(p => p.Id == t.ProductId)
                                    .Select(p => p.ProductName)
                                    .FirstOrDefault(),
                    ServiceTypeId = t.ServiceTypeId,
                    ServiceTypeName = _context.ServiceType
                                        .Where(s => s.Id == t.ServiceTypeId)
                                        .Select(s => s.ServiceTypeName)
                                        .FirstOrDefault(),
                    TicketTypeId = t.TicketTypeId,
                    TicketTypeName = _context.TicketType
                                            .Where(tt => tt.Id == t.TicketTypeId)
                                            .Select(tt => tt.TicketTypeName)
                                            .FirstOrDefault(),
                    TicketSubject = t.TicketSubject,
                    TicketDetails = t.TicketDetails,
                    CompleteBy = t.CompleteBy,
                    CompleteDate = t.CompleteDate,
                    ManagementTicketStatus = t.ManagementTicketStatus,
                    CustomerStatus = t.CustomerStatus,
                    ActiveStatus = t.ActiveStatus,
                    IssuedByName = _context.User
                                        .Where(u => u.Id == t.IssuedBy)
                                        .Select(u => u.UserName)
                                        .FirstOrDefault(),


                    AssignedName = t.Assigned.HasValue
                                    ? _context.User
                                        .Where(u => u.Id == t.Assigned.Value)
                                        .Select(u => u.UserName)
                                        .FirstOrDefault()
                                    : "Not Assigned",
                    CompleteByName = t.CompleteBy.HasValue
                                    ? _context.User
                                        .Where(u => u.Id == t.CompleteBy.Value)
                                        .Select(u => u.UserName)
                                        .FirstOrDefault()
                                    : null
                })
                .ToListAsync();

            return View(tickets);
        }

        public async Task<IActionResult> ClosedTickets()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");
            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var userId = vmuser.Id;
            var tickets = await _context.Ticket
                .Where(t => t.ActiveStatus == 3 && t.IssuedBy == userId) // Assuming 3 is the code for Closed status
                .Select(t => new TicketVm
                {
                    TicketId = t.Id,
                    TicketNo = t.TicketNo,
                    IssuedBy = t.IssuedBy,
                    IssuedDate = t.IssuedDate,
                    ProductId = t.ProductId,
                    ProductName = _context.Product.Where(p => p.Id == t.ProductId).Select(p => p.ProductName).FirstOrDefault(),
                    ServiceTypeId = t.ServiceTypeId,
                    ServiceTypeName = _context.ServiceType.Where(s => s.Id == t.ServiceTypeId).Select(s => s.ServiceTypeName).FirstOrDefault(),
                    TicketTypeId = t.TicketTypeId,
                    TicketTypeName = _context.TicketType.Where(tt => tt.Id == t.TicketTypeId).Select(tt => tt.TicketTypeName).FirstOrDefault(),
                    TicketSubject = t.TicketSubject,
                    TicketDetails = t.TicketDetails,
                    CompleteBy = t.CompleteBy,
                    CompleteDate = t.CompleteDate,
                    ManagementTicketStatus = t.ManagementTicketStatus,
                    CustomerStatus = t.CustomerStatus,
                    ActiveStatus = t.ActiveStatus,
                    IssuedByName = _context.User.Where(u => u.Id == t.IssuedBy).Select(u => u.UserName).FirstOrDefault(),
                   
                    AssignedName = t.Assigned.HasValue ? _context.User.Where(u => u.Id == t.Assigned.Value).Select(u => u.UserName).FirstOrDefault() : "Not Assigned",
                    CompleteByName = t.CompleteBy.HasValue ? _context.User.Where(u => u.Id == t.CompleteBy.Value).Select(u => u.UserName).FirstOrDefault() : null
                })
                .ToListAsync();

            return View(tickets);
        }

        
        public async Task<IActionResult> TotalTickets()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");
            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var userId = vmuser.Id;
            // Fetch tickets with all statuses 1, 2, 3, and 4
            var tickets = await _context.Ticket
                .Where(t => t.ActiveStatus >= 1 && t.ActiveStatus <= 4 && (t.IssuedBy == userId || t.Assigned == userId))
                .Select(t => new TicketVm
                {
                    TicketId = t.Id,
                    TicketNo = t.TicketNo,
                    IssuedBy = t.IssuedBy,
                    IssuedDate = t.IssuedDate,
                    ProductId = t.ProductId,
                    ProductName = _context.Product.Where(p => p.Id == t.ProductId).Select(p => p.ProductName).FirstOrDefault(),
                    ServiceTypeId = t.ServiceTypeId,
                    ServiceTypeName = _context.ServiceType.Where(s => s.Id == t.ServiceTypeId).Select(s => s.ServiceTypeName).FirstOrDefault(),
                    TicketTypeId = t.TicketTypeId,
                    TicketTypeName = _context.TicketType.Where(tt => tt.Id == t.TicketTypeId).Select(tt => tt.TicketTypeName).FirstOrDefault(),
                    TicketSubject = t.TicketSubject,
                    TicketDetails = t.TicketDetails,
                    CompleteBy = t.CompleteBy,
                    CompleteDate = t.CompleteDate,
                    ManagementTicketStatus = t.ManagementTicketStatus,
                    CustomerStatus = t.CustomerStatus,
                    ActiveStatus = t.ActiveStatus,
                    IssuedByName = _context.User.Where(u => u.Id == t.IssuedBy).Select(u => u.UserName).FirstOrDefault(),
                    
                    AssignedName = t.Assigned.HasValue ? _context.User.Where(u => u.Id == t.Assigned.Value).Select(u => u.UserName).FirstOrDefault() : "Not Assigned",
                    CompleteByName = t.CompleteBy.HasValue ? _context.User.Where(u => u.Id == t.CompleteBy.Value).Select(u => u.UserName).FirstOrDefault() : null
                })
                .ToListAsync();

            return View(tickets);
        }

        
        public async Task<IActionResult> UserTicketSummary()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");
            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var userId = vmuser.Id;
            var sql = @"
                                    SELECT 
                                    u.Id AS UserId,
                                    u.Username AS UserName,
                                    -- Tickets Created by the User
                                    SUM(CASE WHEN t.IssuedBy = u.Id AND t.ActiveStatus = 1 THEN 1 ELSE 0 END) AS CreatedCount,
                                    SUM(CASE WHEN t.IssuedBy = u.Id AND t.ActiveStatus = 2 THEN 1 ELSE 0 END) AS ProcessingCount,
                                    SUM(CASE WHEN t.IssuedBy = u.Id AND t.ActiveStatus = 3 THEN 1 ELSE 0 END) AS ClosedCount,
                                    SUM(CASE WHEN t.IssuedBy = u.Id AND t.ActiveStatus = 4 THEN 1 ELSE 0 END) AS DiscardCount,
    
                                    -- Tickets Assigned to the User
                                    SUM(CASE WHEN t.Assigned = u.Id AND t.ActiveStatus = 1 THEN 1 ELSE 0 END) AS ActiveCount,
                                    SUM(CASE WHEN t.Assigned = u.Id AND t.ActiveStatus = 2 THEN 1 ELSE 0 END) AS ProcessingCount,
                                    SUM(CASE WHEN t.Assigned = u.Id AND t.ActiveStatus = 3 THEN 1 ELSE 0 END) AS DelegateCount,
                                    SUM(CASE WHEN t.Assigned = u.Id AND t.ActiveStatus = 4 THEN 1 ELSE 0 END) AS DiscardCount,
    
                                    -- Total Tickets Assigned to the User
                                    COUNT(t.Id) AS TotalCount,
    
                                    -- Total Tickets (if required)
                                    (SELECT COUNT(*) FROM Ticket WHERE IssuedBy = u.Id OR Assigned = u.Id) AS TotalTicketCount
                                FROM 
                                    [User] u
                                LEFT JOIN 
                                    Ticket t ON t.IssuedBy = u.Id OR t.Assigned = u.Id
                                WHERE
                                    u.Id = @UserId
                                GROUP BY 
                                    u.Id, u.Username
                                ORDER BY 
                                    u.Username

    ";

            using (var con = new SqlConnection(AppSetting.ConnectionString))
            {
                var result = await con.QuerySingleOrDefaultAsync<UserTicketSummaryVm>(sql, new { UserId = userId });

                if (result == null)
                {
                    // Handle the case where no data is returned
                    return View(new List<TicketSummary>());
                }

                var ticketSummaries = new List<TicketSummary>
        {
            new TicketSummary { StatusCount = result.ActiveCount, ActiveStatusString = "Active" },
            new TicketSummary { StatusCount = result.ProcessingCount, ActiveStatusString = "Processing" },
            new TicketSummary { StatusCount = result.DelegateCount, ActiveStatusString = "Closed" },
            
            new TicketSummary { StatusCount = result.ActiveCount + result.ProcessingCount + result.DelegateCount + result.DiscardCount, ActiveStatusString = "Total Tickets" } // Adding total count
        };

                return View(ticketSummaries);
            }
        }


    }
}
