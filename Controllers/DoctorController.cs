using HospitalAppointment.Extensions;
using HospitalAppointment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace HospitalAppointment.Controllers
{
    // This controller handles doctor, appointment and post-related operations.
    public class DoctorController : Controller
    {
        // This variable is used to access the IMemoryCache service.
        private readonly IMemoryCache _memoryCache;

        // Constructor receives IMemoryCache using dependency injection.
        public DoctorController(IMemoryCache memoryCache)
        {
            // Store the memory cache object.
            _memoryCache = memoryCache;
        }


        // This method returns the list of doctors.
        // The doctor list is stored in IMemoryCache for 3 minutes.
        public IActionResult Index()
        {
            // Try to get the doctor list from memory cache.
            if (!_memoryCache.TryGetValue(
                "DoctorList",
                out List<Doctor>? doctors) || doctors == null)
            {
                // Create the doctor list if it is not present in cache.
                doctors = new List<Doctor>
                {
                    // First doctor.
                    new Doctor
                    {
                        DoctorId = 1,
                        DoctorName = "Dr. Amaira Khan",
                        Specialization = "Cardiologist",
                        Experience = 10,
                        ConsultationFee = 800
                    },

                    // Second doctor.
                    new Doctor
                    {
                        DoctorId = 2,
                        DoctorName = "Dr. Zoya Qureshi",
                        Specialization = "Dermatologist",
                        Experience = 7,
                        ConsultationFee = 600
                    },

                    // Third doctor.
                    new Doctor
                    {
                        DoctorId = 3,
                        DoctorName = "Dr. Arshad Sheikh",
                        Specialization = "Neurologist",
                        Experience = 12,
                        ConsultationFee = 1000
                    }
                };

                // Create cache settings.
                MemoryCacheEntryOptions cacheOptions =
                    new MemoryCacheEntryOptions();

                // Set the cache expiration to 3 minutes.
                cacheOptions.AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(3);

                // Store the doctor list in memory cache.
                _memoryCache.Set(
                    "DoctorList",
                    doctors,
                    cacheOptions);
            }

            // Send the doctor list to the Index View.
            return View(doctors);
        }


        // This method displays details of a selected doctor.
        // DoctorId is received using the query string.
        // Example: /Doctor/Details?id=2
        //
        // VaryByQueryKeys ensures that each DoctorId
        // has a separate cached response.
        [ResponseCache(
            Duration = 180,
            VaryByQueryKeys = new[] { "id" })]
        public IActionResult Details(int id)
        {
            // Try to get the doctor list from memory cache.
            if (!_memoryCache.TryGetValue(
                "DoctorList",
                out List<Doctor>? doctors) || doctors == null)
            {
                // Create the doctor list if it is not available.
                doctors = CreateDoctorList();

                // Store the doctor list in memory cache for 3 minutes.
                _memoryCache.Set(
                    "DoctorList",
                    doctors,
                    TimeSpan.FromMinutes(3));
            }

            // Find the doctor whose ID matches the query string ID.
            Doctor? doctor = doctors.FirstOrDefault(
                d => d.DoctorId == id);

            // Check whether the doctor was found.
            if (doctor == null)
            {
                // Return a Not Found response if the ID is invalid.
                return NotFound();
            }

            // Calculate the total consultation charge.
            // The extension method adds ₹100 service charge.
            decimal totalCharge =
                doctor.CalculateTotalCharge();

            // Send the total charge to the View.
            ViewBag.TotalCharge = totalCharge;

            // Send the selected doctor to the Details View.
            return View(doctor);
        }


        // This method displays the appointment form.
        public IActionResult Appointment(int id)
        {
            // Try to get the doctor list from memory cache.
            if (!_memoryCache.TryGetValue(
                "DoctorList",
                out List<Doctor>? doctors) || doctors == null)
            {
                // Create the doctor list if it is not available.
                doctors = CreateDoctorList();

                // Store the doctor list in memory cache for 3 minutes.
                _memoryCache.Set(
                    "DoctorList",
                    doctors,
                    TimeSpan.FromMinutes(3));
            }

            // Find the selected doctor using DoctorId.
            Doctor? doctor = doctors.FirstOrDefault(
                d => d.DoctorId == id);

            // Check whether the doctor exists.
            if (doctor == null)
            {
                // Return Not Found if the doctor does not exist.
                return NotFound();
            }

            // Send the selected doctor to the Appointment View.
            return View(doctor);
        }


        [HttpPost]
public IActionResult Appointment(
    int DoctorId,
    string PatientName,
    string Specialization)
{
    // Create cookie options for storing the patient name.
    CookieOptions options = new CookieOptions
    {
        // Keep the patient name cookie for 30 minutes.
        Expires = DateTimeOffset.Now.AddMinutes(30),

        // Prevent JavaScript from accessing the cookie.
        HttpOnly = true
    };

    // Store the patient name in a Cookie.
    Response.Cookies.Append(
        "PatientName",
        PatientName,
        options);

    // Store the selected specialization in Session.
    HttpContext.Session.SetString(
        "SelectedSpecialization",
        Specialization);

    // Pass appointment information to the confirmation page.
    ViewBag.PatientName = PatientName;
    ViewBag.DoctorId = DoctorId;
    ViewBag.Specialization = Specialization;

    // Display the appointment confirmation page.
    return View("AppointmentSuccess");
}

        // This method retrieves posts asynchronously
        // from the JSONPlaceholder web service.
        public async Task<IActionResult> Posts()
        {
            // Create an HttpClient object.
            using HttpClient client = new HttpClient();

            // Send an asynchronous GET request to JSONPlaceholder.
            HttpResponseMessage response =
                await client.GetAsync(
                    "https://jsonplaceholder.typicode.com/posts");

            // Check whether the request was successful.
            if (!response.IsSuccessStatusCode)
            {
                // Return an error message if the request failed.
                return Content(
                    "Unable to retrieve posts.");
            }

            // Read the response content asynchronously.
            string json =
                await response.Content.ReadAsStringAsync();

            // Convert the JSON response into a list of Post objects.
            List<Post>? posts =
                JsonSerializer.Deserialize<List<Post>>(json);

            // If no posts were received, create an empty list.
            posts ??= new List<Post>();

            // Send the posts to the Posts View.
            return View(posts);
        }


        // This method creates the list of sample doctors.
        // It is used whenever the doctor list is not present in cache.
        private List<Doctor> CreateDoctorList()
        {
            // Return a list containing three doctors.
            return new List<Doctor>
            {
                // Doctor 1.
                new Doctor
                {
                    DoctorId = 1,
                    DoctorName = "Dr. Amaira Khan",
                    Specialization = "Cardiologist",
                    Experience = 10,
                    ConsultationFee = 800
                },

                // Doctor 2.
                new Doctor
                {
                    DoctorId = 2,
                    DoctorName = "Dr. Zoya Qureshi",
                    Specialization = "Dermatologist",
                    Experience = 7,
                    ConsultationFee = 600
                },

                // Doctor 3.
                new Doctor
                {
                    DoctorId = 3,
                    DoctorName = "Dr. Arshad Sheikh",
                    Specialization = "Neurologist",
                    Experience = 12,
                    ConsultationFee = 1000
                }
            };
        }
    }


    // This class represents a post received from JSONPlaceholder.
    public class Post
    {
        // Stores the post ID.
        public int id { get; set; }

        // Stores the post title.
        public string title { get; set; } = string.Empty;

        // Stores the post body.
        public string body { get; set; } = string.Empty;
    }
}