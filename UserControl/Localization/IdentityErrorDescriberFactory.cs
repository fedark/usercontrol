using System.Collections.Concurrent;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace UserControl.Localization;

public class IdentityErrorDescriberFactory : IIdentityErrorDescriberFactory
{
    private readonly ConcurrentDictionary<string, IdentityErrorDescriber> describerCache_ = new();
    private readonly IOptions<LocalizationOptions> options_;

    public IdentityErrorDescriberFactory(IOptions<LocalizationOptions> options)
    {
        options_ = options;
    }

    public IdentityErrorDescriber Create(CultureInfo? culture)
    {
        culture ??= CultureInfo.InvariantCulture;

        if (!describerCache_.TryGetValue(culture.Name, out var describer))
        {
            var cultureDescribers = options_.Value.IdentityErrorDescribers;
            if (!cultureDescribers.TryGetValue(culture.Name, out var describerOptions))
            {
                describer = new IdentityErrorDescriber();
            }
            else
            {
                var describerInstance = Activator.CreateInstance(describerOptions.Assembly, describerOptions.Type);
                describer = describerInstance?.Unwrap() as IdentityErrorDescriber;

                if (describer is null)
                {
                    throw new Exception($"Failed to create identity error describer for culture '{culture.Name}'");
                }
            }

            describerCache_[culture.Name] = describer;
        }

        return describer;
    }
}
