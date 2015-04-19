using UnityEngine;
using System.Collections;

public static class ExtensionMethods
{
    public static Color FromHSV(float h, float s, float v)
    {
        float c = v * s;
        float hPrime = h / 60.0f;
        float x = c * (1.0f - Mathf.Abs((hPrime % 2.0f) - 1.0f));

        if (0.0f <= hPrime && hPrime < 1.0f)
            return new Color(c, x, 0.0f);
        else if (1.0f <= hPrime && hPrime < 2.0f)
            return new Color(x, c, 0.0f);
        else if (2.0f <= hPrime && hPrime < 3.0f)
            return new Color(0.0f, c, x);
        else if (3.0f <= hPrime && hPrime < 4.0f)
            return new Color(0.0f, x, c);
        else if (4.0f <= hPrime && hPrime < 5.0f)
            return new Color(x, 0.0f, c);
        else if (5.0f <= hPrime && hPrime < 6.0f)
            return new Color(c, 0.0f, x);

        return Color.black;
    }

    public static Demeanor Increment(this Demeanor effect)
    {
        switch (effect)
        {
            case Demeanor.Slow:
                return Demeanor.Normal;
            case Demeanor.Normal:
                return Demeanor.Angry;
            case Demeanor.Angry:
                return Demeanor.Angry;
        }

        return Demeanor.Normal;
    }

    public static Demeanor Decrement(this Demeanor effect)
    {
        switch (effect)
        {
            case Demeanor.Slow:
                return Demeanor.Slow;
            case Demeanor.Normal:
                return Demeanor.Slow;
            case Demeanor.Angry:
                return Demeanor.Normal;
        }

        return Demeanor.Normal;
    }

    public static Vector3 ProjectY(this Vector3 v, float y)
    {
        return new Vector3(v.x, y, v.z);
    }
}
