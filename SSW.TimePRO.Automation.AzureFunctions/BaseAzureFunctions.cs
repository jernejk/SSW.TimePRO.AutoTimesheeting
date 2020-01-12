using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SSW.TimePRO.Automation.AzureFunctions
{
    public class BaseAzureFunctions
    {
        public BaseAzureFunctions(IMediator mediator)
        {
            Mediator = mediator;
        }

        public IMediator Mediator { get; }

        public Task<IActionResult> RunRequest<T>(IRequest<T> request, CancellationToken ct)
        {
            return RunOrReportValidationErrors(() => Mediator.Send(request, ct));
        }

        public async Task<IActionResult> RunOrReportValidationErrors<T>(Func<Task<T>> action)
        {
            try
            {
                // TODO: Replace it with attribute once they are out of preview.
                var result = await action();
                return new JsonResult(result);
            }
            catch (ValidationException validationException)
            {
                var result = new
                {
                    message = "Validation failed.",
                    errors = validationException.Errors.Select(x => new
                    {
                        x.PropertyName,
                        x.ErrorMessage,
                        x.ErrorCode
                    })
                };

                return new BadRequestObjectResult(result);
            }
        }
    }
}
