using Contracts.AdminUserDtos;
using Microsoft.AspNetCore.Mvc;
using Shared.Base;

namespace Domain.Features.AdminUserFeature;

/// <summary>Exposes HTTP endpoints for managing administrative user accounts.</summary>
[Route("api/admin_user")]
public sealed class AdminUserController : BaseController
{
    #region Dependencies

    private readonly IAdminUserService _adminUserService;

    #endregion

    #region Constructor

    /// <summary>Creates the controller with its administrative user service.</summary>
    /// <param name="adminUserService">Service that performs administrative user operations.</param>
    public AdminUserController(IAdminUserService adminUserService)
    {
        _adminUserService = adminUserService;
    }

    #endregion

    #region GetAllAsync

    /// <summary>Gets all administrative users that have not been deleted.</summary>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken) =>
        Execute(await _adminUserService.GetAllAsync(cancellationToken).ConfigureAwait(false));

    #endregion

    #region GetByIdAsync

    /// <summary>Gets a non-deleted administrative user by its database identifier.</summary>
    /// <param name="id">Database identifier of the administrative user.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpGet("{id:long}", Name = nameof(GetByIdAsync))]
    public async Task<IActionResult> GetByIdAsync(
        [FromRoute] long id,
        CancellationToken cancellationToken
    ) => Execute(await _adminUserService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false));

    #endregion

    #region CreateAsync

    /// <summary>Creates an administrative user.</summary>
    /// <param name="request">Account details for the new user.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateAdminUserDto request,
        CancellationToken cancellationToken
    )
    {
        var result = await _adminUserService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        if (!result.IsSuccess)
            return Execute(result);

        return CreatedAtRoute(nameof(GetByIdAsync), new { id = result.Data }, result);
    }

    #endregion

    #region UpdateAsync

    /// <summary>Updates an administrative user's profile and optional active status.</summary>
    /// <param name="id">Database identifier of the administrative user.</param>
    /// <param name="request">Profile fields and optional status update.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] long id,
        [FromBody] UpdateAdminUserDto request,
        CancellationToken cancellationToken
    ) => Execute(await _adminUserService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false));

    #endregion

    #region DeleteAsync

    /// <summary>Soft-deletes an administrative user.</summary>
    /// <param name="id">Database identifier of the administrative user.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] long id,
        CancellationToken cancellationToken
    ) => Execute(await _adminUserService.DeleteAsync(id, cancellationToken).ConfigureAwait(false));

    #endregion
}
