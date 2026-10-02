using Contracts;
using Contracts.AdminUserDtos;

namespace Domain.Features.AdminUserFeature;

/// <summary>Provides operations for managing administrative user accounts.</summary>
public interface IAdminUserService
{
    /// <summary>Gets all accounts that have not been soft-deleted.</summary>
    /// <param name="cancellationToken">Token used to cancel the database operation.</param>
    /// <returns>A successful result containing the account list.</returns>
    Task<Result<List<AdminUserListDto>>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>Gets a non-deleted account by its public identifier.</summary>
    /// <param name="publicId">Stable public identifier of the account.</param>
    /// <param name="cancellationToken">Token used to cancel the database operation.</param>
    /// <returns>The account when found, or a not-found result.</returns>
    Task<Result<AdminUserListDto>> GetByIdAsync(Guid publicId, CancellationToken cancellationToken);

    /// <summary>Creates an account and stores a hash of its password.</summary>
    /// <param name="request">Client-supplied account details.</param>
    /// <param name="cancellationToken">Token used to cancel the database operation.</param>
    /// <returns>The new account's public identifier, or a validation or duplicate result.</returns>
    Task<Result<Guid>> CreateAsync(CreateAdminUserDto request, CancellationToken cancellationToken);

    /// <summary>Updates a non-deleted account selected by its public identifier.</summary>
    /// <param name="publicId">Stable public identifier of the account to update.</param>
    /// <param name="request">Required username and email, plus an optional status change.</param>
    /// <param name="cancellationToken">Token used to cancel the database operation.</param>
    /// <returns>The updated account, or a not-found, validation, or duplicate result.</returns>
    Task<Result<AdminUserListDto>> UpdateAsync(
        Guid publicId,
        UpdateAdminUserDto request,
        CancellationToken cancellationToken
    );

    /// <summary>Soft-deletes and deactivates an account while preserving its audit history.</summary>
    /// <param name="publicId">Stable public identifier of the account to delete.</param>
    /// <param name="cancellationToken">Token used to cancel the database operation.</param>
    /// <returns>True when the account was deleted, or a not-found result.</returns>
    Task<Result<bool>> DeleteAsync(Guid publicId, CancellationToken cancellationToken);
}
