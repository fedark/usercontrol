using System.Globalization;
using Microsoft.AspNetCore.Identity;

namespace UserControl.Localization;
public interface IIdentityErrorDescriberFactory
{
    IdentityErrorDescriber Create(CultureInfo? culture);
}