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

    /// <summary>Gets a non-deleted administrative user by its public identifier.</summary>
    /// <param name="publicId">Stable public identifier of the administrative user.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpGet("{publicId:guid}", Name = nameof(GetByIdAsync))]
    public async Task<IActionResult> GetByIdAsync(
        [FromRoute] Guid publicId,
        CancellationToken cancellationToken
    ) =>
        Execute(
            await _adminUserService.GetByIdAsync(publicId, cancellationToken).ConfigureAwait(false)
        );

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
        var result = await _adminUserService
            .CreateAsync(request, cancellationToken)
            .ConfigureAwait(false);
        if (!result.IsSuccess)
            return Execute(result);

        return CreatedAtRoute(nameof(GetByIdAsync), new { publicId = result.Data }, result);
    }

    #endregion

    // #region UpdateAsync

    // /// <summary>Updates an administrative user's profile and optional active status.</summary>
    // /// <param name="publicId">Stable public identifier of the administrative user.</param>
    // /// <param name="request">Profile fields and optional status update.</param>
    // /// <param name="cancellationToken">Token used to cancel the request.</param>
    // [HttpPut("{publicId:guid}")]
    // public async Task<IActionResult> UpdateAsync(
    //     [FromRoute] Guid publicId,
    //     [FromBody] UpdateAdminUserDto request,
    //     CancellationToken cancellationToken
    // ) =>
    //     Execute(
    //         await _adminUserService
    //             .UpdateAsync(publicId, request, cancellationToken)
    //             .ConfigureAwait(false)
    //     );

    // #endregion

    #region PatchUpdateAsync

    /// <summary>Partially updates an administrative user's profile or active status.</summary>
    /// <param name="publicId">Stable public identifier of the administrative user.</param>
    /// <param name="request">Optional fields to update.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpPatch("{publicId:guid}")]
    public async Task<IActionResult> PatchUpdateAsync(
        [FromRoute] Guid publicId,
        [FromBody] PatchUpdateAdminUserDto request,
        CancellationToken cancellationToken
    ) =>
        Execute(
            await _adminUserService
                .PatchUpdateAsync(publicId, request, cancellationToken)
                .ConfigureAwait(false)
        );

    #endregion

    #region DeleteAsync

    /// <summary>Soft-deletes an administrative user.</summary>
    /// <param name="publicId">Stable public identifier of the administrative user.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpDelete("{publicId:guid}")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid publicId,
        CancellationToken cancellationToken
    ) =>
        Execute(
            await _adminUserService.DeleteAsync(publicId, cancellationToken).ConfigureAwait(false)
        );

    #endregion

    #region ChangePasswordAsync

    /// <summary>Changes an administrative user's password.</summary>
    /// <param name="publicId">Stable public identifier of the administrative user.</param>
    /// <param name="request">New password that must satisfy the account security policy.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpPut("{publicId:guid}/password")]
    public async Task<IActionResult> ChangePasswordAsync(
        [FromRoute] Guid publicId,
        [FromBody] ChangePasswordDto request,
        CancellationToken cancellationToken
    ) =>
        Execute(
            await _adminUserService
                .ChangePasswordAsync(publicId, request, cancellationToken)
                .ConfigureAwait(false)
        );

    #endregion
}
