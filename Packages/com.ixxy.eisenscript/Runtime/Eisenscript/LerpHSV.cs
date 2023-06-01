using UnityEngine;

public static class LerpHSV
{
    public static Color Lerp(Color a, Color b, float x)
    {
        Vector3 ah = RGB2HSV(a);
        Vector3 bh = RGB2HSV(b);
        return Color.HSVToRGB(
            Mathf.LerpAngle(ah.x, bh.x, x),
            Mathf.Lerp(ah.y, bh.y, x),
            Mathf.Lerp(ah.z, bh.z, x)
        );
    }

    public static Color Lerp(float r0, float g0, float b0, float r1, float g1, float b1, float x)
    {
        Vector3 ah = RGB2HSV(new Color(r0, g0, b0));
        Vector3 bh = RGB2HSV(new Color(r1, g1, b1));
        return Color.HSVToRGB(
            Mathf.LerpAngle(ah.x, bh.x, x),
            Mathf.Lerp(ah.y, bh.y, x),
            Mathf.Lerp(ah.z, bh.z, x)
        );
    }

    static Vector3 RGB2HSV(Color color)
    {
        float cmax = Mathf.Max(color.r, color.g, color.b);
        float cmin = Mathf.Min(color.r, color.g, color.b);
        float delta = cmax - cmin;

        float hue = 0;
        float saturation = 0;

        if (cmax == color.r)
        {
            hue = 60 * (((color.g - color.b) / delta) % 6);
        }
        else if (cmax == color.g)
        {
            hue = 60 * ((color.b - color.r) / delta + 2);
        }
        else if (cmax == color.b)
        {
            hue = 60 * ((color.r - color.g) / delta + 4);
        }

        if (cmax > 0)
        {
            saturation = delta / cmax;
        }

        return new Vector3(hue, saturation, cmax);
    }

    static Color HSV2RGB(Vector3 color)
    {
        float hue = color.x;
        float c = color.z * color.y;
        float x = c * (1 - Mathf.Abs((hue / 60) % 2 - 1));
        float m = color.z - c;

        float r = 0;
        float g = 0;
        float b = 0;

        if (hue < 60)
        {
            r = c;
            g = x;
        }
        else if (hue < 120)
        {
            r = x;
            g = c;
        }
        else if (hue < 180)
        {
            g = c;
            b = x;
        }
        else if (hue < 240)
        {
            g = x;
            b = c;
        }
        else if (hue < 300)
        {
            r = x;
            b = c;
        }
        else
        {
            r = c;
            b = x;
        }

        return new Color(r + m, g + m, b + m);
    }
}