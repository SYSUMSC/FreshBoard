using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreshBoard.Data;
using JavaScriptEngineSwitcher.ChakraCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshBoard.Controllers
{
    [ApiController]
    [Route("interop")]
    public class InteropController : ControllerBase
    {
        private readonly FreshBoardDbContext _context;
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
                    if (ex.Data.Count != 0)
                    {
                        var keys = ex.Data.Keys.Cast<object>().ToList();
                        var values = ex.Data.Values.Cast<object>().ToList();
                        sb.AppendLine("Addition data: ");
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