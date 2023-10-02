using Internship_project.Data;
using Internship_project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Text;

namespace Internship_project.Controllers
{
    [Route("api/swiftmessages")]
    [ApiController]
    public class SwiftMessagesController : ControllerBase
    {
        private readonly SwiftMessageContext _context;
        private readonly ILogger<SwiftMessagesController> _logger;

        public SwiftMessagesController(SwiftMessageContext context, ILogger<SwiftMessagesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSwiftMessage([FromForm] IFormFile swiftMessageFile)
        {

            try
            {
                if (swiftMessageFile == null || swiftMessageFile.Length == 0)
                {
                    _logger.LogError("No file uploaded.");
                    return BadRequest("No file uploaded.");
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(swiftMessageFile.FileName);

                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                var filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await swiftMessageFile.CopyToAsync(stream);
                }

                _logger.LogInformation($"File '{fileName}' uploaded succesfully. ");

                string swiftMessageText;
                using (var reader = new StreamReader(swiftMessageFile.OpenReadStream()))
                {
                    swiftMessageText = await reader.ReadToEndAsync();
                }

                ProcessSwiftMessage(swiftMessageText);

                return Ok($"File '{fileName}' uploaded succesfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error '{ex.Message}'");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }  
        }

        private IActionResult ProcessSwiftMessage(string swiftMessageText)
        {
            try
            {
                if (swiftMessageText == null)
                {
                    _logger.LogError("Invalid Swift Message");
                    return BadRequest("Invalid Swift Message");
                }





                string field1 = ExtractFieldValue(swiftMessageText, ":20:");
                string field2 = ExtractFieldValue(swiftMessageText, ":21:");
                string field3 = ExtractFieldValue(swiftMessageText, ":79:");
                string field4 = ExtractFieldValue2(swiftMessageText, "1:");
                string field5 = ExtractFieldValue2(swiftMessageText, "2:");
                string field6 = ExtractFieldValue2(swiftMessageText, "MAC:");
                string field7 = ExtractFieldValue2(swiftMessageText, "CHK:");

                var swiftMessage = new SwiftMessage
                {
                    Field1 = field4,
                    Field2 = field5,
                    Field20 = field1,
                    Field21 = field2,
                    Field79 = field3,
                    MAC = field6,
                    CHK = field7

                };

                _context.AddSwiftMessage(swiftMessage);
               

                _logger.LogInformation("Swift Message was saved succesffully.");

                return Ok("Swift Message was saved successfully.");
            }

            catch (Exception ex)
            {
                _logger.LogError($"Error '{ex.Message}'");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        private string ExtractFieldValue(string swiftMessageText, string fieldTag)
        {
            int startIndex = swiftMessageText.IndexOf(fieldTag);
            if(startIndex == -1)
            {
                return null;
            }

            startIndex += fieldTag.Length;
            int endIndex = swiftMessageText.IndexOf("\n", startIndex);
            if(endIndex == -1)
            {
                return swiftMessageText.Substring(startIndex).Trim();
            }

            return swiftMessageText.Substring(startIndex, endIndex - startIndex).Trim();
        }

        private string ExtractFieldValue2(string swiftMessageText, string fieldTag)
        {
            int startIndex = swiftMessageText.IndexOf(fieldTag);
            if (startIndex == -1)
            {
                return null;
            }

            startIndex += fieldTag.Length;
            int endIndex = swiftMessageText.IndexOf("}", startIndex);
            if (endIndex == -1)
            {
                return swiftMessageText.Substring(startIndex).Trim();
            }

            return swiftMessageText.Substring(startIndex, endIndex - startIndex).Trim();
        }
    }
}
