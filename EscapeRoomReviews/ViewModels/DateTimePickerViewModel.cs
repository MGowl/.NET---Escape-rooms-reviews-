namespace EscapeRoomReviews.ViewModels;

public class DateTimePickerViewModel
{
    public string FieldName { get; set; } = string.Empty;

    public DateTime? Value { get; set; }

    public bool IsOptional { get; set; }
}
