using Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Api.Common
{
    public static class StatusHandler
    {
        // GET - vraća 200 s podacima, ili 404 ako nije pronađeno
        public static IActionResult HandleResult<T>(this ControllerBase controller, Result<T> result)
        {
            if (result.IsSuccess)
                return controller.Ok(result.Value);

            if (result.IsNotFound)
                return controller.NotFound(result.ErrorItems);

            return controller.BadRequest(result.ErrorItems);
        }

        // POST (kreiranje) - vraća 201 Created s Location headerom, ili 400 ako je unos loš
        public static IActionResult HandleCreated<T>(
            this ControllerBase controller, Result<T> result, string actionName, object routeValues)
        {
            if (result.IsSuccess)
                return controller.CreatedAtAction(actionName, routeValues, result.Value);

            if (result.IsNotFound)
                return controller.NotFound(result.ErrorItems);

            return controller.BadRequest(result.ErrorItems);
        }

        // DELETE - vraća 204 No Content (bez tijela), ili 404 ako nije pronađeno
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