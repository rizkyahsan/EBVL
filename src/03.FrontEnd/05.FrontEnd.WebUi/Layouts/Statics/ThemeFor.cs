using MudBlazor.Utilities;

namespace EBVL.FrontEnd.WebUi.Layouts.Statics;

public static class ThemeFor
{
    public static readonly MudTheme Default = new()
    {
        PaletteLight = new PaletteLight()
        {
            Primary = new MudColor(0, 167, 111, 1.0),
            PrimaryContrastText = new MudColor(255, 255, 255, 1.0),

            Secondary = new MudColor(142, 51, 255, 1.0),
            SecondaryContrastText = new MudColor(255, 255, 255, 1.0),

            Tertiary = new MudColor(242, 175, 59, 1.0),
            TertiaryContrastText = new MudColor(255, 255, 255, 1.0),

            Info = new MudColor(0, 184, 217, 1.0),
            InfoContrastText = new MudColor(255, 255, 255, 1.0),

            Success = new MudColor(34, 197, 94, 1.0),
            SuccessContrastText = new MudColor(255, 255, 255, 1.0),

            Warning = new MudColor(255, 171, 0, 1.0),
            WarningContrastText = new MudColor(255, 255, 255, 1.0),

            Error = new MudColor(255, 86, 48, 1.0),
            ErrorContrastText = new MudColor(255, 255, 255, 1.0),

            Dark = new MudColor(28, 37, 46, 1.0),
            DarkContrastText = new MudColor(255, 255, 255, 1.0),

            White = new MudColor(255, 255, 255, 1.0),
            GrayLighter = "#DFE3E8",
            GrayLight = "#C4CDD5",
            GrayDefault = "#919EAB",
            GrayDark = "#637381",
            GrayDarker = "#454F5B",
            Black = new MudColor(39, 44, 52, 1.0),

            TextPrimary = new MudColor(28, 37, 46, 1.0),
            TextSecondary = new MudColor(0, 0, 0, 0.5372549019607843),
            TextDisabled = new MudColor(0, 0, 0, 0.3764705882352941),

            Surface = new MudColor(255, 255, 255, 1.0),

            Background = new MudColor(255, 255, 255, 1.0),
            BackgroundGray = new MudColor(244, 246, 248, 1.0),
            AppbarBackground = new MudColor(255, 255, 255, 0.8),
            AppbarText = new MudColor(0, 45, 24, 1.0),
            DrawerBackground = new MudColor(255, 255, 255, 1.0),
            DrawerText = new MudColor(99, 115, 129, 1.0),
            DrawerIcon = new MudColor(99, 115, 129, 1.0),

            ActionDefault = new MudColor(28, 37, 46, 1.0),
            ActionDisabled = new MudColor(145, 158, 171, 0.8),
            ActionDisabledBackground = new MudColor(145, 158, 171, 0.24),

            LinesDefault = new MudColor(145, 158, 171, 0.2),
            LinesInputs = new MudColor(145, 158, 171, 0.2),

            TableLines = new MudColor(145, 158, 171, 0.2),
            TableStriped = new MudColor(0, 0, 0, 0.0196078431372549),
            TableHover = new MudColor(0, 0, 0, 0.0392156862745098),

            Divider = new MudColor(224, 224, 224, 1.0),
            DividerLight = new MudColor(0, 0, 0, 0.8),
            Skeleton = new MudColor(0, 0, 0, 0.10980392156862745),

            BorderOpacity = 1.0,
            HoverOpacity = 0.06,
            RippleOpacity = 0.1,
            RippleOpacitySecondary = 0.2,

            OverlayDark = new MudColor("#212121").SetAlpha(0.5).ToString(MudColorOutputFormats.RGBA),
            OverlayLight = new MudColor(Colors.Shades.White).SetAlpha(0.5).ToString(MudColorOutputFormats.RGBA)
        },
        PaletteDark = new PaletteDark()
        {
            Primary = new MudColor(0, 167, 111, 1.0),
            PrimaryContrastText = new MudColor(255, 255, 255, 1.0),

            Secondary = new MudColor(142, 51, 255, 1.0),
            SecondaryContrastText = new MudColor(255, 255, 255, 1.0),

            Tertiary = new MudColor(242, 175, 59, 1.0),
            TertiaryContrastText = new MudColor(255, 255, 255, 1.0),

            Info = new MudColor(0, 184, 217, 1.0),
            InfoContrastText = new MudColor(255, 255, 255, 1.0),

            Success = new MudColor(34, 197, 94, 1.0),
            SuccessContrastText = new MudColor(255, 255, 255, 1.0),

            Warning = new MudColor(255, 171, 0, 1.0),
            WarningContrastText = new MudColor(255, 255, 255, 1.0),

            Error = new MudColor(255, 86, 48, 1.0),
            ErrorContrastText = new MudColor(255, 255, 255, 1.0),

            Dark = new MudColor(255, 255, 255, 1.0),
            DarkContrastText = new MudColor(28, 37, 46, 1.0),

            White = new MudColor(255, 255, 255, 1.0),
            GrayLighter = "#DFE3E8",
            GrayLight = "#C4CDD5",
            GrayDefault = "#919EAB",
            GrayDark = "#637381",
            GrayDarker = "#454F5B",
            Black = new MudColor(39, 44, 52, 1.0),

            TextPrimary = new MudColor(255, 255, 255, 1.0),
            TextSecondary = new MudColor(255, 255, 255, 0.4980392156862745),
            TextDisabled = new MudColor(255, 255, 255, 0.2),

            Surface = new MudColor(55, 55, 64, 1.0),

            Background = new MudColor(20, 26, 33, 1.0),
            BackgroundGray = new MudColor(40, 50, 61, 1.0),
            AppbarBackground = new MudColor(20, 26, 33, 0.8),
            AppbarText = new MudColor(255, 255, 255, 1.0),
            DrawerBackground = new MudColor(20, 26, 33, 1.0),
            DrawerIcon = new MudColor(255, 255, 255, 0.5),
            DrawerText = new MudColor(255, 255, 255, 0.5),

            ActionDefault = new MudColor(255, 255, 255, 1.0),
            ActionDisabled = new MudColor(255, 255, 255, 0.25882352941176473),
            ActionDisabledBackground = new MudColor(255, 255, 255, 0.11764705882352941),

            LinesDefault = new MudColor(145, 158, 171, 0.2),
            LinesInputs = new MudColor(145, 158, 171, 0.2),

            TableLines = new MudColor(255, 255, 255, 0.11764705882352941),
            TableStriped = new MudColor(255, 255, 255, 0.2),
            TableHover = new MudColor(0, 0, 0, 0.0392156862745098),

            Divider = new MudColor(255, 255, 255, 0.11764705882352941),
            DividerLight = new MudColor(255, 255, 255, 0.058823529411764705),
            Skeleton = new MudColor(255, 255, 255, 0.10980392156862745),

            BorderOpacity = 1.0,
            HoverOpacity = 0.06,
            RippleOpacity = 0.1,
            RippleOpacitySecondary = 0.2,

            OverlayDark = new MudColor("#212121").SetAlpha(0.5).ToString(MudColorOutputFormats.RGBA),
            OverlayLight = new MudColor(Colors.Shades.White).SetAlpha(0.5).ToString(MudColorOutputFormats.RGBA)
        },
        Shadows = new Shadow()
        {
            Elevation =
            [
                /* Elevation = 0 */ "none",
                /* Elevation = 1 */ "0px 2px 1px -1px rgba(145 158 171 / 20%), 0px 1px 1px 0px rgba(145 158 171 / 14%), 0px 1px 3px 0px rgba(145 158 171 / 12%)",
                /* Elevation = 2 */ "0px 3px 1px -2px rgba(145 158 171 / 20%), 0px 2px 2px 0px rgba(145 158 171 / 14%), 0px 1px 5px 0px rgba(145 158 171 / 12%)",
                /* Elevation = 3 */ "0px 3px 3px -2px rgba(145 158 171 / 20%), 0px 3px 4px 0px rgba(145 158 171 / 14%), 0px 1px 8px 0px rgba(145 158 171 / 12%)",
                /* Elevation = 4 */ "0px 2px 4px -1px rgba(145 158 171 / 20%), 0px 4px 5px 0px rgba(145 158 171 / 14%), 0px 1px 10px 0px rgba(145 158 171 / 12%)",
                /* Elevation = 5 */ "0px 3px 5px -1px rgba(145 158 171 / 20%), 0px 5px 8px 0px rgba(145 158 171 / 14%), 0px 1px 14px 0px rgba(145 158 171 / 12%)",
                /* Elevation = 6 */ "0px 3px 5px -1px rgba(145 158 171 / 20%), 0px 6px 10px 0px rgba(145 158 171 / 14%), 0px 1px 18px 0px rgba(145 158 171 / 12%)",
                /* Elevation = 7 */ "0px 4px 5px -2px rgba(145 158 171 / 20%), 0px 7px 10px 1px rgba(145 158 171 / 14%), 0px 2px 16px 1px rgba(145 158 171 / 12%)",
                /* Elevation = 8 */ "0px 5px 5px -3px rgba(145 158 171 / 20%), 0px 8px 10px 1px rgba(145 158 171 / 14%), 0px 3px 14px 2px rgba(145 158 171 / 12%)",
                /* Elevation = 9 */ "0px 5px 6px -3px rgba(145 158 171 / 20%), 0px 9px 12px 1px rgba(145 158 171 / 14%), 0px 3px 16px 2px rgba(145 158 171 / 12%)",
                /* Elevation = 10 */ "0px 6px 6px -3px rgba(145 158 171 / 20%), 0px 10px 14px 1px rgba(145 158 171 / 14%), 0px 4px 18px 3px rgba(145 158 171 / 12%)",
                /* Elevation = 11 */ "0px 6px 7px -4px rgba(145 158 171 / 20%), 0px 11px 15px 1px rgba(145 158 171 / 14%), 0px 4px 20px 3px rgba(145 158 171 / 12%)",
                /* Elevation = 12 */ "0px 7px 8px -4px rgba(145 158 171 / 20%), 0px 12px 17px 2px rgba(145 158 171 / 14%), 0px 5px 22px 4px rgba(145 158 171 / 12%)",
                /* Elevation = 13 */ "0px 7px 8px -4px rgba(145 158 171 / 20%), 0px 13px 19px 2px rgba(145 158 171 / 14%), 0px 5px 24px 4px rgba(145 158 171 / 12%)",
                /* Elevation = 14 */ "0px 7px 9px -4px rgba(145 158 171 / 20%), 0px 14px 21px 2px rgba(145 158 171 / 14%), 0px 5px 26px 4px rgba(145 158 171 / 12%)",
                /* Elevation = 15 */ "0px 8px 9px -5px rgba(145 158 171 / 20%), 0px 15px 22px 2px rgba(145 158 171 / 14%), 0px 6px 28px 5px rgba(145 158 171 / 12%)",
                /* Elevation = 16 */ "0px 8px 10px -5px rgba(145 158 171 / 20%), 0px 16px 24px 2px rgba(145 158 171 / 14%), 0px 6px 30px 5px rgba(145 158 171 / 12%)",
                /* Elevation = 17 */ "0px 8px 11px -5px rgba(145 158 171 / 20%), 0px 17px 26px 2px rgba(145 158 171 / 14%), 0px 6px 32px 5px rgba(145 158 171 / 12%)",
                /* Elevation = 18 */ "0px 9px 11px -5px rgba(145 158 171 / 20%), 0px 18px 28px 2px rgba(145 158 171 / 14%), 0px 7px 34px 6px rgba(145 158 171 / 12%)",
                /* Elevation = 19 */ "0px 9px 12px -6px rgba(145 158 171 / 20%), 0px 19px 29px 2px rgba(145 158 171 / 14%), 0px 7px 36px 6px rgba(145 158 171 / 12%)",
                /* Elevation = 20 */ "0px 10px 13px -6px rgba(145 158 171 / 20%), 0px 20px 31px 3px rgba(145 158 171 / 14%), 0px 8px 38px 7px rgba(145 158 171 / 12%)",
                /* Elevation = 21 */ "0px 10px 13px -6px rgba(145 158 171 / 20%), 0px 21px 33px 3px rgba(145 158 171 / 14%), 0px 8px 40px 7px rgba(145 158 171 / 12%)",
                /* Elevation = 22 */ "0px 10px 14px -6px rgba(145 158 171 / 20%), 0px 22px 35px 3px rgba(145 158 171 / 14%), 0px 8px 42px 7px rgba(145 158 171 / 12%)",
                /* Elevation = 23 */ "0px 11px 14px -7px rgba(145 158 171 / 20%), 0px 23px 36px 3px rgba(145 158 171 / 14%), 0px 9px 44px 8px rgba(145 158 171 / 12%)",
                /* Elevation = 24 */ "0px 11px 15px -7px rgba(145 158 171 / 20%), 0px 24px 38px 3px rgba(145 158 171 / 14%), 0px 9px 46px 8px rgba(145 158 171 / 12%)",
                /* Elevation = 25 */ "0px 0px 2px 0px rgba(145 158 171 / 20%), 0px 12px 24px -4px rgba(145 158 171 / 12%)"
            ]
        },
        LayoutProperties = new LayoutProperties()
        {
            DefaultBorderRadius = "6px",
            DrawerMiniWidthLeft = "88px",
            DrawerMiniWidthRight = "88px",
            DrawerWidthLeft = "300px",
            DrawerWidthRight = "300px",
            AppbarHeight = "64px"
        },
        Typography = new Typography()
        {
            Default = new DefaultTypography()
            {
                FontFamily = ["Public Sans Variable", "Segoe UI", "sans-serif"],
                FontWeight = "400",
                FontSize = "0.875rem",
                LineHeight = "1.5",
                LetterSpacing = "normal",
                TextTransform = "none"
            },
            H1 = new H1Typography()
            {
                FontFamily = ["Public Sans Variable", "Segoe UI", "sans-serif"],
                FontWeight = "700",
                FontSize = "2.000rem",
                LineHeight = "1.25",
                LetterSpacing = "normal",
                TextTransform = "none"
            },
            H2 = new H2Typography()
            {
                FontFamily = ["Public Sans Variable", "Segoe UI", "sans-serif"],
                FontWeight = "700",
                FontSize = "1.500rem",
                LineHeight = "1.33",
                LetterSpacing = "normal",
                TextTransform = "none"
            },
            H3 = new H3Typography()
            {
                FontFamily = ["Public Sans Variable", "Segoe UI", "sans-serif"],
                FontWeight = "600",
                FontSize = "1.250rem",
                LineHeight = "1.5",
                LetterSpacing = "normal",
                TextTransform = "none"
            },
            H4 = new H4Typography()
            {
                FontFamily = ["Public Sans Variable", "Segoe UI", "sans-serif"],
                FontWeight = "600",
                FontSize = "1.125rem",
                LineHeight = "1.5",
                LetterSpacing = "normal",
                TextTransform = "none"
            },
            H5 = new H5Typography()
            {
                FontFamily = ["Public Sans Variable", "Segoe UI", "sans-serif"],
                FontWeight = "500",
                FontSize = "1.000rem",
                LineHeight = "1.5",
                LetterSpacing = "normal",
                TextTransform = "none"
            },
            H6 = new H6Typography()
            {
                FontFamily = ["Public Sans Variable", "Segoe UI", "sans-serif"],
                FontWeight = "500",
                FontSize = "1.000rem",
                LineHeight = "1.57",
                LetterSpacing = "normal",
                TextTransform = "none"
            },
            Subtitle1 = new Subtitle1Typography()
            {
                FontFamily = ["Public Sans Variable", "Segoe UI", "sans-serif"],
                FontWeight = "400",
                FontSize = "0.9375rem",
                LineHeight = "1.5",
                LetterSpacing = "normal",
                TextTransform = "none"
            },
            Subtitle2 = new Subtitle2Typography()
            {
                FontFamily = ["Public Sans Variable", "Segoe UI", "sans-serif"],
                FontWeight = "400",
                FontSize = "0.875rem",
                LineHeight = "1.5",
                LetterSpacing = "normal",
                TextTransform = "none"
            },
            Body1 = new Body1Typography()
            {
                FontFamily = ["Public Sans Variable", "Segoe UI", "sans-serif"],
                FontWeight = "400",
                FontSize = "0.875rem",
                LineHeight = "1.5",
                LetterSpacing = "normal",
                TextTransform = "none"
            },
            Body2 = new Body2Typography()
            {
                FontFamily = ["Public Sans Variable", "Segoe UI", "sans-serif"],
                FontWeight = "400",
                FontSize = "0.750rem",
                LineHeight = "1.57",
                LetterSpacing = "normal",
                TextTransform = "none"
            },
            Caption = new CaptionTypography()
            {
                FontFamily = ["Public Sans Variable", "Segoe UI", "sans-serif"],
                FontWeight = "400",
                FontSize = "0.750rem",
                LineHeight = "1.57",
                LetterSpacing = "normal",
                TextTransform = "none"
            },
            Overline = new OverlineTypography()
            {
                FontFamily = ["Public Sans Variable", "Segoe UI", "sans-serif"],
                FontWeight = "700",
                FontSize = "0.750rem",
                LineHeight = "1.57",
                LetterSpacing = "normal",
                TextTransform = "uppercase"
            },
            Button = new ButtonTypography()
            {
                FontFamily = ["Public Sans Variable", "Segoe UI", "sans-serif"],
                FontWeight = "700",
                FontSize = "0.875rem",
                LineHeight = "1.5",
                LetterSpacing = "normal",
                TextTransform = "none"
            }
        }
    };
}
