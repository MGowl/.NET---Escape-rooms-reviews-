namespace EscapeRoomReviews.ViewModels;

public class AutocompleteViewModel
{
    public string FieldName { get; set; } = string.Empty;
    public int? SelectedId { get; set; }
    public string SelectedName { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Placeholder { get; set; } = string.Empty;
    public string SearchUrl { get; set; } = string.Empty;
    public string HelperText { get; set; } = "Upisite najmanje 2 slova za pretragu.";
    public string EmptyText { get; set; } = "Nema rezultata.";
    public string LoadingText { get; set; } = "Ucitavanje...";
    public string ContainerClass { get; set; } = "mb-3 ajax-autocomplete";
    public bool IsRequired { get; set; }
    public string RequiredMessage { get; set; } = "Ovo polje je obavezno.";
    public int? MinValue { get; set; }
    public int? MaxValue { get; set; }
    public string RangeMessage { get; set; } = string.Empty;
}
