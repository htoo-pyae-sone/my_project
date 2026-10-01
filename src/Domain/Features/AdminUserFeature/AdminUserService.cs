using Contracts;
using Contracts.AdminUserDtos;
using Database.AppDbContextModels;
using Database.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;

namespace Domain.Features.AdminUserFeature;

/// <summary>Implements database-backed operations for administrative user accounts.</summary>
public sealed class AdminUserService : IAdminUserService
{
    #region Dependencies

    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher<AdminUser> _passwordHasher;

    #endregion

    #region Constructor

    /// <summary>Creates the service with its database context and password hasher.</summary>
    /// <param name="dbContext">Database context used to persist account changes.</param>
    /// <param name="passwordHasher">Password hasher used before storing passwords.</param>
    public AdminUserService(AppDbContext dbContext, IPasswordHasher<AdminUser> passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    #endregion

    #region GetAllAsync

    /// <inheritdoc />
    public async Task<Result<List<AdminUserListDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await _dbContext.AdminUsers
            .AsNoTracking()
            .Where(user => !user.IsDeleted)
            .OrderByDescending(user => user.Id)
            .Select(user => new AdminUserListDto
            {
                Id = user.Id,
                PublicId = user.PublicId,
                UserName = user.UserName,
                Email = user.Email,
                IsActive = user.IsActive
            })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return Result<List<AdminUserListDto>>.Success(users);
    }

    #endregion

    #region GetByIdAsync

    /// <inheritdoc />
    public async Task<Result<AdminUserListDto>> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        if (id <= 0)
            return Result<AdminUserListDto>.Failure(ResultType.ValidationError, ErrorCodes.InvalidUserId);

        var user = await _dbContext.AdminUsers
            .AsNoTracking()
            .Where(user => user.Id == id && !user.IsDeleted)
            .Select(user => new AdminUserListDto
            {
                Id = user.Id,
                PublicId = user.PublicId,
                UserName = user.UserName,
                Email = user.Email,
                IsActive = user.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        return user is null
            ? Result<AdminUserListDto>.Failure(ResultType.NotFound, ErrorCodes.AdminUserNotFound)
            : Result<AdminUserListDto>.Success(user);
    }

    #endregion

    #region CreateAsync

    /// <inheritdoc />
    public async Task<Result<long>> CreateAsync(
        CreateAdminUserDto request,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Password))
            return Result<long>.Failure(ResultType.ValidationError, ErrorCodes.PasswordRequired);

        var user = new AdminUser { PublicId = Guid.NewGuid() };
        try
        {
            user.ChangeUserName(request.UserName);
            user.ChangeEmail(request.Email);

            if (!request.IsActive)
                user.Deactivate();
        }
        catch (DomainException exception)
        {
            return Result<long>.Failure(ResultType.ValidationError, exception.Code);
        }

        if (await _dbContext.AdminUsers.AnyAsync(
                existing => existing.UserName == user.UserName,
                cancellationToken
            ).ConfigureAwait(false))
        {
            return Result<long>.Failure(ResultType.DuplicateRecord, ErrorCodes.UserNameAlreadyExists);
        }

        if (await _dbContext.AdminUsers.AnyAsync(
                existing => existing.Email == user.Email,
                cancellationToken
            ).ConfigureAwait(false))
        {
            return Result<long>.Failure(ResultType.DuplicateRecord, ErrorCodes.EmailAlreadyExists);
        }

        user.ChangePasswordHash(_passwordHasher.HashPassword(user, request.Password));
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = user.CreatedAt;
        _dbContext.AdminUsers.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result<long>.Success(user.Id);
    }

    #endregion

    #region UpdateAsync

    /// <inheritdoc />
    public async Task<Result<AdminUserListDto>> UpdateAsync(
        long id,
        UpdateAdminUserDto request,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        if (id <= 0)
            return Result<AdminUserListDto>.Failure(ResultType.ValidationError, ErrorCodes.InvalidUserId);

        var user = await _dbContext.AdminUsers
            .FirstOrDefaultAsync(account => account.Id == id && !account.IsDeleted, cancellationToken)
            .ConfigureAwait(false);

        if (user is null)
            return Result<AdminUserListDto>.Failure(ResultType.NotFound, ErrorCodes.AdminUserNotFound);

        if (string.IsNullOrWhiteSpace(request.UserName))
            return Result<AdminUserListDto>.Failure(ResultType.ValidationError, ErrorCodes.UserNameRequired);

        if (string.IsNullOrWhiteSpace(request.Email))
            return Result<AdminUserListDto>.Failure(ResultType.ValidationError, ErrorCodes.EmailRequired);

        var normalizedUserName = request.UserName.Trim();
        if (await _dbContext.AdminUsers.AnyAsync(
                existing => existing.Id != id && existing.UserName == normalizedUserName,
                cancellationToken
            ).ConfigureAwait(false))
        {
            return Result<AdminUserListDto>.Failure(ResultType.DuplicateRecord, ErrorCodes.UserNameAlreadyExists);
        }

        var normalizedEmail = request.Email.Trim();
        if (await _dbContext.AdminUsers.AnyAsync(
                existing => existing.Id != id && existing.Email == normalizedEmail,
                cancellationToken
            ).ConfigureAwait(false))
        {
            return Result<AdminUserListDto>.Failure(ResultType.DuplicateRecord, ErrorCodes.EmailAlreadyExists);
        }

        user.ChangeUserName(normalizedUserName);
        user.ChangeEmail(normalizedEmail);

        if (request.IsActive is bool desiredActive && desiredActive != user.IsActive)
        {
            if (desiredActive)
                user.Activate();
            else
                user.Deactivate();
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result<AdminUserListDto>.Success(ToListDto(user));
    }

    #endregion

    #region DeleteAsync

    /// <inheritdoc />
    public async Task<Result<bool>> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        if (id <= 0)
            return Result<bool>.Failure(ResultType.ValidationError, ErrorCodes.InvalidUserId);

        var user = await _dbContext.AdminUsers
            .FirstOrDefaultAsync(account => account.Id == id && !account.IsDeleted, cancellationToken)
            .ConfigureAwait(false);

        if (user is null)
            return Result<bool>.Failure(ResultType.NotFound, ErrorCodes.AdminUserNotFound);

        user.IsDeleted = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result<bool>.Success(true);
    }

    #endregion

    #region ToListDto

    private static AdminUserListDto ToListDto(AdminUser user) => new()
    {
        Id = user.Id,
        PublicId = user.PublicId,
        UserName = user.UserName,
        Email = user.Email,
        IsActive = user.IsActive
    };

    #endregion
}
