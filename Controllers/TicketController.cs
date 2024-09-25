using CompanyManagement.Helper;
using CompanyManagement.ModelHelper;
using CompanyManagement.Models;
using CompanyManagement.ViewModel;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CompanyManagement.Controllers
{
    public class TicketController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public CMScontext _context;

        public TicketController(CMScontext context)
        {
            _context = context;
        }
       
        public IActionResult TicketList()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }
            using (var con = new SqlConnection(AppSetting.ConnectionString))
            {
                var sql = $@"SELECT    t.Id AS TicketId,  t.TicketNo,   t.IssuedBy,   t.IssuedDate,   t.ProductId,   ticket.ProductName,  t.ServiceTypeId,  s.ServiceTypeName,  t.TicketTypeId,  tt.TicketTypeName,   t.TicketSubject,
    t.TicketDetails,   t.CompleteBy,    t.CompleteDate,    t.ManagementTicketStatus,    t.CustomerStatus,
    CASE    WHEN t.ActiveStatus = 1 THEN 'Active'   WHEN t.ActiveStatus = 2 THEN 'Processing'     WHEN t.ActiveStatus = 3 THEN 'Closed' WHEN t.ActiveStatus = 4 THEN 'Discarded'       ELSE 'Active'   END AS StatusString,  -- Translated status string
    u.UserName AS IssuedByName FROM    Ticket t    INNER JOIN [User] u ON t.IssuedBy = u.Id    INNER JOIN Product ticket ON t.ProductId = ticket.Id     INNER JOIN ServiceType s ON t.ServiceTypeId = s.Id    LEFT JOIN TicketType tt ON t.TicketTypeId = tt.Id ";
                ViewBag.Ticket = con.Query<TicketVm>(sql).ToList();
                return View();
            }
        }
        public IActionResult TicketType()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var ticketTypeList = _context.TicketType.Where(i => i.ActiveStatus == 1).ToList();
            return View(ticketTypeList);
        }
        //add ticket type list
        [HttpGet]
        public IActionResult AddTicketType()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }


            ViewBag.TitleName = "Add TicketType";
            ViewBag.Action = "AddTicketType";
            return View("~/Views/Ticket/_AddTicketType.cshtml", new TicketType());
        }


        [HttpPost]
        public JsonResult AddTicketType(TicketType ticketType)
        {

            ticketType.ActiveStatus = 1;
            _context.TicketType.Add(ticketType);
            _context.SaveChanges();
            return Json(AutoResponse.SuccessMessage("Ticket Type Added"));

        }
        [HttpGet]
        public IActionResult EditTicketType(int id)
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var ticketType = _context.TicketType.Find(id);

            ViewBag.TitleName = "Edit TicketType";
            ViewBag.Action = "EditTicketType";
            return View("~/Views/Ticket/_AddTicketType.cshtml", ticketType);
        }
        [HttpPost]
        public JsonResult EditTicketType(TicketType ticketType)
        {
            ticketType.ActiveStatus = 1;

            _context.TicketType.Update(ticketType);
            _context.SaveChanges();
            return Json(AutoResponse.SuccessMessage("Ticket Type Updated"));
        }
        //Delete a ticket type
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                var ticketType = _context.TicketType
                    .FirstOrDefault(i => i.Id == id);

                if (ticketType != null)
                {
                    ticketType.ActiveStatus = 0;
                    _context.TicketType.Update(ticketType);
                    _context.SaveChanges();
                    return Json(new { success = true, message = "Ticket Type deleted successfully" });
                }

                return Json(new { success = false, message = "Ticket Type not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet]
        public IActionResult CreateTicket()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");
            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var userId = vmuser.Id;
            var mproducts = _context.Product.ToList();
            // Fetch user products synchronously
            var userProducts = _context.UserProduct
                .Where(i => i.UserId == userId).ToList();

            // Fetch products synchronously based on user products
            var products = userProducts.Select(i => i.Product).ToList();

            // Fetch service types synchronously
            var serviceTypes = _context.ServiceType.ToList();

            // Fetch the last ticket synchronously
            var lastTicket = _context.Ticket.OrderByDescending(i => i.Id).FirstOrDefault();

            ViewBag.UserProduct = new SelectList(products, "Id", "ProductName");
            ViewBag.ServiceType = new SelectList(serviceTypes, "Id", "ServiceTypeName");

            return View(new Ticket
            {
                IssuedBy = (int)userId,
                ActiveStatus = (int)TicketStatus.Created,
                IssuedDate = DateTime.Now,
                ManagementTicketStatus = (int)TicketStatus.Created,
                CustomerStatus = (int)TicketStatus.Created,
                TicketNo = lastTicket?.TicketNo + 1 ?? 1 // Increment ticket number or set to 1
            });
        }

        [HttpPost]
        public JsonResult CreateTicket(Ticket ticket)
        {
            try
            {
                var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");
                if (vmuser == null)
                {
                    return Json(AutoResponse.ExceptionOccuredMessage("User is not logged in."));
                }

                var userId = vmuser.Id;

                if (ticket.ProductId <= 0)
                {
                    return Json(AutoResponse.ExceptionOccuredMessage("Please select a product."));
                }

                if (ticket.ServiceTypeId <= 0)
                {
                    return Json(AutoResponse.ExceptionOccuredMessage("Select a service type."));
                }

                ticket.IssuedBy =(int) userId;
                ticket.ActiveStatus = (int)TicketStatus.Created;
                ticket.IssuedDate = DateTime.Now;
                ticket.ManagementTicketStatus = (int)TicketStatus.Created;
                ticket.CustomerStatus = (int)TicketStatus.Created;

                 _context.Ticket.AddAsync(ticket);
                 _context.SaveChangesAsync();

                return Json(AutoResponse.SuccessMessage("Ticket created. The support team will contact you as soon as possible."));
            }
            catch (Exception ex)
            {
                // Optionally log the exception
                // _logger.LogError(ex, "Error creating ticket: {Message}", ex.Message);
                return Json(AutoResponse.ExceptionOccuredMessage("Server is busy. Try again later."));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTicketDetails(int id)
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var ticket = await _context.Ticket
                .Where(t => t.Id == id)
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
                    TicketTypeId = t.TicketTypeId,  // Add this if you have TicketType data
                    TicketTypeName = _context.TicketType.Where(tt => tt.Id == t.TicketTypeId).Select(tt => tt.TicketTypeName).FirstOrDefault(),
                    TicketSubject = t.TicketSubject,
                    TicketDetails = t.TicketDetails,
                    CompleteBy = t.CompleteBy,
                    CompleteDate = t.CompleteDate,
                    ManagementTicketStatus = t.ManagementTicketStatus,
                    CustomerStatus = t.CustomerStatus,
                    ActiveStatus = t.ActiveStatus,
                    AssignedName= _context.User.Where(u => u.Id == t.Assigned).Select(u => u.UserName).FirstOrDefault(),
                    IssuedByName = _context.User.Where(u => u.Id == t.IssuedBy).Select(u => u.UserName).FirstOrDefault(),
                    CompleteByName = t.CompleteBy.HasValue ? _context.User.Where(u => u.Id == t.CompleteBy.Value).Select(u => u.UserName).FirstOrDefault() : null
                })
                .FirstOrDefaultAsync();        

            return View(ticket);
        }
        [HttpGet]
        public async Task<IActionResult> Assign(int id)
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }
            // Retrieve the ticket
            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            // Get users who own the product related to the ticket
            var users = await _context.UserProduct
                .Where(up => up.ProductId == ticket.ProductId)
                .Select(up => new SelectListItem
                {
                    Value = up.UserId.ToString(),
                    Text = _context.User.FirstOrDefault(u => u.Id == up.UserId).UserName
                }).ToListAsync();

            // Create a view model
            var model = new AssignTicketViewModel
            {
                Ticket = ticket,
                Users = users
            };

            return View(model);
        }
        [HttpPost]
        public async Task<JsonResult> Assign(Ticket Ticket)
        {
            // Set ticket status
            Ticket.ActiveStatus = 2;
            Ticket.ManagementTicketStatus = 2;
            Ticket.CustomerStatus = 2;
         

            // Update the ticket in the database
            _context.Ticket.Update(Ticket);
            await _context.SaveChangesAsync();

            // Return success message with redirect URL
            return Json(new 
            {
                success = true,
                message = "User assigned successfully.",
                redirectUrl = Url.Action("GetAllTickets", "Ticket") // Adjust the controller/action names as needed
            });
        }

        [HttpPost]
        public JsonResult CloseTicket(int id)
        {
            try
            {
                // Retrieve the ticket by ID
                var ticket = _context.Ticket
                    .FirstOrDefault(a => a.Id == id);

                // Check if the ticket exists
                if (ticket != null)
                {
                    // Get the logged-in user’s ID from session
                    var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

                    // Ensure the user is logged in
                    if (vmuser == null)
                    {
                        return Json(AutoResponse.ErrorMessage("User not logged in. Please log in to continue."));
                    }

                    long userId = vmuser.Id; // Assuming Id is long and non-nullable

                    // Update the ticket’s status and other properties
                    ticket.ActiveStatus = 3; // Closed
                    ticket.CustomerStatus = 3; // Update customer status
                    ticket.ManagementTicketStatus = 3; // Update management status
                    ticket.CompleteDate = DateTime.Now; // Set completion date
                    ticket.CompleteBy = (int)userId; // Set CompleteBy to the ID of the logged-in user

                    // Save changes to the database
                    _context.SaveChanges();

                    // Return a success message
                    return Json(AutoResponse.SuccessMessage("Ticket closed successfully"));
                }

                // Return an error message if the ticket is not found
                return Json(AutoResponse.ErrorMessage("Ticket not found"));
            }
            catch (Exception ex)
            {
                // Return an error message if an exception occurs
                return Json(AutoResponse.ErrorMessage($"An error occurred: {ex.Message}"));
            }
        }

        public IActionResult AdDashboard()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var sql = @"
        SELECT 
            SUM(CASE WHEN ActiveStatus = 1 THEN 1 ELSE 0 END) AS ActiveCount, 
            SUM(CASE WHEN ActiveStatus = 2 THEN 1 ELSE 0 END) AS ProcessingCount,
            SUM(CASE WHEN ActiveStatus = 3 THEN 1 ELSE 0 END) AS DelegateCount,
            SUM(CASE WHEN ActiveStatus = 4 THEN 1 ELSE 0 END) AS DiscardCount,
            COUNT(*) AS TotalCount
        FROM Ticket
    ";

            using (var con = new SqlConnection(AppSetting.ConnectionString))
            {
                var result = con.QuerySingle(sql);

                var ticketSummaries = new List<TicketSummary>
        {
            new TicketSummary { StatusCount = result.ActiveCount, ActiveStatusString = "Active" },
            new TicketSummary { StatusCount = result.ProcessingCount, ActiveStatusString = "Processing" },
            new TicketSummary { StatusCount = result.DelegateCount, ActiveStatusString = "Closed" },
            new TicketSummary { StatusCount = result.DiscardCount, ActiveStatusString = "Discard" },
            new TicketSummary { StatusCount = result.TotalCount, ActiveStatusString = "Total Tickets" }  // Adding total count
        };

                ViewBag.TicketSummaries = ticketSummaries;
                return View(ticketSummaries);
            }
        }

        public IActionResult GetAllTickets()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }
            using (var con = new SqlConnection(AppSetting.ConnectionString))
            {
            var sql = $@"SELECT u.Username IssuedByName, ISNULL(asu.Username,'') AssignedName, 
       p.ProductName, t.IssuedDate AS IssuedDate, t.Id, t.TicketSubject, 
       t.TicketDetails, t.ActiveStatus 
FROM Ticket t
INNER JOIN [User] u ON u.Id = t.IssuedBy
LEFT JOIN [User] asu ON asu.Id = t.Assigned
LEFT JOIN Product p ON p.Id = t.ProductId;
";
            var tickets = con.Query<TicketVm>(sql).ToList();
            return View(tickets);
            }
        }
        public async Task<IActionResult> ActiveTickets()
        {
            var tickets = await _context.Ticket
                .Where(t => t.ActiveStatus == 1) // Assuming 1 is the code for Active status
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

        // GET: /Ticket/ProcessingTickets
        public async Task<IActionResult> ProcessingTickets()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var tickets = await _context.Ticket
                .Where(t => t.ActiveStatus == 2) // Assuming 2 is the code for Processing status
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

        // GET: /Ticket/DelegateTickets
        public async Task<IActionResult> ClosedTickets()
        {
            var tickets = await _context.Ticket
                .Where(t => t.ActiveStatus == 3) // Assuming 3 is the code for Delegate status
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

        // GET: /Ticket/DiscardedTickets
        public async Task<IActionResult> DiscardedTickets()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var tickets = await _context.Ticket
                .Where(t => t.ActiveStatus == 4) // Assuming 4 is the code for Discarded status
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
        [HttpGet]
        public IActionResult Communicate(int id)
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");
            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var userId = vmuser.Id; // Retrieve user ID from the session

            // Retrieve previous messages for the specific ticketId
            var previousMessages = _context.TicketChat
                .Where(tc => tc.TicketId == id)
                .OrderBy(tc => tc.CreatedDate)
                .ToList();

            // Create the view model
            var viewModel = new TicketChatViewModel
            {
                TicketId = id,
                UserId = (int)userId,
                Username = _context.User.FirstOrDefault(u => u.Id == userId)?.UserName, // Get username
                PreviousMessages = previousMessages,
                CreatedDate = DateTime.Now
            };

            return View(viewModel);
        }

        // POST: TicketChat/Communicate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Communicate(TicketChatViewModel viewModel)
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");
            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var userId = vmuser.Id; // Retrieve user ID from the session again

            // Validate that either a message or a file is sent, but not both
            bool hasTextMessage = !string.IsNullOrWhiteSpace(viewModel.ChatMessage);
            bool hasFileUpload = viewModel.FileUpload != null && viewModel.FileUpload.Length > 0;

            if (!hasTextMessage && !hasFileUpload)
            {
                ModelState.AddModelError("ChatMessage", "You must send either a message or a file.");
                return View(viewModel); // Return the view with the validation error
            }

            if (hasTextMessage && hasFileUpload)
            {
                ModelState.AddModelError("ChatMessage", "You can only send either a message or a file, not both.");
                return View(viewModel); // Return the view with the validation error
            }

            // Create the chat message
            var ticketChat = new TicketChat
            {
                TicketId = viewModel.TicketId,
                UserId = (int)userId, // Use the userId retrieved from the session
                CreatedDate = DateTime.Now
            };

            if (hasTextMessage)
            {
                ticketChat.ChatMessage = viewModel.ChatMessage;
            }

            if (hasFileUpload)
            {
                // Assuming you have logic to save the file and get its path
                ticketChat.FileUploadPath = await SaveFileAsync(viewModel.FileUpload);
            }

            // Save the chat message to the database
            _context.TicketChat.Add(ticketChat);
            await _context.SaveChangesAsync();

            // Redirect to the same communicate action to display the updated messages
            return RedirectToAction("Communicate", new { id = viewModel.TicketId });
        }

        // Helper method to save the uploaded file
        private async Task<string> SaveFileAsync(IFormFile file)
        {
            var filePath = Path.Combine("uploads", file.FileName); // Adjust your path as needed
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return filePath; // Return the path of the saved file
        }



        public IActionResult AssignedTickets()
        {
            // Retrieve the logged-in user ID from the session
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var userId = vmuser.Id;
            // Fetch the list of tickets assigned to the logged-in user
            var assignedTickets = _context.Ticket
                 .Where(t => t.Assigned == userId)
               .ToList();

            // Create and populate the view model
            var viewModel = new AssignedTicketsViewModel
            {
                Tickets = assignedTickets
            };

            return View(viewModel);
        }
        public IActionResult ClosedTicekt()
        {
            // Retrieve the logged-in user ID from the session
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var userId = vmuser.Id;
           
            var assignedTickets = _context.Ticket
                 .Where(t => t.Assigned == userId)
               .ToList();

            // Create and populate the view model
            var viewModel = new AssignedTicketsViewModel
            {
                Tickets = assignedTickets
            };

            return View(viewModel);
        }

    }

}

