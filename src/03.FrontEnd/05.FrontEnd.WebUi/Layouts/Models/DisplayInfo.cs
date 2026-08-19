using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EBVL.FrontEnd.WebUi.Layouts.Models;

public sealed class DisplayInfo : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsDarkMode
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;

                OnPropertyChanged();
            }
        }
    } = false;

    public bool IsDrawerOpen
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;

                OnPropertyChanged();
            }
        }
    } = true;

    public string IconForDarkMode => IsDarkMode
        ? Icons.Material.TwoTone.LightMode
        : Icons.Material.TwoTone.DarkMode;

    public string IconForDrawer => IsDrawerOpen
        ? Icons.Material.Filled.ChevronLeft
        : Icons.Material.Filled.ChevronRight;

    public Color CalendarColor => IsDarkMode
        ? Color.Default
        : Color.Surface;

    public string SrcLogo
    {
        get
        {
            if (!IsDarkMode && IsDrawerOpen)
            {
                return "img/logo-pertamina-colorful-ppn.svg";
            }

            if (!IsDarkMode && !IsDrawerOpen)
            {
                return "img/logo-pertamina-colorful-small.svg";
            }

            if (IsDrawerOpen)
            {
                return "img/logo-pertamina-white-ppn.svg";
            }

            return "img/logo-pertamina-white-small.svg";
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = default)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }
}
