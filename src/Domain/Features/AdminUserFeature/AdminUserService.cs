using Contracts;
using Contracts.AdminUserDtos;
using Database.AppDbContextModels;
using Database.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Constants;

namespace Domain.Features.AdminUserFeature;

/// <summary>Implements database-backed operations for administrative user accounts.</summary>
public sealed class AdminUserService : IAdminUserService
{
    #region Dependencies

    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher<AdminUser> _passwordHasher;
    private readonly ILogger<AdminUserService> _logger;

    #endregion

    #region Constructor

    /// <summary>Creates the service with its database context and password hasher.</summary>
    /// <param name="dbContext">Database context used to persist account changes.</param>
    /// <param name="passwordHasher">Password hasher used before storing passwords.</param>
    /// <param name="logger">Logger for administrative account operations.</param>
    public AdminUserService(
        AppDbContext dbContext,
        IPasswordHasher<AdminUser> passwordHasher,
        ILogger<AdminUserService> logger
    )
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    #endregion

    #region GetAllAsync

    /// <inheritdoc />
    public async Task<Result<List<AdminUserListDto>>> GetAllAsync(
        CancellationToken cancellationToken
    )
    {
        var users = await _dbContext
            .AdminUsers.AsNoTracking()
            .Where(user => !user.IsDeleted)
            .OrderByDescending(user => user.Id)
            .Select(user => new AdminUserListDto
            {
                PublicId = user.PublicId,
                UserName = user.UserName,
                Email = user.Email,
                IsActive = user.IsActive,
            })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return Result<List<AdminUserListDto>>.Success(users);
    }

    #endregion

    #region GetByIdAsync

    /// <inheritdoc />
    public async Task<Result<AdminUserListDto>> GetByIdAsync(
        Guid publicId,
        CancellationToken cancellationToken
    )
    {
        if (publicId == Guid.Empty)
            return LogFailure<AdminUserListDto>(
                nameof(GetByIdAsync),
                ResultType.ValidationError,
                ErrorCodes.InvalidUserId
            );

        var user = await _dbContext
            .AdminUsers.AsNoTracking()
            .Where(user => user.PublicId == publicId && !user.IsDeleted)
            .Select(user => new AdminUserListDto
            {
                PublicId = user.PublicId,
                UserName = user.UserName,
                Email = user.Email,
                IsActive = user.IsActive,
            })
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        return user is null
            ? LogFailure<AdminUserListDto>(
                nameof(GetByIdAsync),
                ResultType.NotFound,
                ErrorCodes.AdminUserNotFound
            )
            : Result<AdminUserListDto>.Success(user);
    }

    #endregion

    #region CreateAsync

    /// <inheritdoc />
    public async Task<Result<Guid>> CreateAsync(
        CreateAdminUserDto request,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.UserName))
            return LogFailure<Guid>(
                nameof(CreateAsync),
                ResultType.ValidationError,
                ErrorCodes.UserNameRequired
            );

        if (string.IsNullOrWhiteSpace(request.Email))
            return LogFailure<Guid>(
                nameof(CreateAsync),
                ResultType.ValidationError,
                ErrorCodes.EmailRequired
            );

        if (string.IsNullOrWhiteSpace(request.Password))
            return LogFailure<Guid>(
                nameof(CreateAsync),
                ResultType.ValidationError,
                ErrorCodes.PasswordRequired
            );

        var normalizedUserName = request.UserName.Trim();
        var normalizedEmail = request.Email.Trim();
        var duplicateErrorCode = await FindDuplicateErrorCodeAsync(
                normalizedUserName,
                normalizedEmail,
                null,
                cancellationToken
            )
            .ConfigureAwait(false);

        if (duplicateErrorCode is not null)
            return LogFailure<Guid>(
                nameof(CreateAsync),
                ResultType.DuplicateRecord,
                duplicateErrorCode
            );

        var user = new AdminUser { PublicId = Guid.NewGuid() };
        try
        {
            user.ChangeUserName(normalizedUserName);
            user.ChangeEmail(normalizedEmail);

            ApplyActiveStatus(user, request.IsActive);
        }
        catch (DomainException exception)
        {
            return LogFailure<Guid>(
                nameof(CreateAsync),
                ResultType.ValidationError,
                exception.Code
            );
        }

        user.ChangePasswordHash(_passwordHasher.HashPassword(user, request.Password));
        _dbContext.AdminUsers.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Created admin user {AdminUserId}", user.Id);
        return Result<Guid>.Success(user.PublicId);
    }

    #endregion

    #region UpdateAsync

    /// <inheritdoc />
    public async Task<Result<AdminUserListDto>> UpdateAsync(
        Guid publicId,
        UpdateAdminUserDto request,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        if (publicId == Guid.Empty)
            return LogFailure<AdminUserListDto>(
                nameof(UpdateAsync),
                ResultType.ValidationError,
                ErrorCodes.InvalidUserId
            );

        var user = await _dbContext
            .AdminUsers.FirstOrDefaultAsync(
                account => account.PublicId == publicId && !account.IsDeleted,
                cancellationToken
            )
            .ConfigureAwait(false);

        if (user is null)
            return LogFailure<AdminUserListDto>(
                nameof(UpdateAsync),
                ResultType.NotFound,
                ErrorCodes.AdminUserNotFound
            );

        if (string.IsNullOrWhiteSpace(request.UserName))
            return LogFailure<AdminUserListDto>(
                nameof(UpdateAsync),
                ResultType.ValidationError,
                ErrorCodes.UserNameRequired
            );

        if (string.IsNullOrWhiteSpace(request.Email))
            return LogFailure<AdminUserListDto>(
                nameof(UpdateAsync),
                ResultType.ValidationError,
                ErrorCodes.EmailRequired
            );

        var normalizedUserName = request.UserName.Trim();
        var normalizedEmail = request.Email.Trim();
        var duplicateErrorCode = await FindDuplicateErrorCodeAsync(
                normalizedUserName,
                normalizedEmail,
                user.Id,
                cancellationToken
            )
            .ConfigureAwait(false);

        if (duplicateErrorCode is not null)
            return LogFailure<AdminUserListDto>(
                nameof(UpdateAsync),
                ResultType.DuplicateRecord,
                duplicateErrorCode
            );

        user.ChangeUserName(normalizedUserName);
        user.ChangeEmail(normalizedEmail);
        ApplyActiveStatus(user, request.IsActive);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Updated admin user {AdminUserId}", user.Id);
        return Result<AdminUserListDto>.Success(ToListDto(user));
    }

    #endregion

    #region DeleteAsync

    /// <inheritdoc />
    public async Task<Result<bool>> DeleteAsync(Guid publicId, CancellationToken cancellationToken)
    {
        if (publicId == Guid.Empty)
            return LogFailure<bool>(
                nameof(DeleteAsync),
                ResultType.ValidationError,
                ErrorCodes.InvalidUserId
            );

        var user = await _dbContext
            .AdminUsers.FirstOrDefaultAsync(
                account => account.PublicId == publicId && !account.IsDeleted,
                cancellationToken
            )
            .ConfigureAwait(false);

        if (user is null)
            return LogFailure<bool>(
                nameof(DeleteAsync),
                ResultType.NotFound,
                ErrorCodes.AdminUserNotFound
            );

        user.IsDeleted = true;
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Soft-deleted admin user {AdminUserId}", user.Id);
        return Result<bool>.Success(true);
    }

    #endregion

    #region LogFailure

    private Result<T> LogFailure<T>(string operation, ResultType type, string errorCode)
    {
        _logger.LogWarning(
            "Admin user operation {Operation} failed with {ResultType} and {ErrorCode}",
            operation,
            type,
            errorCode
        );

        return Result<T>.Failure(type, errorCode);
    }

    #endregion

    #region FindDuplicateErrorCodeAsync

    private async Task<string?> FindDuplicateErrorCodeAsync(
        string userName,
        string email,
        long? excludedUserId,
        CancellationToken cancellationToken
    )
    {
        var duplicateUserName = await _dbContext
            .AdminUsers.AnyAsync(
                existing =>
                    existing.UserName == userName
                    && (!excludedUserId.HasValue || existing.Id != excludedUserId.Value),
                cancellationToken
            )
            .ConfigureAwait(false);

        if (duplicateUserName)
            return ErrorCodes.UserNameAlreadyExists;

        var duplicateEmail = await _dbContext
            .AdminUsers.AnyAsync(
                existing =>
                    existing.Email == email
                    && (!excludedUserId.HasValue || existing.Id != excludedUserId.Value),
                cancellationToken
            )
            .ConfigureAwait(false);

        return duplicateEmail ? ErrorCodes.EmailAlreadyExists : null;
    }

    #endregion

    #region ApplyActiveStatus

    private static void ApplyActiveStatus(AdminUser user, bool? desiredActive)
    {
        if (desiredActive is not bool active || active == user.IsActive)
            return;

        if (active)
            user.Activate();
        else
            user.Deactivate();
    }

    #endregion

    #region ToListDto

    private static AdminUserListDto ToListDto(AdminUser user) =>
        new()
        {
            PublicId = user.PublicId,
            UserName = user.UserName,
            Email = user.Email,
            IsActive = user.IsActive,
        };

    #endregion
}
