using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Core;
using System;
using System.Text;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using FreshBoard.Data;
using Microsoft.EntityFrameworkCore;

namespace FreshBoard.Controllers
{
    [ApiController]
    [Route("interop")]
    public class InteropController : ControllerBase
    {
        private FreshBoardDbContext _context;
        public InteropController(FreshBoardDbContext context)
        {
            _context = context;
        }
        private static readonly ChakraCoreJsEngineFactory factory =
            new ChakraCoreJsEngineFactory(
                new ChakraCoreSettings
                {
                    EnableExperimentalFeatures = true
                });
        public class ValidateModel
        {
            public int Id { get; set; }
            public string Script { get; set; } = "";
            public string Param { get; set; } = "";
        }

        [Route("validate")]
        [HttpPost]
        public async Task<IActionResult> Validate([FromBody]ValidateModel model)
        {
            var problem = await _context.Problem.FirstOrDefaultAsync(i => i.Id == model.Id);
            if (problem == null)
            {
                return Content("Problem doesn't exist.");
            }
            using var engine = factory.CreateEngine();
            try
            {
                var script = engine.Precompile(problem.ServerSideScript ?? string.Empty);
                engine.Execute(script);
                var result = engine.CallFunction<string>("run", model.Param);
                engine.CollectGarbage();
                return Content(result);
            }
            catch (Exception exception)
            {
                var sb = new StringBuilder();
                Exception? ex = exception;
                while (ex != null)
                {
                    sb.AppendLine($"Exception: {ex.Message}");
                    sb.AppendLine($"Source: {ex.Source}");
                    sb.AppendLine($"Stack trace: {ex.StackTrace}");
                    if (ex.Data != null && ex.Data.Count != 0)
                    {
                        var keys = new List<object?>();
                        var values = new List<object?>();
                        foreach (var i in ex.Data.Keys) keys.Add(i);
                        foreach (var i in ex.Data.Values) keys.Add(i);
                        sb.AppendLine($"Addition data: ");
                        for (var i = 0; i < keys.Count; i++)
                            sb.AppendLine($"{keys[i]} = {values[i]}");
                    }
                    ex = ex.InnerException;
                }
                return Content(sb.ToString());
            }
        }
    }
}