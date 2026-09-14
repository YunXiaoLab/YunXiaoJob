using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Application.Interfaces.Services;
using YunXiaoJob.Domain.Entities;
using System.Security.Cryptography;

namespace YunXiaoJob.Application.UseCases.Accounts;

public class StartRegistrationChallengeUseCase
{
    private readonly IUserRepository _users; private readonly IPasswordService _passwords; private readonly IEmailService _email; private readonly IUnitOfWork _unit;
    public StartRegistrationChallengeUseCase(IUserRepository users, IPasswordService passwords, IEmailService email, IUnitOfWork unit) { _users=users;_passwords=passwords;_email=email;_unit=unit; }
    public async Task<RegistrationChallengeResponse> ExecuteAsync(RegisterCandidateRequest request, CancellationToken ct = default)
    {
        var email=request.Email.Trim().ToLowerInvariant(); if(await _users.ExistsByEmailAsync(email,ct)) throw new InvalidOperationException("Email is already in use.");
        var now=DateTime.UtcNow; var existing=await _users.GetRegistrationChallengeByEmailAsync(email,ct);
        if(existing is not null && existing.ExpiresAtUtc>now && now<existing.LastSentAtUtc.AddSeconds(60)) throw new InvalidOperationException("Please wait before requesting another OTP.");
        var otp=RandomNumberGenerator.GetInt32(100000,1000000).ToString(); var challenge=existing??new RegistrationChallenge{Email=email};
        challenge.FullName=request.FullName.Trim();challenge.PasswordHash=_passwords.HashPassword(request.Password);challenge.OtpHash=_passwords.HashPassword(otp);challenge.ExpiresAtUtc=now.AddMinutes(5);challenge.LastSentAtUtc=now;challenge.AttemptCount=0;challenge.ResendCount=existing is null?0:existing.ResendCount+1;challenge.UpdatedAtUtc=now;
        if(challenge.ResendCount>5)throw new InvalidOperationException("OTP resend limit exceeded."); if(existing is null)await _users.AddRegistrationChallengeAsync(challenge,ct);await _unit.SaveChangesAsync(ct);await _email.SendRegistrationOtpAsync(email,otp,ct);
        return new RegistrationChallengeResponse(challenge.Id,challenge.ExpiresAtUtc,challenge.LastSentAtUtc.AddSeconds(60));
    }
}

public class VerifyRegistrationOtpUseCase
{
    private readonly IUserRepository _users; private readonly ICandidateRepository _candidates; private readonly IPasswordService _passwords; private readonly IUnitOfWork _unit;
    public VerifyRegistrationOtpUseCase(IUserRepository users, ICandidateRepository candidates, IPasswordService passwords, IUnitOfWork unit){_users=users;_candidates=candidates;_passwords=passwords;_unit=unit;}
    public async Task<UserResponse> ExecuteAsync(VerifyRegistrationOtpRequest request,CancellationToken ct=default)
    { var challenge=await _users.GetRegistrationChallengeByIdAsync(request.ChallengeId,ct)??throw new KeyNotFoundException("Registration challenge was not found."); if(challenge.ExpiresAtUtc<=DateTime.UtcNow)throw new InvalidOperationException("OTP has expired."); if(++challenge.AttemptCount>5)throw new InvalidOperationException("OTP attempt limit exceeded."); if(!_passwords.VerifyPassword(request.Otp,challenge.OtpHash)){await _unit.SaveChangesAsync(ct);throw new InvalidOperationException("OTP is invalid.");} if(await _users.ExistsByEmailAsync(challenge.Email,ct))throw new InvalidOperationException("Email is already in use."); var user=new User{Email=challenge.Email,FullName=challenge.FullName,PasswordHash=challenge.PasswordHash};await _users.AddAsync(user,ct);await _candidates.AddProfileAsync(new CandidateProfile{UserId=user.Id},ct);_users.RemoveRegistrationChallenge(challenge);await _unit.SaveChangesAsync(ct);return new UserResponse(user.Id,user.Email,user.FullName,user.PhoneNumber,user.PlatformRole,user.IsActive); }
}

public class ResendRegistrationOtpUseCase
{
    private readonly IUserRepository _users; private readonly IPasswordService _passwords; private readonly IEmailService _email; private readonly IUnitOfWork _unit;
    public ResendRegistrationOtpUseCase(IUserRepository users,IPasswordService passwords,IEmailService email,IUnitOfWork unit){_users=users;_passwords=passwords;_email=email;_unit=unit;}
    public async Task<RegistrationChallengeResponse> ExecuteAsync(ResendRegistrationOtpRequest request,CancellationToken ct=default)
    { var challenge=await _users.GetRegistrationChallengeByIdAsync(request.ChallengeId,ct)??throw new KeyNotFoundException("Registration challenge was not found.");var now=DateTime.UtcNow;if(challenge.ExpiresAtUtc<=now)throw new InvalidOperationException("OTP challenge has expired. Start registration again.");if(now<challenge.LastSentAtUtc.AddSeconds(60))throw new InvalidOperationException("Please wait before requesting another OTP.");if(++challenge.ResendCount>5)throw new InvalidOperationException("OTP resend limit exceeded.");var otp=RandomNumberGenerator.GetInt32(100000,1000000).ToString();challenge.OtpHash=_passwords.HashPassword(otp);challenge.LastSentAtUtc=now;challenge.AttemptCount=0;challenge.UpdatedAtUtc=now;await _unit.SaveChangesAsync(ct);await _email.SendRegistrationOtpAsync(challenge.Email,otp,ct);return new RegistrationChallengeResponse(challenge.Id,challenge.ExpiresAtUtc,now.AddSeconds(60)); }
}
