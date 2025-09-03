using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; // Required to read configuration
using Stripe.Checkout;


namespace Web_Frameworks_2025_EON.Controllers
{
    public class DonationController : Controller
    {
        private readonly IConfiguration _configuration;

        public DonationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This will show the main donation page
        public IActionResult Index()
        {
            // We'll pass the Stripe Publishable Key to the view
            ViewBag.StripePublishableKey = _configuration["Stripe:PublishableKey"];
            return View();
        }

        // We will build these actions in the next step
        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCheckoutSession(decimal amount)
        {
            // The URL of your application, used to build the success/cancel URLs
            var domain = "https://localhost:7094"; // Replace with your actual port number

            var options = new SessionCreateOptions
            {
                // Define the item the user is paying for (the donation)
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    // Stripe requires the amount in cents
                    UnitAmountDecimal = amount * 100,
                    Currency = "eur",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Donation to Athlete Trade",
                    },
                },
                Quantity = 1,
            },
        },
                Mode = "payment",
                // The URLs Stripe will redirect to after payment
                SuccessUrl = domain + Url.Action("Success", "Donation"),
                CancelUrl = domain + Url.Action("Cancel", "Donation"),
            };

            var service = new SessionService();
            Session session = service.Create(options);

            // Redirect the user to the secure Stripe payment page
            return Redirect(session.Url);
        }
    }
}