namespace UserControl.Localization;

public class LocalizationOptions
{
    public string DefaultCulture { get; set; } = default!;
    public IEnumerable<string> SupportedCultures { get; set; } = default!;
    public IDictionary<string, Describer> IdentityErrorDescribers { get; set; } = default!;

    public class Describer
    {
        public string Assembly { get; set; } = default!;
        public string Type { get; set; } = default!;
    }
}
