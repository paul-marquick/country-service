namespace CountryService.BlazorAdminApp.Components.DataGrid;

public record Column
{
    public required string Name { get; set; }
    public required string HeaderText { get; set; }
    public int? Width { get; set; }
    //public bool IsVisible { get; set; } = true;
    //public bool IsSortable { get; set; } = true;
    //public bool IsFilterable { get; set; } = true;
    //public bool IsEditable { get; set; } = false;
    //public string? Format { get; set; }
    //public string? CssClass { get; set; }
    //public string? HeaderCssClass { get; set; }
}
