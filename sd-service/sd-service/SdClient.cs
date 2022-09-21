namespace sd_service
{
	public class SdRequest
	{
		public string Prompt { get; set; } = "A dog riding a skateboard.";
		public int Iterations { get; set; } = 50;

		public int Width { get; set; } = 512;
		public int Height { get; set; } = 512;

		public double ClassifierGuidance { get; set; } = 7.5;
	}

	public class SdClient
	{
		private static readonly HttpClient _client = new HttpClient();

		private string _endpoint;

		private ILogger _logger;

		public SdClient(string uri, ILogger logger)
		{
			this._logger = logger;
			this._endpoint = uri;
		}

		/// <summary>
		/// Sends the request, and returns a base64 encoded png image as the response.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns>The base64 encoded png image response.</returns>
		public async Task<ImageResponse?> SendRequest(SdRequest request)
		{
			// Using length 11, because this is what I saw when inspecting traffic from the webapi.
			string sessionHash = Guid.NewGuid().ToString().Substring(startIndex: 0, length: 11);

			// 1. send the hello request.
			string serverProvidedSessionId;
			{
				this._logger.LogInformation($"Starting conversation with session hash {sessionHash}...");
				HttpResponseMessage? httpResponse = await _client.PostAsJsonAsync(this._endpoint, new HelloRequest(sessionHash));
				HelloResponse? responseBody = await httpResponse.Content.ReadFromJsonAsync<HelloResponse>();

				if (responseBody == null)
				{
					return null;
				}

				serverProvidedSessionId = responseBody.SessionId;

				this._logger.LogInformation($"Server responded with session ID {serverProvidedSessionId}");
			}

			// 2. Send the query to trigger SD.
			{
				this._logger.LogInformation($"Sending prompt to SD...");
				var sdQuery = new StableDiffusionQuery(
					sessionHash,
					serverProvidedSessionId,
					request.Prompt,
					request.Iterations,
					request.Width,
					request.Height,
					request.ClassifierGuidance);

				HttpResponseMessage? httpResponse = await _client.PostAsJsonAsync(this._endpoint, sdQuery);
				httpResponse.EnsureSuccessStatusCode();

				this._logger.LogInformation($"SD received the prompt.");
			}

			// 3. Send the update request query.
			{
				this._logger.LogInformation($"Sending fn 13 to SD...");
				var updateRequest = new HelloRequest(sessionHash)
				{
					data = new[] { serverProvidedSessionId },
					fn_index = 13
				};

				HttpResponseMessage? httpResponse = await _client.PostAsJsonAsync(this._endpoint, updateRequest);
				httpResponse.EnsureSuccessStatusCode();
				var responseText = await httpResponse.Content.ReadAsStringAsync();

				this._logger.LogInformation("SD responded to 13.");
			}

			// 4. Ping.
			{
				this._logger.LogInformation($"Sending fn 12 to SD...");
				var updateRequest = new HelloRequest(sessionHash)
				{
					data = new[] { serverProvidedSessionId },
					fn_index = 12
				};

				HttpResponseMessage? httpResponse = await _client.PostAsJsonAsync(this._endpoint, updateRequest);
				httpResponse.EnsureSuccessStatusCode();
				var responseText = await httpResponse.Content.ReadAsStringAsync();

				this._logger.LogInformation("SD responded to 12.");
			}

			// 5. Get resulting image.
			{
				this._logger.LogInformation($"Generation is done, asking for the image...");
				var updateRequest = new HelloRequest(sessionHash)
				{
					data = new[] { serverProvidedSessionId },
					fn_index = 4
				};

				HttpResponseMessage? httpResponse = await _client.PostAsJsonAsync(this._endpoint, updateRequest);
				httpResponse.EnsureSuccessStatusCode();
				var imageResponses = await httpResponse.Content.ReadAsStringAsync();
				var imageResponse = await httpResponse.Content.ReadFromJsonAsync<ImageResponse>();

				this._logger.LogInformation($"Received successful response after sending the query.");

				return imageResponse;
			}
		}

		private class HelloRequest
		{
			public int fn_index { get; set; } = 53;

			public string[] data { get; set; } = Array.Empty<string>();

			public string session_hash { get; }

			public HelloRequest(string sessionHash)
			{
				this.session_hash = sessionHash;
			}
		}

		private class HelloResponse
		{
			/// <summary>
			/// Will contain the unique session value that will be sent for the remainder of this request.
			/// </summary>
			public string[] data { get; set; }

			public string SessionId => this.data.First();

			public double duration { get; set; }

			public double average_duration { get; set; }
		}

		public class ImageResponse
		{
			/// <summary>
			/// Will contain the unique session value that will be sent for the remainder of this request.
			/// </summary>
			public string[][] data { get; set; }

			public double duration { get; set; }

			public double average_duration { get; set; }
		}

		private class StableDiffusionQuery
		{
			public int fn_index { get; } = 15;

			/// <summary>
			/// 0: text prompt
			/// 1: iterations
			/// 2: k_lms
			/// 3: ["Normalize Prompt Weights (ensure sum of weights add up to 1.0)","Save individual images","Save grid","Sort samples by prompt","Write sample info files"]
			/// 4: RealESRGAN_x4plus
			/// 5+: 0, 1, 1, 7.5, "", 512, 512, null, 0, "", SessionId, false, false, false, 3
			/// </summary>
			public object[] data { get; set; }

			public string session_hash { get; set; }

			public StableDiffusionQuery(
				string sessionHash,
				string sessionId,
				string textPrompt,
				int iterations,
				int imageWidth,
				int imageHeight,
				double classifierGuidance)
			{
				this.session_hash = sessionHash;

				var data = new List<object?>
				{
					textPrompt, // prompt
					iterations, // iterations
					"k_lms", // sampling method
					new[] { "Normalize Prompt Weights (ensure sum of weights add up to 1.0)","Save individual images","Save grid","Sort samples by prompt","Write sample info files" }, // boilerplate
					"RealESRGAN_x4plus", // boilerplate
					0, 1, 1, classifierGuidance, "", imageWidth, imageHeight, null, 0, "", sessionId, false, false, false, 3 // boilerplate

				};

				this.data = data.ToArray();
			}
		}
	}
}
