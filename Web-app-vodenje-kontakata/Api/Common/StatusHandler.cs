using Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Api.Common
{
    public static class StatusHandler
    {
        public static IActionResult HandleResult<T>(this ControllerBase controller, Result<T> result)
        {
            if (result.IsSuccess)
                return controller.Ok(result.Value);

            if (result.IsNotFound)
                return controller.NotFound(result.ErrorItems);

            return controller.BadRequest(result.ErrorItems);
        }

        public static IActionResult HandleCreated<T>(
            this ControllerBase controller, Result<T> result, string actionName, object routeValues)
        {
            if (result.IsSuccess)
                return controller.CreatedAtAction(actionName, routeValues, result.Value);

            if (result.IsNotFound)
                return controller.NotFound(result.ErrorItems);

            return controller.BadRequest(result.ErrorItems);
        }

        public static IActionResult HandleDeleted<T>(this ControllerBase controller, Result<T> result)
        {
            if (result.IsSuccess)
                return controller.NoContent();

            if (result.IsNotFound)
                return controller.NotFound(result.ErrorItems);

            return controller.BadRequest(result.ErrorItems);
        }
    }
}