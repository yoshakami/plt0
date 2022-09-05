using System;

class Preceptual_Brightness_class
{
    // https://stackoverflow.com/questions/596216/formula-to-determine-perceived-brightness-of-rgb-color
    // I found this one while experimenting cmpr algorithms and looking for "perceptual brightness"
    // sRGB luminance(Y) values
    const double rY = 0.212655;
    const double gY = 0.715158;
    const double bY = 0.072187;

    // Inverse of sRGB "gamma" function. (approx 2.2)
    double inv_gam_sRGB(int ic)
    {
        double c = ic / 255.0;
        if (c <= 0.04045)
            return c / 12.92;
        else
            return Math.Pow(((c + 0.055) / (1.055)), 2.4);
    }

    // sRGB "gamma" function (approx 2.2)
    int gam_sRGB(double v)
    {
        if (v <= 0.0031308)
            v *= 12.92;
        else
            v = 1.055 * Math.Pow(v, 1.0 / 2.4) - 0.055;
        return (int)(v * 255 + 0.5); // This is correct in C++. Other languages may not
                                     // require +0.5
    }

    // GRAY VALUE ("brightness")
    public int Preceptual_Brightness(int r, int g, int b)
    {
        return gam_sRGB(
                rY * inv_gam_sRGB(r) +
                gY * inv_gam_sRGB(g) +
                bY * inv_gam_sRGB(b)
        );
    }
}