using CSScriptLib;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CodeExecution.Controllers;
[ApiController]
[Route("[controller]")]

public class CodeExecutionController : ControllerBase
{
	[HttpPost(Name = "Execute")]
	public async Task<IActionResult> Execute([FromBody] Request request)
	{
		try
		{
			dynamic script = CSScript.Evaluator.LoadCode(request.Code);
			dynamic className = Activator.CreateInstance(script.GetType());

			var method = className.GetType().GetMethod(request.MethodName);

			if (method != null)
			{
				// Convert arguments to their target types
				var parameters = method.GetParameters();
				var arguments = new object[parameters.Length];
				for (var i = 0; i < parameters.Length; i++)
				{
					if (parameters[i].ParameterType == typeof(string) && request.Arguments[i] is JsonElement)
					{
						var jsonElement = (JsonElement)request.Arguments[i];
						arguments[i] = jsonElement.GetString();
					}
					else
					{
						arguments[i] = request.Arguments[i];
					}
				}

				var result = method.Invoke(className, arguments);
				return Ok(result);
			}

			return BadRequest("Method not found.");
		}
		catch (Exception e)
		{
			return BadRequest(e.Message);
		}
	}
}

public record Request(string Code, string MethodName, List<object> Arguments);