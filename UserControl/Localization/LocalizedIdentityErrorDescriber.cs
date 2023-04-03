using System.Globalization;
using Microsoft.AspNetCore.Identity;

namespace UserControl.Localization;

public class LocalizedIdentityErrorDescriber : IdentityErrorDescriber
{
    private readonly IdentityErrorDescriber describer_;

    public LocalizedIdentityErrorDescriber(IIdentityErrorDescriberFactory factory)
    {
        describer_ = factory.Create(CultureInfo.CurrentUICulture);
    }

    public override IdentityError ConcurrencyFailure()
    {
        return describer_.ConcurrencyFailure();
    }

    public override IdentityError DefaultError()
    {
        return describer_.DefaultError();
    }

    public override IdentityError DuplicateEmail(string email)
    {
        return describer_.DuplicateEmail(email);
    }

    public override IdentityError DuplicateRoleName(string role)
    {
        return describer_.DuplicateRoleName(role);
    }

    public override IdentityError DuplicateUserName(string userName)
    {
        return describer_.DuplicateUserName(userName);
    }

    public override IdentityError InvalidEmail(string email)
    {
        return describer_.InvalidEmail(email);
    }

    public override IdentityError InvalidRoleName(string role)
    {
        return describer_.InvalidRoleName(role);
    }

    public override IdentityError InvalidToken()
    {
        return describer_.InvalidToken();
    }

    public override IdentityError InvalidUserName(string userName)
    {
        return describer_.InvalidUserName(userName);
    }

    public override IdentityError LoginAlreadyAssociated()
    {
        return describer_.LoginAlreadyAssociated();
    }

    public override IdentityError PasswordMismatch()
    {
        return describer_.PasswordMismatch();
    }

    public override IdentityError PasswordRequiresDigit()
    {
        return describer_.PasswordRequiresDigit();
    }

    public override IdentityError PasswordRequiresLower()
    {
        return describer_.PasswordRequiresLower();
    }

    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return describer_.PasswordRequiresNonAlphanumeric();
    }

    public override IdentityError PasswordRequiresUpper()
    {
        return describer_.PasswordRequiresUpper();
    }

    public override IdentityError PasswordTooShort(int length)
    {
        return describer_.PasswordTooShort(length);
    }

    public override IdentityError UserAlreadyHasPassword()
    {
        return describer_.UserAlreadyHasPassword();
    }

    public override IdentityError UserAlreadyInRole(string role)
    {
        return describer_.UserAlreadyInRole(role);
    }

    public override IdentityError UserLockoutNotEnabled()
    {
        return describer_.UserLockoutNotEnabled();
    }

    public override IdentityError UserNotInRole(string role)
    {
        return describer_.UserNotInRole(role);
    }
}
