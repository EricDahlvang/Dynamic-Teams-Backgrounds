using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace sd_service.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class TextToImageController : ControllerBase
	{
		private readonly ILogger<TextToImageController> _logger;

		private readonly IConfiguration _configuration;

		private int Iterations => int.Parse(this._configuration["StableDiffusionValues:Iterations"]);

		private double ClassifierGuidance => double.Parse(this._configuration["StableDiffusionValues:ClassifierGuidance"]);

		private int ImageWidth => int.Parse(this._configuration["StableDiffusionValues:ImageWidth"]);

		private int ImageHeight => int.Parse(this._configuration["StableDiffusionValues:ImageHeight"]);

		private string AuthKey => this._configuration["StableDiffusionValues:AuthenticationSecret"];

		private bool EnableTestGetEndpoint => bool.Parse(this._configuration["StableDiffusionValues:EnableTestGetEndpoint"]);

		/// <summary>
		/// The endpoint where the StableDiffusion model is hosted via webui.
		/// </summary>
		private string SdHostEndpoint => this._configuration["StableDiffusionValues:StableDiffusionHostUri"];

		public TextToImageController(ILogger<TextToImageController> logger, IConfiguration configuration)
		{
			_logger = logger;
			_configuration = configuration;
		}

		/// <summary>
		/// Given a request that contains a text prompt,
		/// generate an image using and then return it as a base64 encoded string.
		/// </summary>
		/// <param name="request">The request containing the prompt.</param>
		/// <returns>The response containing the image.</returns>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Image))]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> Post(TextToImageRequest request)
		{
			this._logger.LogInformation($"Received POST requst for conversationId: {request.ConversationId}");

			if (request.AuthKey != this.AuthKey)
			{
				this._logger.LogCritical($"provided auth key was invalid: {request.AuthKey}");
				return Unauthorized();
			}

			var client = new SdClient(this.SdHostEndpoint, this._logger);

			var sdRequest = this.BuildRequest(request.Prompt);
			var imageResponse = await client.SendRequest(sdRequest);

			return Ok(new Image { Base64EncodedImage = imageResponse.data.First().First() });
		}

		[HttpGet("Test")]
		public async Task<Image> TestImageGeneration()
		{
			this._logger.LogInformation($"Received a GET request to the test endpoint.");

			if (!this.EnableTestGetEndpoint)
			{
				this._logger.LogCritical($"GET endpoint is not enabled in configuration, so not doing anything.");
				return null;
			}

			var client = new SdClient(this.SdHostEndpoint, this._logger);

			var sdRequest = this.BuildRequest(prompt: "a dog riding a skateboard");
			var imageResponse = await client.SendRequest(sdRequest);

			return new Image { Base64EncodedImage = imageResponse.data.First().First() };
		}

		private SdRequest BuildRequest(string prompt)
		{
			var sdRequest = new SdRequest
			{
				Prompt = prompt,
				Iterations = this.Iterations,
				ClassifierGuidance = this.ClassifierGuidance,
				Height = this.ImageHeight,
				Width = this.ImageWidth
			};

			return sdRequest;
		}

		[DataContract]
		public class TextToImageRequest
		{
			[DataMember(Name = "prompt")]
			public string Prompt { get; set; }

			[DataMember(Name = "conversationId")]
			public string ConversationId { get; set; }

			[DataMember(Name = "authKey")]
			public string AuthKey { get; set; }

			[DataMember(Name = "timestamp")]
			public DateTimeOffset TimeStamp { get; set; }
		}
	}
}